using System.Linq.Expressions;

namespace TravoRides.Application.Interfaces
{
    public interface IJobScheduler
    {
        string Enqueue<T>(Expression<Action<T>> methodCall);
        void Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay);
        void AddOrUpdateRecurring<T>(string jobId, Expression<Action<T>> methodCall, string cronExpression);
    }
}

