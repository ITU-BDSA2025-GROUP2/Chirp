namespace SimpleDB;

using CsvHelper;
using System.Globalization;

public sealed class CsvDatabase<T> : IDatabaseRepository<T>
{
    //private readonly string _file;
    private readonly StreamReader _reader;
    private readonly StreamWriter _writer;

    public CsvDatabase(string file)
    {
        //var _file = file;
        var stream = File.Open(file, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        _reader = new StreamReader(stream, leaveOpen: true);
        _writer = new StreamWriter(stream, leaveOpen: true);
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