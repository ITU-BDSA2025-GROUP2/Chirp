namespace Server;
using System.Net.Http.Json;


public interface IDatabaseRepository<T>
{
    /// <summary>
    /// Read the contents of the database file
    /// </summary>
    /// <param name="limit">Set how many entries will be read</param>
    /// <returns>a Generic IEnumerable for iteration</returns>
    public IEnumerable<T> Read(int? limit = null);
    /// <summary>
    /// Write the record into the database file
    /// </summary>
    /// <param name="record">generic record to be written into the database file</param>
    public void Store(T record);
}