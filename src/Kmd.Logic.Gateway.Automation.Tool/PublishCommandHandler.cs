using System.Threading.Tasks;
using Serilog;

namespace Kmd.Logic.Gateway.Automation.Tool
{

    internal class PublishCommandHandler
    {
        public Task<int> Handle(PublishCommand cmd)
        {
            Log.Information("Published");
            return Task.FromResult(0);
        }
    }
}
