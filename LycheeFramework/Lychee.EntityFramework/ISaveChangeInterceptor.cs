using System.Threading.Tasks;

namespace Lychee.EntityFramework
{
    public interface ISaveChangeInterceptor
    {
        Task ExecuteBeforeAsync();

        Task ExecuteAfterAsync();
    }
}