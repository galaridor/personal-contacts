using System.Text.Json;
using Contacts.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Contacts.Api.Middleware
{
	internal sealed class GlobalExceptionHandler(
		IProblemDetailsService problemDetailsService,
		ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
	{
		public async ValueTask<bool> TryHandleAsync(
			HttpContext httpContext,
			Exception exception,
			CancellationToken cancellationToken)
		{
			ProblemDetails? problemDetails = exception switch
			{
				ValidationException validationException => FromValidationException(validationException),
				DomainException domainException => FromDomainException(domainException),
				BadHttpRequestException => MalformedRequest,
				_ => null,
			};

			if (problemDetails is null)
			{
				logger.LogError(
					exception,
					"Unhandled exception while processing {Method} {Path}.",
					httpContext.Request.Method,
					httpContext.Request.Path);

				problemDetails = new ProblemDetails
				{
					Status = StatusCodes.Status500InternalServerError,
					Title = "An unexpected error occurred.",
					Detail = "The request could not be completed.",
				};
			}
			else
			{
				logger.LogInformation(
					"Rejected {Method} {Path}: {Reason}.",
					httpContext.Request.Method,
					httpContext.Request.Path,
					problemDetails.Title);
			}

			httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;

			return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
			{
				HttpContext = httpContext,
				ProblemDetails = problemDetails,
				Exception = exception,
			});
		}

		private static ProblemDetails MalformedRequest => new()
		{
			Status = StatusCodes.Status400BadRequest,
			Title = "The request could not be read.",
			Detail = "The request body is missing or is not a valid JSON.",
		};

		private static ProblemDetails FromValidationException(ValidationException exception)
		{
			Dictionary<string, string[]> errors = exception.Errors
				.GroupBy(failure => ToClientPropertyName(failure.PropertyName))
				.ToDictionary(
					group => group.Key,
					group => group.Select(failure => failure.ErrorMessage).Distinct().ToArray());

			return new ValidationProblemDetails(errors)
			{
				Status = StatusCodes.Status400BadRequest,
				Title = "One or more validation errors occurred.",
			};
		}

		private static ProblemDetails FromDomainException(DomainException exception)
		{
			if (string.IsNullOrEmpty(exception.PropertyName))
			{
				return new ProblemDetails
				{
					Status = StatusCodes.Status400BadRequest,
					Title = "The request could not be processed.",
					Detail = exception.Message,
				};
			}

			Dictionary<string, string[]> errors = new Dictionary<string, string[]>
			{
				[ToClientPropertyName(exception.PropertyName)] = [exception.Message],
			};

			return new ValidationProblemDetails(errors)
			{
				Status = StatusCodes.Status400BadRequest,
				Title = "One or more validation errors occurred.",
			};
		}

		private static string ToClientPropertyName(string propertyName) => string.Join(
			'.',
			propertyName.Split('.').Select(JsonNamingPolicy.CamelCase.ConvertName));
	}
}
