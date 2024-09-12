using System.Security.Cryptography;
using System.Text;
using CodeVijetWeb.Models;

namespace CodeVijetWeb.Services;

/*
 * Сервис отслеживающий файлы для
 * отображения на Виджете
 * by TheCrazyWolf
 */
public class WatchDogService : BackgroundService
{
    /// <summary>
    /// Текущий путь для отслеживания
    /// </summary>
    private string _pathForTracking = "";

    /// <summary>
    /// Ожидание задержки
    /// для прошаривание файлов и обновление страниц с кодом
    /// </summary>
    private readonly int _timeWait;

    /// <summary>
    /// Черный список папок для игнора
    /// </summary>
    private readonly IEnumerable<string> _blackContainerPaths;

    /// <summary>
    /// Черный список расширений файлов для игнора
    /// </summary>
    private readonly IEnumerable<string> _blackContainerExtensions;

    /// <summary>
    /// Доступные виджеты с файлами для просмотра
    /// </summary>
    private IList<ListingCode> _widgets = new List<ListingCode>();

    /* Теги для отслеживания файлов, которые можно прочитать */
    private readonly string _tagTrackCopyable;
    private readonly string _tagTrackNoCopyable;

    /// <summary>
    /// Внедрение DJ зависимостей и получение доступа
    /// к конфигурационным файлам
    /// </summary>
    private readonly IConfiguration _configuration;

    public WatchDogService(IConfiguration configuration)
    {
        _configuration = configuration;

        /* Грузим теги из конфиг файла, если там пусто - ставим значение по умолчанию */
        _tagTrackCopyable = _configuration.GetValue<string>("tagForTrack") ?? "// track";
        _tagTrackNoCopyable = _configuration.GetValue<string>("tagForTrackAndNoCopy") ?? "// nocopy";
        _timeWait = _configuration.GetValue<int>("TimerForFetchFiles");
        _blackContainerExtensions = _configuration.GetSection("BlackContainerExtensions").Get<List<string>>() ??
                                    new List<string>();
        _blackContainerPaths =
            _configuration.GetSection("BlackContainerPaths").Get<List<string>>() ?? new List<string>();
    
        // Запускаю сервис
        // Мне кажется на этот моменте я что то делаю не так???
        Task.Run(() => ExecuteAsync(new CancellationToken()));
    }

    /// <summary>
    /// Фоновая служба, которая будет работать и работать
    /// </summary>
    /// <param name="stoppingToken"></param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(_timeWait, stoppingToken);

            if (string.IsNullOrEmpty(_pathForTracking))
            {
                _widgets = new List<ListingCode>();
                continue;
            }

