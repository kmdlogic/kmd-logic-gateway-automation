// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Kmd.Logic.Gateway.Automation.Client.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class GetApiSetsResponse
    {
        /// <summary>
        /// Initializes a new instance of the GetApiSetsResponse class.
        /// </summary>
        public GetApiSetsResponse()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the GetApiSetsResponse class.
        /// </summary>
        /// <param name="publishStatus">Possible values include: 'UnPublished',
        /// 'Requested', 'Published'</param>
        public GetApiSetsResponse(string publishStatus = default(string), System.Guid? versionSetId = default(System.Guid?), string path = default(string), System.Guid? providerId = default(System.Guid?), ApiVersionResponse currentVersion = default(ApiVersionResponse))
        {
            PublishStatus = publishStatus;
            VersionSetId = versionSetId;
            Path = path;
            ProviderId = providerId;
            CurrentVersion = currentVersion;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets possible values include: 'UnPublished', 'Requested',
        /// 'Published'
        /// </summary>
        [JsonProperty(PropertyName = "publishStatus")]
        public string PublishStatus { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "versionSetId")]
        public System.Guid? VersionSetId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "providerId")]
        public System.Guid? ProviderId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "currentVersion")]
        public ApiVersionResponse CurrentVersion { get; set; }

    }
}