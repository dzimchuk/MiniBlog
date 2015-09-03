using System.Threading.Tasks;

namespace MiniBlog.PostSync
{
    public interface IChangeNotifier
    {
        void TrachChange(string fileName);
        Task NotifyAsync();
    }
}