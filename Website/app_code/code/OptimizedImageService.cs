using System;
using System.Collections.Generic;
using System.Threading;
using MiniBlog.Contracts;

internal class OptimizedImageService
{
    private readonly IOptimizedImageMapProvider mapProvider;
    private readonly Timer timer;
    private Dictionary<string, string> optimizedImageMap;

    public OptimizedImageService(IOptimizedImageMapProvider mapProvider)
    {
        this.mapProvider = mapProvider;
        timer = new Timer(RefreshMap, null, TimeSpan.FromSeconds(10), TimeSpan.FromHours(12));
    }

    private void RefreshMap(object state)
    {
        //Interlocked.Exchange(ref optimizedImageMap, mapProvider.GetMap());
    }

    public string FindOptimizedImagePath(string originalImagePath)
    {
        var map = Interlocked.CompareExchange(ref optimizedImageMap, null, null);
        return map != null && map.ContainsKey(originalImagePath) ? map[originalImagePath] : null;
    }
}