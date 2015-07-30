namespace MiniBlog.Contracts
{
    public interface IFileStorage
    {
        string Save(byte[] bytes, string extension);
    }
}