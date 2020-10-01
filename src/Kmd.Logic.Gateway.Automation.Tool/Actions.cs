using System.Threading.Tasks;
using Serilog;

namespace Kmd.Logic.Gateway.Automation.Tool
{
    internal class Actions
    {
        public Task<int> Publish()
        {
            Log.Information("Published");
            return Task.FromResult(0);
        }
    }
}
