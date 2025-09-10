namespace SimpleDB;

using CsvHelper;
using System.Globalization;

public sealed class CsvDatabase<T> : IDatabaseRepository<T>
{
    public IEnumerable<T> Read(int? limit = null)
    {
        using var reader = new StreamReader("../SimpleDB/chirp_cli_db.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        csv.Context.RegisterClassMap<CsvMessageMapping>();
        var record = csv.GetRecords<Messages>().ToList();

        return (IEnumerable<T>)record;
    }

    public void Store(T record)
    {

        using var writer = new StreamWriter("../SimpleDB/chirp_cli_db.csv", true);
        using var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);

        csvWriter.WriteRecord(record);
        csvWriter.NextRecord();
    }
}