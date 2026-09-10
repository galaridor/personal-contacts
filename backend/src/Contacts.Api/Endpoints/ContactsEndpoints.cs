using Contacts.Api.Contracts.Common;
using Contacts.Api.Contracts.Contacts;
using Contacts.Api.Extensions;
using Contacts.Application.Common.Models;
using Contacts.Application.Common.Results;
using Contacts.Application.Contacts.Commands.CreateContact;
using Contacts.Application.Contacts.Commands.DeleteContact;
using Contacts.Application.Contacts.Commands.UpdateContact;
using Contacts.Application.Contacts.Models;
using Contacts.Application.Contacts.Queries.GetContact;
using Contacts.Application.Contacts.Queries.GetContacts;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Contacts.Api.Endpoints
{
	internal static class ContactsEndpoints
	{
		private const string GetContactRouteName = "GetContactById";

		public static IEndpointRouteBuilder MapContactsEndpoints(this IEndpointRouteBuilder endpoints)
		{
			RouteGroupBuilder contacts = endpoints
				.MapGroup("/api/contacts")
				.WithTags("Contacts");

			contacts.MapGet("/", ListAsync)
				.WithSummary("Lists one page of contacts, optionally filtered by first name or surname.")
				.ProducesValidationProblem();

			contacts.MapGet("/{id:guid}", GetByIdAsync)
				.WithName(GetContactRouteName)
				.WithSummary("Returns a single contact.")
				.ProducesProblem(StatusCodes.Status404NotFound);

			contacts.MapPost("/", CreateAsync)
				.WithSummary("Creates a contact.")
				.ProducesValidationProblem();

			contacts.MapPut("/{id:guid}", UpdateAsync)
				.WithSummary("Replaces every field of an existing contact.")
				.ProducesValidationProblem()
				.ProducesProblem(StatusCodes.Status404NotFound);

			contacts.MapDelete("/{id:guid}", DeleteAsync)
				.WithSummary("Deletes a contact.")
				.ProducesProblem(StatusCodes.Status404NotFound);

			return endpoints;
		}

		private static async Task<Results<Ok<PagedResponse<ContactResponse>>, ProblemHttpResult>> ListAsync(
			string? search,
			int? page,
			int? pageSize,
			GetContactsQueryHandler getContacts,
			CancellationToken cancellationToken)
		{
			GetContactsQuery query = new(
				search,
				page ?? Paging.DefaultPage,
				pageSize ?? Paging.DefaultPageSize);

			Result<PagedResult<ContactDto>> result = await getContacts.HandleAsync(query, cancellationToken);

			return result.IsSuccess
				? TypedResults.Ok(result.Value.ToResponse())
				: result.Error.ToProblem();
		}

		private static async Task<Results<Ok<ContactResponse>, ProblemHttpResult>> GetByIdAsync(
			Guid id,
			GetContactQueryHandler getContact,
			CancellationToken cancellationToken)
		{
			Result<ContactDto> result = await getContact.HandleAsync(new GetContactQuery(id), cancellationToken);

			return result.IsSuccess
				? TypedResults.Ok(result.Value.ToResponse())
				: result.Error.ToProblem();
		}

		private static async Task<Results<CreatedAtRoute<ContactResponse>, ProblemHttpResult>> CreateAsync(
			CreateContactRequest request,
			CreateContactCommandHandler createContact,
			CancellationToken cancellationToken)
		{
			Result<ContactDto> result = await createContact.HandleAsync(request.ToCommand(), cancellationToken);

			if (result.IsFailure)
			{
				return result.Error.ToProblem();
			}

			ContactResponse contact = result.Value.ToResponse();

			// Including the route to retrieve the newely created contact in the location header of the response
			return TypedResults.CreatedAtRoute(contact, GetContactRouteName, new { id = contact.Id });
		}

		private static async Task<Results<Ok<ContactResponse>, ProblemHttpResult>> UpdateAsync(
			Guid id,
			UpdateContactRequest request,
			UpdateContactCommandHandler updateContact,
			CancellationToken cancellationToken)
		{
			Result<ContactDto> result = await updateContact.HandleAsync(request.ToCommand(id), cancellationToken);

			return result.IsSuccess
				? TypedResults.Ok(result.Value.ToResponse())
				: result.Error.ToProblem();
		}

		private static async Task<Results<NoContent, ProblemHttpResult>> DeleteAsync(
			Guid id,
			DeleteContactCommandHandler deleteContact,
			CancellationToken cancellationToken)
		{
			Result result = await deleteContact.HandleAsync(new DeleteContactCommand(id), cancellationToken);

			return result.IsSuccess
				? TypedResults.NoContent()
				: result.Error.ToProblem();
		}
	}
}
