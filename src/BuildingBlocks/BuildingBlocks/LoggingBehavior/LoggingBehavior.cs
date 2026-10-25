using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BuildingBlocks.LoggingBehavior
{

    public class LoggingBehavior<TRequest, TResponse>
        (ILogger<LoggingBehavior<TRequest, TResponse>> logger) 
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull, IRequest<TResponse>
        where TResponse : notnull
    {
        
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            logger.LogInformation("[START] Handle request={Request} - response={Response} - requestData={requestData}", typeof(TRequest).Name, typeof(TResponse).Name, request);
            
            var timer = Stopwatch.StartNew();
            timer.Start();
            
            var response = await next();
            timer.Stop();

            var timeTaken = timer.Elapsed;

            if (timeTaken.Seconds > 3) // Log a warning if the request took more than 3 seconds
                logger.LogWarning("[PREFORMANCE] Handle request={Request} - response={Response} took {TimeTaken} seconds", typeof(TRequest).Name, typeof(TResponse).Name, timeTaken.Seconds);
            
            logger.LogInformation("[END] Handled request={Request} with response={Response} and took {TimeTaken} seconds", typeof(TRequest).Name, typeof(TResponse).Name, timeTaken.Seconds);

            return response;
        }
    }
}
