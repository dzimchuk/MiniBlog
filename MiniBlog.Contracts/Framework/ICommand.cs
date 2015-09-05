using System;

namespace MiniBlog.Contracts.Framework
{
    public interface ICommand<in TModel>
    {
        void Apply(TModel model);
    }
}