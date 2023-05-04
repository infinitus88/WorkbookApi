namespace WorkbookApi.Security
{
    public interface ITokenBuilder
    {
        string Build(string username, DateTime expireDate);
    }

}
