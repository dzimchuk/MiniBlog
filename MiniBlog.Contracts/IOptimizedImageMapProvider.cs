using System.Collections.Generic;

namespace MiniBlog.Contracts
{
    public interface IOptimizedImageMapProvider
    {
        Dictionary<string, string> GetMap();
    }
}