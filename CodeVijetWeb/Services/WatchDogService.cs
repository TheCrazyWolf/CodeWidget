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
    /// TODO: необходимо вынести в конфиг файл
    /// </summary>
    private readonly IEnumerable<string> _blackContainerPaths;

    /// <summary>
    /// Черный список расширений файлов для игнора
    /// TODO: необходимо вынести в конфиг файл
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
        _blackContainerExtensions = _configuration.GetSection("BlackContainerExtensions").Get<List<string>>() ?? new List<string>();
        _blackContainerPaths = _configuration.GetSection("BlackContainerPaths").Get<List<string>>() ?? new List<string>();

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
                continue;

            _widgets = FetchFiles();
        }
    }

    /// <summary>
    /// Смена пути файла для отслеживания
    /// </summary>
    /// <param name="newPath"></param>
    public void ChangePathForTracking(string newPath)
    {
        /* Надо реализовать более адекватную проверку, пока что не до неё
         */
        _pathForTracking = string.IsNullOrEmpty(newPath) ? _pathForTracking : newPath;
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
        
        var widgets = new List<ListingCode>();

        foreach (var path in pathProjects)
        {
            if (!File.Exists(path))
                continue;

            string? content;

            try
            {
                content = File.ReadAllText(path);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                continue;
            }

            if (!(content.Contains(_tagTrackCopyable) || content.Contains(_tagTrackNoCopyable)))
                continue;

            widgets.Add(new ListingCode
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
                IdListingCode = GetUniqIdentity(path)
            });
        }

        return widgets;
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
        using var md5 = MD5.Create();
        var inputBytes = Encoding.ASCII.GetBytes(fullPath);
        var hashBytes = md5.ComputeHash(inputBytes);
        return Convert.ToHexString(hashBytes).ToLower(); // .NET 5 +
    }
}