namespace TravoRides.Application.Common.Exceptions
{
    public class AuthorizationFailureException : AppException
    {
        public AuthorizationFailureException(string message)
            : base(message, 403)
        {
        }
    }
}
