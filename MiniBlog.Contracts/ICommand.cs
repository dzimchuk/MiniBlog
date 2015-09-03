namespace MiniBlog.Contracts
{
    public interface ICommand<in TModel>
    {
        void Apply(TModel model);
    }
}