namespace WebServer.Mapper
{
    public interface IMapper<T>
    {
        T Map(string[] dataSet);
        bool TryMap(string[] dataSet, out T result);
    }
}