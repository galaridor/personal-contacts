namespace Contacts.Api.Contracts.Common
{
	internal sealed record PagedResponse<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount);
}
