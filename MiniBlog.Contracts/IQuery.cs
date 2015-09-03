namespace MiniBlog.Contracts
{
    public interface IQuery<in TModel, out TResult>
    {
        TResult Execute(TModel model);
    }
}