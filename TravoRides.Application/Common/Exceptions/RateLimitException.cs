namespace TravoRiders.Application.Common.Exceptions
{
    public class RateLimitException : AppException
    {
        public RateLimitException(string message) : base(message, 429)
        {
        }
    }
}
