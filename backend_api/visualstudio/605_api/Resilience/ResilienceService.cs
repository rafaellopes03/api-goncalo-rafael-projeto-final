using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace _605_api.Resilience;

public class ResilienceService
{
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;

    public ResilienceService()
    {
        // Tenta 3 vezes com 2 segundos de espera entre tentativas
        _retryPolicy = Policy
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(2),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    Console.WriteLine($"[Retry] Tentativa {retryCount} após erro: {exception.Message}");
                });

        // Abre o circuito após 3 falhas consecutivas durante 30 segundos
        _circuitBreakerPolicy = Policy
            .Handle<HttpRequestException>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: 3,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (exception, duration) =>
                {
                    Console.WriteLine($"[CircuitBreaker] Circuito aberto por {duration.TotalSeconds}s — {exception.Message}");
                },
                onReset: () =>
                {
                    Console.WriteLine("[CircuitBreaker] Circuito fechado — serviço recuperado.");
                });
    }

    public AsyncRetryPolicy RetryPolicy => _retryPolicy;
    public AsyncCircuitBreakerPolicy CircuitBreakerPolicy => _circuitBreakerPolicy;

    public async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
    {
        return await _retryPolicy.WrapAsync(_circuitBreakerPolicy).ExecuteAsync(action);
    }
}