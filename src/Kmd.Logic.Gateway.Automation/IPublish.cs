using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kmd.Logic.Gateway.Automation
{
    public interface IPublish
    {
        Task<IEnumerable<ValidationResult>> ProcessAsync(string path);
    }
}
