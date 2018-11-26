namespace WebServer.Mapper
{
    public interface IMapper<T>
    {
        T Map(string[] dataset);
        bool TryMap(string[] dataset, out T result);
    }
}