            _widgets = FetchFiles();
        }
    }

    /// <summary>
    /// Смена пути файла для отслеживания
    /// </summary>
    /// <param name="newPath"></param>
    public void ChangePathForTrackingSafely(string newPath)
    {
        _pathForTracking = string.IsNullOrEmpty(newPath) ? _pathForTracking : newPath;
    }
    
    public void ChangePathForTrackingUnSafely(string newPath)
    {
        _pathForTracking = newPath;
    }

    /// <summary>
    /// Проверка файлов и чтение
    /// </summary>
    private IList<ListingCode> FetchFiles()
    {
        var pathProjects = Directory.EnumerateFiles(_pathForTracking, "*.*", SearchOption.AllDirectories)
            .Where(file => !_blackContainerPaths.Any(file.Contains) &&
                           !_blackContainerExtensions.Any(ext =>
                               file.EndsWith("." + ext, StringComparison.OrdinalIgnoreCase)))
            .ToArray();


        foreach (var path in pathProjects)
        {
            if (!File.Exists(path)) continue;

            string content = TryReadFileContent(path);

            // проверка на тегов
            if (!(content.Contains(_tagTrackCopyable) || content.Contains(_tagTrackNoCopyable))) continue;
            
            // Если такой уже виджет в коллекции
            if (_widgets.FirstOrDefault(x => x.FullPath == path) != null) continue;

            // генерация нового виджета
            var newWidget = new ListingCode
            {
                FullPath = path,
                ShortPath = $"{path.Split(Path.DirectorySeparatorChar)
                    .Reverse()
                    .Skip(1)
                    .First()}{Path.DirectorySeparatorChar}{Path.GetFileName(path)}",
                /* Помещаем туда код и удаляем теги для вида. Зачем? да просто*/
                Code = content.Replace(_tagTrackCopyable, string.Empty)
                    .Replace(_tagTrackNoCopyable, string.Empty),
                FileName = Path.GetFileName(path),
                IsCopyable = content.Contains(_tagTrackCopyable),
                IdListingCode = GetUniqIdentity(path),
            };

            newWidget.History.Add(new HistoryCode(1,
                TimeOnly.FromDateTime(DateTime.Now), newWidget.Code));
            
            // добавление
            _widgets.Add(newWidget);
        }


        // Перебираем существующие виджеты для обновления
        foreach (var item in _widgets.ToList())
        {
            // после удаления файла, чтобы виджет пропадал
            if (!File.Exists(item.FullPath))
            {
                _widgets.Remove(item); continue;
            }
            
            string content = TryReadFileContent(item.FullPath);
            
            if (!(content.Contains(_tagTrackCopyable) || content.Contains(_tagTrackNoCopyable)))
            {
                // удаляем если больше не трекаем
                _widgets.Remove(item); continue;
            }

            // дергаем прошлую историю кода
            var lastHistory = item.History.LastOrDefault();

            // обновляем кол
            item.Code = content.Replace(_tagTrackCopyable, string.Empty)
                .Replace(_tagTrackNoCopyable, string.Empty);
            item.IsCopyable = content.Contains(_tagTrackCopyable);

            // изменилась ли длина кода по сравнению с предыдущим
            if (lastHistory?.Code.Length == item.Code.Length) continue;
            
            // новый id для истории
            var newId = lastHistory is null ? 1 : lastHistory.Id + 1;
            
            // если изменилась, добавляем историю
            item.History.Add(new HistoryCode(newId, TimeOnly.FromDateTime(DateTime.Now), item.Code));
        }

        return _widgets;
    }

    /// <summary>
    /// Считываем содежимое файлов
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private string TryReadFileContent(string path)
    {
        string content = string.Empty;
        
        try
        {
            content = File.ReadAllText(path);
        }
        catch 
        {
            // ignored
        }

        return content;
    }

    /// <summary>
    /// Получение листинга
    /// </summary>
    /// <param name="idListing">Полный путь к файлу</param>
    /// <returns></returns>
    public ListingCode? GetListingCode(string idListing)
        => _widgets.FirstOrDefault(widget => widget.IdListingCode == idListing);

    /// <summary>
    /// Получение файлов доступных для просмотра
    /// </summary>
    /// <returns></returns>
    public IEnumerable<WidgetMenu> GetWidgets()
    {
        return _widgets.Select(widget => new WidgetMenu
        {
            FullPath = widget.FullPath,
            FileName = widget.FileName,
            ShortPath = widget.ShortPath,
            IdListingCode = widget.IdListingCode
        });
    }

    /// <summary>
    /// Получение время задержки обновления файлов
    /// </summary>
    /// <returns></returns>
    public int GetTimeWaitFromConfig()
        => _timeWait;

    /// <summary>
    /// Генерация ID на основе строки с полным путем
    /// </summary>
    /// <returns></returns>
    private string GetUniqIdentity(string fullPath)
    {
        var inputBytes = Encoding.ASCII.GetBytes(fullPath);
        var hashBytes = MD5.HashData(inputBytes);
        return Convert.ToHexString(hashBytes).ToLower(); // .NET 5 +
    }

    /// <summary>
    /// Возращает текущую директорию для отслеживания
    /// </summary>
    /// <returns></returns>
    public string GetCurrentTrackingPath()
        => _pathForTracking;
}