using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Kmd.Logic.Gateway.Automation.Tool.OutputFormatters
{
    public class ValidationResultContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (property.DeclaringType == typeof(ValidationResult))
            {
                if (property.PropertyName == "Errors")
                {
                    property.ShouldSerialize = instance =>
                    {
                        ValidationResult result = (ValidationResult)instance;
                        return result.IsError;
                    };
                }

                if (property.PropertyName == "ValidatePublishingResult")
                {
                    property.ShouldSerialize = instance =>
                    {
                        ValidationResult result = (ValidationResult)instance;
                        return !result.IsError;
                    };
                }
            }

            return property;
        }
    }
}
