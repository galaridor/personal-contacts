using Contacts.Application.Common.Results;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Contacts.Api.Extensions
{
	internal static class ResultExtensions
	{
		public static ProblemHttpResult ToProblem(this Error error)
		{
			(int status, string? title) = error.Type switch
			{
				ErrorType.NotFound => (StatusCodes.Status404NotFound, "Resource not found."),
				_ => (StatusCodes.Status400BadRequest, "The request could not be processed."),
			};

			return TypedResults.Problem(detail: error.Message, statusCode: status, title: title);
		}
	}
}
