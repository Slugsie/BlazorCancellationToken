using Microsoft.AspNetCore.Mvc;

namespace BlazorCancellationToken.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CancellationController : ControllerBase
    {
        private readonly ILogger<CancellationController> _logger;

        public CancellationController(
            ILogger<CancellationController> logger
            )
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<String> GetResult(CancellationToken ct)
        {
            var delay = 500;
            var iterations = 10;
            var total = 0;

            try
            {
                _logger.LogInformation("GetResult called, waiting {delay * iterations}ms", delay * iterations);

                // Run several consecutive long running tasks.
                // In this case it's a simple Task.Delay for demonstration purposes
                for (int loop = 0; loop < iterations; loop++)
                {
                    await Task.Delay(delay, ct);
                    total++;
                }

                _logger.LogInformation($"GetResult exiting after {total} iterations, OK");
                return $"OK, total = {total}";
            }
            // The exact exception thrown will depend on the method that was cancelled. Task.Delay throws a TaskCanceledException.
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "The task was cancelled after {total} iterations.", total);
                return $"Task cancelled";
            }
            // Some other exception was thrown
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in GetResult");
                return $"Failed, total = {total}";
            }
        }
    }
}
