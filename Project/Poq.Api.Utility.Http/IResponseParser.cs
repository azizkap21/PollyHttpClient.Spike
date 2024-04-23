namespace Poq.Api.Utility.Http
{
    public interface IResponseParser<out T> where T : class
    {
        T Parse(string result);
    }
}
