namespace TravoRiders.Application.Common.Exceptions
{
    public class AuthenticationException : AppException
    {
        public AuthenticationException(string message)
            : base(message, 401)
        {
        }
    }
}
