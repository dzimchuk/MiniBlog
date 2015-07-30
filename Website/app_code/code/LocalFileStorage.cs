using System;
using System.IO;
using System.Web;
using System.Web.Hosting;
using MiniBlog.Contracts;

internal class LocalFileStorage : IFileStorage
{
    public string Save(byte[] bytes, string extension)
    {
        string relative = "~/posts/files/" + Guid.NewGuid();

        if (string.IsNullOrWhiteSpace(extension))
            extension = ".bin";
        else
            extension = "." + extension.Trim('.');

        relative += extension;

        string file = HostingEnvironment.MapPath(relative);

        File.WriteAllBytes(file, bytes);

        return VirtualPathUtility.ToAbsolute(relative);
    }
}