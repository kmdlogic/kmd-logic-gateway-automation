using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kmd.Logic.Gateway.Automation
{
    public interface IPublish
    {
        Task<IList<PublishResult>> Process(string path);
    }
}
