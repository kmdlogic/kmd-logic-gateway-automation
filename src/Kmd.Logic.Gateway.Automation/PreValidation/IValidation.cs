using System.Threading.Tasks;
using Kmd.Logic.Gateway.Automation.PublishFile;

namespace Kmd.Logic.Gateway.Automation.PreValidation
{
    internal interface IValidation
    {
        Task<bool> ValidateAsync(PublishFileModel publishFileModel);
    }
}
