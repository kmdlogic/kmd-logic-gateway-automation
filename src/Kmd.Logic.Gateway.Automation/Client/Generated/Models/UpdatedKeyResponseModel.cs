// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Kmd.Logic.Gateway.Automation.Client.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class UpdatedKeyResponseModel
    {
        /// <summary>
        /// Initializes a new instance of the UpdatedKeyResponseModel class.
        /// </summary>
        public UpdatedKeyResponseModel()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the UpdatedKeyResponseModel class.
        /// </summary>
        public UpdatedKeyResponseModel(System.Guid? id = default(System.Guid?), string key = default(string))
        {
            Id = id;
            Key = key;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public System.Guid? Id { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "key")]
        public string Key { get; set; }

    }
}
