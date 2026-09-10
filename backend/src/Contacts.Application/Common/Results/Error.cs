namespace Contacts.Application.Common.Results
{
	public sealed record Error(string Code, ErrorType Type, string Message)
	{
		public static readonly Error None = new(string.Empty, ErrorType.Failure, string.Empty);

		public static Error NotFound(string code, string message) => new(code, ErrorType.NotFound, message);
	}

	public enum ErrorType
	{
		Failure,

		NotFound,
	}
}
