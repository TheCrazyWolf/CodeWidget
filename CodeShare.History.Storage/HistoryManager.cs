using CodeShare.Models.History;
using Newtonsoft.Json;

namespace CodeShare.History.Storage;

public class HistoryManager
{
    private readonly string _defaultPath = "history.json";
    private IList<HistorySession> _history = new List<HistorySession>();


    public HistoryManager()
    {
        OpenFile();
    }
    
    public IEnumerable<HistorySession> GetHistoryAsync()
    {
        return _history;
    }
    
    public void SaveHistory()
    {
        var json = JsonConvert.SerializeObject(_history);
        File.WriteAllText(_defaultPath, json);
    }

    public void OpenFile()
    {
        if (!File.Exists(_defaultPath))
        {
            SaveHistory();
            return;
        }
        
        var json = File.ReadAllText(_defaultPath);
        _history = JsonConvert.DeserializeObject<List<HistorySession>>(json) ?? new List<HistorySession>();
    }


    public void AddHistory(string path)
    {
        var item = _history.FirstOrDefault(h => h.Path == path);
        if(item is not null) return;
        _history.Add(new HistorySession() { Path = path, AddedAt = DateTime.Now });
        SaveHistory();
    }
}