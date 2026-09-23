using System.Linq.Expressions;
using Hangfire;
using TravoRides.Application.Interfaces;

namespace TravoRides.Infrastructure.Services
{
    public class HangfireJobScheduler : IJobScheduler
    {
        private readonly IBackgroundJobClient _client;
        private readonly IRecurringJobManager _recurringJobManager;

        public HangfireJobScheduler(IBackgroundJobClient client, IRecurringJobManager recurringJobManager)
        {
            _client = client;
            _recurringJobManager = recurringJobManager;
        }

        public string Enqueue<T>(Expression<Action<T>> methodCall)
            => _client.Enqueue(methodCall);

        public void Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay)
            => _client.Schedule(methodCall, delay);

        public void AddOrUpdateRecurring<T>(string jobId, Expression<Action<T>> methodCall, string cronExpression)
            => _recurringJobManager.AddOrUpdate(jobId, methodCall, cronExpression);
    }
}

