using System;

namespace MiniBlog.Contracts.Framework
{
    public interface IConfiguration
    {
        string Find(string key);
    }
}