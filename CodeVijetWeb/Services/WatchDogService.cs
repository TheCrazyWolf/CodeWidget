using CodeVijetWeb.Models;

namespace CodeVijetWeb.Services;

/*
 * Сервис отслеживающий файлы для
 * отображения на Виджете
 * by TheCrazyWolf
 */
public class WatchDogService : BackgroundService
{
    /* Путь для отслеживания */
    private string _pathForTracking = "";

    /* Таймер для службы */
    private readonly int _timeWait = 1000;

    /*
     * Черный контейнер для игнора
     * чтобы за забивать всю оперативку
     */
    private readonly IEnumerable<string> _blackContainerPaths = new List<string>()
    {
        ".git", ".idea", "obj", "bin"
    };

    /* Виджеты с кодом */
    private IList<CodeWidget> _widgets = new List<CodeWidget>();

    /* Теги для отслеживания файлов, которые можно прочитать */
    private const string _tagTrackCopyable = "// track";
    private const string _tagTrackNoCopyable = "// nocopy";

    public WatchDogService()
    {
        Task.Run(() => ExecuteAsync(new CancellationToken()));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(_timeWait, stoppingToken);

            if (string.IsNullOrEmpty(_pathForTracking))
                continue;

            FetchFiles();
        }
    }

    public void ChangePathForTracking(string newPath)
    {
        /* Надо реализовать более адекватную проверку, пока что не до неё
         */
        _pathForTracking = string.IsNullOrEmpty(newPath) ? _pathForTracking : newPath;
    }

    private void FetchFiles()
    {
        var pathProjects = Directory.EnumerateFiles(_pathForTracking, "*.*", SearchOption.AllDirectories)
            .Where(file => !_blackContainerPaths.Any(file.Contains))
            .ToArray();

        _widgets = new List<CodeWidget>();

        foreach (var path in pathProjects)
        {
            if (!File.Exists(path))
                continue;

            var content = File.ReadAllText(path);

            if (!content.Contains(_tagTrackCopyable) || !content.Contains(_tagTrackNoCopyable))
                continue;

            _widgets.Add(new CodeWidget
            {
                FullPath = path, ShortPath = "",
                Code = content,
                IsCopyable = content.Contains(_tagTrackCopyable)
            });
        }
    }

    /* Получение виджета по пути */
    public CodeWidget? GetCurrentWidget(string fullPath)
    {
        return _widgets.FirstOrDefault(widget => widget.FullPath == fullPath);
    }

    /* Получение всех виджетов */
    public IEnumerable<WidgetMenu> GetWidgets()
    {
        return _widgets.Select(widget => new WidgetMenu
        {
            FullPath = widget.FullPath,
            FileName = Path.GetFileName(widget.FullPath)
        });
    }
}