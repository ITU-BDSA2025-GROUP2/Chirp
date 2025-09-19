namespace Server;

using CsvHelper;
using System.Globalization;
using System.Net.Http.Json;

public sealed class CsvDatabase<T> : IDatabaseRepository<T>
{
    private static readonly Lazy<CsvDatabase<T>> LazyInstance = new Lazy<CsvDatabase<T>>(() => new CsvDatabase<T>());
    
    //private readonly string _file;
    private readonly StreamReader _reader;
    private readonly StreamWriter _writer;
    private readonly CsvWriter _csvWriter;
    private readonly CsvReader _csvReader;



    public static CsvDatabase<T> Instance { get { return LazyInstance.Value; } }
    
    private CsvDatabase()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "data", "chirp_cli_db.csv");
        var stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        _reader = new StreamReader(stream, leaveOpen: true);
        _writer = new StreamWriter(stream, leaveOpen: true);
        _csvWriter = new CsvWriter(_writer, CultureInfo.InvariantCulture);
        _csvReader = new CsvReader(_reader, CultureInfo.InvariantCulture);
    }
    
    /*
    public static CsvDatabase<T> Instance
    {
        get
        {
            //ensures that another instance cannot be created while the process of making sure only on instance is present is still running
            //can be removed as it can affect performance (will just be less secure if we work with threads)
            lock (_padlock)
            {
                if (_instance == null)
                {
                    _instance = new CsvDatabase<T>();
                }
                Console.Out.WriteLine("INSTANCE");
                return _instance;
            }
        }
    }
    */
    
    
    
    
    
    
    public IEnumerable<T> Read(int? limit = null)
    {
        // Go to first line
        _reader.BaseStream.Seek(0, SeekOrigin.Begin);
        _reader.DiscardBufferedData();
        
        _csvReader.Context.RegisterClassMap<CsvMessageMapping>();
        
        var record = _csvReader.GetRecords<Messages>().ToList();

        return (IEnumerable<T>)record;
    }

    public void Store(T record)
    {
        Console.WriteLine(record);
        // Go to last line
        _writer.BaseStream.Seek(0, SeekOrigin.End);
        
        _csvWriter.WriteRecord(record);
        _csvWriter.NextRecord();
        _writer.Flush();
    }
}