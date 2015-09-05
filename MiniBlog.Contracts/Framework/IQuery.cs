using System;

namespace MiniBlog.Contracts.Framework
{
    public interface IQuery<in TModel, out TResult>
    {
        TResult Execute(TModel model);
    }
}