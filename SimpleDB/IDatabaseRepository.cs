namespace SimpleDB;

public interface IDatabaseRepository<T>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="limit"></param>
    /// <returns></returns>
    public IEnumerable<T> Read(int? limit = null);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="record"></param>
    public void Store(T record);
}