using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Kmd.Logic.Gateway.Automation.Tool.OutputFormatters
{
    internal static class ValidationResultExtension
    {
        public static JObject ToJson(this ValidationResult validationResult)
        {
            dynamic result = new JObject();
            result.Status = !validationResult.IsError ? "Passed" : "Failed";
            if (validationResult.Errors != null && validationResult.Errors.Any())
            {
                result.Errors = validationResult.Errors.ToJson();
            }

            if (validationResult.ValidatePublishingResult != null)
            {
                dynamic validatePublishingResultJson = new JObject();

                if (validationResult.ValidatePublishingResult.Errors != null && validationResult.ValidatePublishingResult.Errors.Any())
                {
                    validatePublishingResultJson.Errors = validationResult.ValidatePublishingResult.Errors.ToJson();
                }

                if (validationResult.ValidatePublishingResult.Products != null && validationResult.ValidatePublishingResult.Products.Any())
                {
                    validatePublishingResultJson.Products = validationResult.ValidatePublishingResult.Products.ToJson();
                }

                if (validationResult.ValidatePublishingResult.Apis != null && validationResult.ValidatePublishingResult.Apis.Any())
                {
                    validatePublishingResultJson.Apis = validationResult.ValidatePublishingResult.Apis.ToJson();
                }

                if (((JObject)validatePublishingResultJson).HasValues)
                {
                    result.ValidatePublishingResult = validatePublishingResultJson;
                }
            }

            return result;
        }

        private static JObject ToJson(this ItemValidationResultBase itemValidationResultBase)
        {
            dynamic result = new JObject();
            if (itemValidationResultBase != null)
            {
                if (itemValidationResultBase.Errors != null && itemValidationResultBase.Errors.Any())
                {
                    var errorsJsonArray = new JArray();
                    foreach (var error in itemValidationResultBase.Errors)
                    {
                        errorsJsonArray.Add(error);
                    }

                    result.Errors = errorsJsonArray;
                }
                else
                {
                    if (itemValidationResultBase.EntityId.HasValue)
                    {
                        result.Id = itemValidationResultBase.EntityId.Value;
                    }

                    result.Status = Enum.GetName(typeof(ValidationStatus), itemValidationResultBase.Status);
                }
            }

            return result;
        }

        private static JArray ToJson<T>(this IEnumerable<T> results)
        {
            var errorsJsonArray = new JArray();
            if (results != null)
            {
                foreach (var error in results)
                {
                    errorsJsonArray.Add(error.ToString());
                }
            }

            return errorsJsonArray;
        }

        private static JArray ToJson(this IEnumerable<ProductValidationResult> productValidationResults)
        {
            var productsJsonArray = new JArray();
            if (productValidationResults != null)
            {
                foreach (var product in productValidationResults)
                {
                    dynamic productJson = new JObject();
                    productJson.Name = product.Name;
                    productJson.Merge(product.ToJson());

                    if (product.Policies != null)
                    {
                        var productPoliciesJson = product.Policies.ToJson();

                        if (productPoliciesJson.HasValues)
                        {
                            productJson.Policies = productPoliciesJson;
                        }
                    }

                    productsJsonArray.Add(productJson);
                }
            }

            return productsJsonArray;
        }

        private static JArray ToJson(this IEnumerable<ApiValidationResult> apiValidationResults)
        {
            var apisJsonArray = new JArray();

            if (apiValidationResults != null)
            {
                foreach (var api in apiValidationResults)
                {
                    dynamic apiJson = new JObject();
                    apiJson.Name = api.Name;
                    apiJson.Path = api.Path;
                    apiJson.Version = api.Version;
                    if (api.ApiVersionSetId.HasValue)
                    {
                        apiJson.ApiVersionSetId = api.ApiVersionSetId.Value;
                    }

                    apiJson.Merge(api.ToJson());

                    if (api.Revisions != null && api.Revisions.Any())
                    {
                        var revisionsJsonArray = new JArray();
                        foreach (var revision in api.Revisions)
                        {
                            revisionsJsonArray.Add(revision.ToJson());
                        }

                        apiJson.Revisions = revisionsJsonArray;
                    }

                    if (api.Policies != null)
                    {
                        var apiPoliciesJson = api.Policies.ToJson();

                        if (apiPoliciesJson.HasValues)
                        {
                            apiJson.Policies = apiPoliciesJson;
                        }
                    }

                    apisJsonArray.Add(apiJson);
                }
            }

            return apisJsonArray;
        }

        private static JObject ToJson(this PoliciesValidationResult policies)
        {
            dynamic policiesJson = new JObject();
            if (policies != null)
            {
                if (policies.RateLimitPolicy != null)
                {
                    policiesJson.RateLimitPolicy = policies.RateLimitPolicy.ToJson();
                }

                if (policies.CustomPolicies != null && policies.CustomPolicies.Any())
                {
                    var customPoliciesJsonArray = new JArray();

                    foreach (var customPolicy in policies.CustomPolicies)
                    {
                        customPoliciesJsonArray.Add(customPolicy.ToJson());
                    }

                    policiesJson.CustomPolicies = customPoliciesJsonArray;
                }
            }

            return policiesJson;
        }
    }
}
