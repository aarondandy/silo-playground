using System.Threading.Tasks;
using Orleans;

namespace GrainInterfaces
{
    public interface ICorn : IGrainWithIntegerKey
    {
        Task BeginPop();
    }
}
