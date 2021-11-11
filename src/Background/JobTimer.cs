namespace CacheService.Background;

internal class JobTimer : IDisposable
{
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly IJobManager _bgManager;
    private readonly Timer _timer;

    public JobTimer(IJobManager bgManager, CacheServiceConfiguration configuration)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _bgManager = bgManager;
        _timer = new Timer(OnTimerCallback, default, 0, (int)configuration.BackgroundJobInterval.TotalMilliseconds);
    }

    private async void OnTimerCallback(object? state)
    {
        await _bgManager.ExecuteJobsAsync(_cancellationTokenSource.Token);
    }

    public void Dispose()
    {
        if (_cancellationTokenSource.IsCancellationRequested) return;

        _cancellationTokenSource.Cancel();
        _timer.Dispose();
    }
}
