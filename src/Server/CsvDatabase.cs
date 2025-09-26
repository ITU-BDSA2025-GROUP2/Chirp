namespace Server;

using CsvHelper;
using System.Globalization;
using System.Net.Http.Json;

public sealed class CsvDatabase<T> : IDatabaseRepository<T>
{
    private static Lazy<CsvDatabase<T>> LazyInstance = new Lazy<CsvDatabase<T>>(() => new CsvDatabase<T>());
    
    //private readonly string _file;
    private StreamReader _reader;
    private StreamWriter _writer;
    private CsvWriter _csvWriter;
    private CsvReader _csvReader;



    public static CsvDatabase<T> Instance { get { return LazyInstance.Value; } }
    
    private CsvDatabase()
    {
        
        
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
        var filePath = Path.Combine(AppContext.BaseDirectory, "data", "chirp_cli_db.csv");
        _reader = new StreamReader(filePath);
        _csvReader = new CsvReader(_reader, CultureInfo.InvariantCulture);
        // Go to first line
        _reader.BaseStream.Seek(0, SeekOrigin.Begin);
        _reader.DiscardBufferedData();
        
        _csvReader.Context.RegisterClassMap<CsvMessageMapping>();
        
        var record = _csvReader.GetRecords<Messages>().ToList();

        return (IEnumerable<T>)record;
    }

    public void Store(T record)
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "data", "chirp_cli_db.csv");
        _writer = new StreamWriter(filePath, true);
        _csvWriter = new CsvWriter(_writer, CultureInfo.InvariantCulture);

        Console.WriteLine(record);
        // Go to last line
        _writer.BaseStream.Seek(0, SeekOrigin.End);
        
        _csvWriter.WriteRecord(record);
        _csvWriter.NextRecord();
        _writer.Flush();
    }
}