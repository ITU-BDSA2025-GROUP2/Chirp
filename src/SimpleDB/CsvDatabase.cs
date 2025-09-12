namespace SimpleDB;

using CsvHelper;
using System.Globalization;

public sealed class CsvDatabase<T> : IDatabaseRepository<T>
{
    //code for singleton implementation taken from: https://csharpindepth.com/Articles/Singleton
    private static CsvDatabase<T> _instance = null;
    private static readonly object _padlock = new object();
    
    //private readonly string _file;
    private static StreamReader _reader;
    private static StreamWriter _writer;


    public CsvDatabase()
    {
        var file = "chirp_cli_db.csv";
        var stream = File.Open(file, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        _reader = new StreamReader(stream, leaveOpen: true);
        _writer = new StreamWriter(stream, leaveOpen: true);
    }
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
    
    
    public IEnumerable<T> Read(int? limit = null)
    {
        // Go to first line
        _reader.BaseStream.Seek(0, SeekOrigin.Begin);
        _reader.DiscardBufferedData();
        
        using var csv = new CsvReader(_reader, CultureInfo.InvariantCulture);
        csv.Context.RegisterClassMap<CsvMessageMapping>();
        var record = csv.GetRecords<Messages>().ToList();

        return (IEnumerable<T>)record;
    }

    public void Store(T record)
    {
        // Go to last line
        _writer.BaseStream.Seek(0, SeekOrigin.End);
        using var csvWriter = new CsvWriter(_writer, CultureInfo.InvariantCulture);
        
        csvWriter.WriteRecord(record);
        csvWriter.NextRecord();
        _writer.Flush();
    }
}