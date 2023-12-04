namespace CodeVijetWeb.Services;

/*
 * Сервис отслеживающий файлы для 
 * отображения на Виджете
 * by TheCrazyWolf
 */
public class WatchDogService : BackgroundService
{
    /* Путь для отслеживания */
    public string PathForTracking { get; set; } = "";
    
    /* Таймер для службы */
    private readonly int _timeWait = 1000;
    public WatchDogService()
    {
        
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }
}