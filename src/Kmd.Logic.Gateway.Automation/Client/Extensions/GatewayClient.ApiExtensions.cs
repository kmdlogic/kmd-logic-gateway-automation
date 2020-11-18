namespace Kmd.Logic.Gateway.Automation.Client
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;
    using Kmd.Logic.Gateway.Automation.Client.Models;
    using Microsoft.Rest;
    using Microsoft.Rest.Serialization;
    using Newtonsoft.Json;

    /// <summary>
    /// Custom extensions to GatewayClient.
    /// </summary>
    internal partial interface IGatewayClient
    {
        Task<object> CustomCreateApiAsync(System.Guid subscriptionId, string name, string path, string apiVersion, Stream openApiSpec, System.Guid? apiVersionSetId = default(System.Guid?), string providerId = default(string), string visibility = default(string), string backendServiceUrl = default(string), IList<System.Guid?> productIds = default(IList<System.Guid?>), Stream logo = default(Stream), Stream documentation = default(Stream), string status = default(string), bool? isCurrent = default(bool?), CancellationToken cancellationToken = default(CancellationToken));

        Task<object> CustomUpdateApiAsync(System.Guid subscriptionId, System.Guid apiId, string name = default(string), string apiVersion = default(string), string visibility = default(string), string backendServiceUrl = default(string), IList<System.Guid?> productIds = default(IList<System.Guid?>), Stream logo = default(Stream), Stream documentation = default(Stream), string status = default(string), CancellationToken cancellationToken = default(CancellationToken));
    }

    /// <summary>
    /// Custom extensions of GatewayClient.
    /// </summary>
    internal partial class GatewayClient : IGatewayClient
    {
        public async Task<object> CustomCreateApiAsync(System.Guid subscriptionId, string name, string path, string apiVersion, Stream openApiSpec, System.Guid? apiVersionSetId = default(System.Guid?), string providerId = default(string), string visibility = default(string), string backendServiceUrl = default(string), IList<System.Guid?> productIds = default(IList<System.Guid?>), Stream logo = default(Stream), Stream documentation = default(Stream), string status = default(string), bool? isCurrent = default(bool?), CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var result = await this.CustomCreateApiWithHttpMessagesAsync(subscriptionId, name, path, apiVersion, openApiSpec, apiVersionSetId, providerId, visibility, backendServiceUrl, productIds, logo, documentation, status, isCurrent, null, cancellationToken).ConfigureAwait(false))
            {
                return result.Body;
            }
        }

        public async Task<object> CustomUpdateApiAsync(System.Guid subscriptionId, System.Guid apiId, string name = default(string), string apiVersion = default(string), string visibility = default(string), string backendServiceUrl = default(string), IList<System.Guid?> productIds = default(IList<System.Guid?>), Stream logo = default(Stream), Stream documentation = default(Stream), string status = default(string), CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var result = await this.CustomUpdateApiWithHttpMessagesAsync(subscriptionId, apiId, name, apiVersion, visibility, backendServiceUrl, productIds, logo, documentation, status, null, cancellationToken).ConfigureAwait(false))
            {
                return result.Body;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Can't address. Has implications!!")]
        private async Task<HttpOperationResponse<object>> CustomCreateApiWithHttpMessagesAsync(System.Guid subscriptionId, string name, string path, string apiVersion, Stream openApiSpec, System.Guid? apiVersionSetId = default(System.Guid?), string providerId = default(string), string visibility = default(string), string backendServiceUrl = default(string), IList<System.Guid?> productIds = default(IList<System.Guid?>), Stream logo = default(Stream), Stream documentation = default(Stream), string status = default(string), bool? isCurrent = default(bool?), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            ValidateRequest(name, path, apiVersion, openApiSpec);

            // Tracing
            bool shouldTrace = ServiceClientTracing.IsEnabled;
            string invocationId = null;
            if (shouldTrace)
            {
                invocationId = ServiceClientTracing.NextInvocationId.ToString(CultureInfo.InvariantCulture);
                Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
                tracingParameters.Add("subscriptionId", subscriptionId);
                tracingParameters.Add("apiVersionSetId", apiVersionSetId);
                tracingParameters.Add("name", name);
                tracingParameters.Add("providerId", providerId);
                tracingParameters.Add("path", path);
                tracingParameters.Add("visibility", visibility);
                tracingParameters.Add("apiVersion", apiVersion);
                tracingParameters.Add("openApiSpec", openApiSpec);
                tracingParameters.Add("backendServiceUrl", backendServiceUrl);
                tracingParameters.Add("productIds", productIds);
                tracingParameters.Add("logo", logo);
                tracingParameters.Add("documentation", documentation);
                tracingParameters.Add("status", status);
                tracingParameters.Add("isCurrent", isCurrent);
                tracingParameters.Add("cancellationToken", cancellationToken);
                ServiceClientTracing.Enter(invocationId, this, "CreateApi", tracingParameters);
            }

            // Construct URL
            string url = this.ConstructUrl(subscriptionId, apiVersionSetId);

            // Create HTTP transport objects
            var httpRequest = new HttpRequestMessage();
            HttpResponseMessage httpResponse = null;
            httpRequest.Method = new HttpMethod("POST");
            httpRequest.RequestUri = new System.Uri(url);

            // Set Headers
            if (customHeaders != null)
            {
                foreach (var header in customHeaders)
                {
                    if (httpRequest.Headers.Contains(header.Key))
                    {
                        httpRequest.Headers.Remove(header.Key);
                    }

                    httpRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            // Serialize Request
            string requestContent = null;
            MultipartFormDataContent multiPartContent = new MultipartFormDataContent();
            if (name != null)
            {
                StringContent nameContent = new StringContent(name, System.Text.Encoding.UTF8);
                multiPartContent.Add(nameContent, "name");
            }

            if (providerId != null)
            {
                StringContent providerIdContent = new StringContent(providerId, System.Text.Encoding.UTF8);
                multiPartContent.Add(providerIdContent, "providerId");
            }

            if (path != null)
            {
                StringContent pathContent = new StringContent(path, System.Text.Encoding.UTF8);
                multiPartContent.Add(pathContent, "path");
            }

            if (visibility != null)
            {
                StringContent visibilityContent = new StringContent(SafeJsonConvert.SerializeObject(visibility, this.SerializationSettings).Trim('"'), System.Text.Encoding.UTF8);
                multiPartContent.Add(visibilityContent, "visibility");
            }

            if (apiVersion != null)
            {
                StringContent apiVersionContent = new StringContent(apiVersion, System.Text.Encoding.UTF8);
                multiPartContent.Add(apiVersionContent, "apiVersion");
            }

            if (openApiSpec != null)
            {
                StreamContent openApiSpecContent = new StreamContent(openApiSpec);
                openApiSpecContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                ContentDispositionHeaderValue contentDispositionHeaderValue = new ContentDispositionHeaderValue("form-data");
                contentDispositionHeaderValue.Name = "openApiSpec";

                // get filename from stream if it's a file otherwise, just use  'unknown'
                var fileStream = openApiSpec as FileStream;
                var fileName = (fileStream != null ? fileStream.Name : null) ?? "unknown";
                if (System.Linq.Enumerable.Any(fileName, c => c > 127))
                {
                    // non ASCII chars detected, need UTF encoding:
                    contentDispositionHeaderValue.FileNameStar = fileName;
                }
                else
                {
                    // ASCII only
                    contentDispositionHeaderValue.FileName = fileName;
                }

                openApiSpecContent.Headers.ContentDisposition = contentDispositionHeaderValue;
                multiPartContent.Add(openApiSpecContent, "openApiSpec");
            }

            if (backendServiceUrl != null)
            {
                StringContent backendServiceUrlContent = new StringContent(backendServiceUrl, System.Text.Encoding.UTF8);
                multiPartContent.Add(backendServiceUrlContent, "backendServiceUrl");
            }

            if (productIds != null)
            {
                int i = 0;
                foreach (var productId in productIds)
                {
                    var productIdContent = new StringContent(productId.ToString());
                    multiPartContent.Add(productIdContent, $"productIds[{i}]");
                    i++;
                }
            }

            if (logo != null)
            {
                StreamContent logoContent = new StreamContent(logo);
                logoContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                ContentDispositionHeaderValue logoContentDispositionHeaderValue = new ContentDispositionHeaderValue("form-data");
                logoContentDispositionHeaderValue.Name = "logo";

                // get filename from stream if it's a file otherwise, just use  'unknown'
                var logoFileStream = logo as FileStream;
                var logoFileName = (logoFileStream != null ? logoFileStream.Name : null) ?? "unknown";
                if (System.Linq.Enumerable.Any(logoFileName, c => c > 127))
                {
                    // non ASCII chars detected, need UTF encoding:
                    logoContentDispositionHeaderValue.FileNameStar = logoFileName;
                }
                else
                {
                    // ASCII only
                    logoContentDispositionHeaderValue.FileName = logoFileName;
                }

                logoContent.Headers.ContentDisposition = logoContentDispositionHeaderValue;
                multiPartContent.Add(logoContent, "logo");
            }

            if (documentation != null)
            {
                StreamContent documentationContent = new StreamContent(documentation);
                documentationContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                ContentDispositionHeaderValue documentationContentDispositionHeaderValue = new ContentDispositionHeaderValue("form-data");
                documentationContentDispositionHeaderValue.Name = "documentation";

                // get filename from stream if it's a file otherwise, just use  'unknown'
                var documentationfileStream = documentation as FileStream;
                var documentationfileName = (documentationfileStream != null ? documentationfileStream.Name : null) ?? "unknown";
                if (System.Linq.Enumerable.Any(documentationfileName, c => c > 127))
                {
                    // non ASCII chars detected, need UTF encoding:
                    documentationContentDispositionHeaderValue.FileNameStar = documentationfileName;
                }
                else
                {
                    // ASCII only
                    documentationContentDispositionHeaderValue.FileName = documentationfileName;
                }

                documentationContent.Headers.ContentDisposition = documentationContentDispositionHeaderValue;
                multiPartContent.Add(documentationContent, "documentation");
            }

            if (status != null)
            {
                StringContent statusContent = new StringContent(SafeJsonConvert.SerializeObject(status, this.SerializationSettings).Trim('"'), System.Text.Encoding.UTF8);
                multiPartContent.Add(statusContent, "status");
            }

            if (isCurrent != null)
            {
                StringContent isCurrentContent = new StringContent(SafeJsonConvert.SerializeObject(isCurrent, this.SerializationSettings).Trim('"'), System.Text.Encoding.UTF8);
                multiPartContent.Add(isCurrentContent, "isCurrent");
            }

            httpRequest.Content = multiPartContent;

            // Set Credentials
            if (this.Credentials != null)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await this.Credentials.ProcessHttpRequestAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            }

            // Send Request
            if (shouldTrace)
            {
                ServiceClientTracing.SendRequest(invocationId, httpRequest);
            }

            cancellationToken.ThrowIfCancellationRequested();
            httpResponse = await this.HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            if (shouldTrace)
            {
                ServiceClientTracing.ReceiveResponse(invocationId, httpResponse);
            }

            HttpStatusCode statusCode = httpResponse.StatusCode;
            cancellationToken.ThrowIfCancellationRequested();
            string responseContent = null;
            if ((int)statusCode != 201 && (int)statusCode != 400 && (int)statusCode != 409 && (int)statusCode != 422)
            {
                var ex = new HttpOperationException($"Operation returned an invalid status code '{statusCode}'");
                if (httpResponse.Content != null)
                {
                    responseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
                else
                {
                    responseContent = string.Empty;
                }

                ex.Request = new HttpRequestMessageWrapper(httpRequest, requestContent);
                ex.Response = new HttpResponseMessageWrapper(httpResponse, responseContent);
                if (shouldTrace)
                {
                    ServiceClientTracing.Error(invocationId, ex);
                }

                httpRequest.Dispose();
                if (httpResponse != null)
                {
                    httpResponse.Dispose();
                }

                throw ex;
            }

            // Create Result
            var result = new HttpOperationResponse<object>();
            result.Request = httpRequest;
            result.Response = httpResponse;

            // Deserialize Response
            if (statusCode == HttpStatusCode.Created)
            {
                responseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    result.Body = SafeJsonConvert.DeserializeObject<ApiListModel>(responseContent, this.DeserializationSettings);
                }
                catch (JsonException ex)
                {
                    httpRequest.Dispose();
                    if (httpResponse != null)
                    {
                        httpResponse.Dispose();
                    }

                    throw new SerializationException("Unable to deserialize the response.", responseContent, ex);
                }
            }

            // Deserialize Response
            if (statusCode == HttpStatusCode.BadRequest || statusCode == HttpStatusCode.Conflict || (int)statusCode == 422)
            {
                responseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    result.Body = SafeJsonConvert.DeserializeObject<IDictionary<string, IList<string>>>(responseContent, this.DeserializationSettings);
                }
                catch (JsonException ex)
                {
                    httpRequest.Dispose();
                    if (httpResponse != null)
                    {
                        httpResponse.Dispose();
                    }

                    throw new SerializationException("Unable to deserialize the response.", responseContent, ex);
                }
            }

            if (shouldTrace)
            {
                ServiceClientTracing.Exit(invocationId, result);
            }

            return result;
        }

        private string ConstructUrl(Guid subscriptionId, Guid? apiVersionSetId)
        {
            var baseUrl = this.BaseUri.AbsoluteUri;
            var url = new System.Uri(new System.Uri(baseUrl + (baseUrl.EndsWith("/", StringComparison.OrdinalIgnoreCase) ? string.Empty : "/")), "subscriptions/{subscriptionId}/gateway/apis").ToString();
            url = url.Replace("{subscriptionId}", System.Uri.EscapeDataString(SafeJsonConvert.SerializeObject(subscriptionId, this.SerializationSettings).Trim('"')));
            List<string> queryParameters = new List<string>();
            if (apiVersionSetId != null)
            {
                queryParameters.Add($"apiVersionSetId={System.Uri.EscapeDataString(SafeJsonConvert.SerializeObject(apiVersionSetId, this.SerializationSettings).Trim('"'))}");
            }

            if (queryParameters.Count > 0)
            {
                url += "?" + string.Join("&", queryParameters);
            }

            return url;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:Static elements should appear before instance elements", Justification = "Not addressing for the sake of readability")]
        private static void ValidateRequest(string name, string path, string apiVersion, Stream openApiSpec)
        {
            if (name == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "name");
            }

            if (path == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "path");
            }

            if (apiVersion == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "apiVersion");
            }

            if (openApiSpec == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "openApiSpec");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1307:Specify StringComparison", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public async Task<HttpOperationResponse<object>> CustomUpdateApiWithHttpMessagesAsync(System.Guid subscriptionId, System.Guid apiId, string name = default(string), string apiVersion = default(string), string visibility = default(string), string backendServiceUrl = default(string), IList<System.Guid?> productIds = default(IList<System.Guid?>), Stream logo = default(Stream), Stream documentation = default(Stream), string status = default(string), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Tracing
            bool shouldTrace = ServiceClientTracing.IsEnabled;
            string invocationId = null;
            if (shouldTrace)
            {
                invocationId = ServiceClientTracing.NextInvocationId.ToString(CultureInfo.InvariantCulture);
                Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
                tracingParameters.Add("subscriptionId", subscriptionId);
                tracingParameters.Add("apiId", apiId);
                tracingParameters.Add("name", name);
                tracingParameters.Add("apiVersion", apiVersion);
                tracingParameters.Add("visibility", visibility);
                tracingParameters.Add("backendServiceUrl", backendServiceUrl);
                tracingParameters.Add("productIds", productIds);
                tracingParameters.Add("logo", logo);
                tracingParameters.Add("documentation", documentation);
                tracingParameters.Add("status", status);
                tracingParameters.Add("cancellationToken", cancellationToken);
                ServiceClientTracing.Enter(invocationId, this, "UpdateApi", tracingParameters);
            }

            // Construct URL
            var baseUrl = this.BaseUri.AbsoluteUri;
            var url = new System.Uri(new System.Uri(baseUrl + (baseUrl.EndsWith("/") ? string.Empty : "/")), "subscriptions/{subscriptionId}/gateway/apis/{apiId}").ToString();
            url = url.Replace("{subscriptionId}", System.Uri.EscapeDataString(SafeJsonConvert.SerializeObject(subscriptionId, this.SerializationSettings).Trim('"')));
            url = url.Replace("{apiId}", System.Uri.EscapeDataString(SafeJsonConvert.SerializeObject(apiId, this.SerializationSettings).Trim('"')));

            // Create HTTP transport objects
            var httpRequest = new HttpRequestMessage();
            HttpResponseMessage httpResponse = null;
            httpRequest.Method = new HttpMethod("PUT");
            httpRequest.RequestUri = new System.Uri(url);

            // Set Headers
            if (customHeaders != null)
            {
                foreach (var header in customHeaders)
                {
                    if (httpRequest.Headers.Contains(header.Key))
                    {
                        httpRequest.Headers.Remove(header.Key);
                    }

                    httpRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            // Serialize Request
            string requestContent = null;
            MultipartFormDataContent multiPartContent = new MultipartFormDataContent();
            if (name != null)
            {
                StringContent nameContent = new StringContent(name, System.Text.Encoding.UTF8);
                multiPartContent.Add(nameContent, "name");
            }

            if (apiVersion != null)
            {
                StringContent apiVersionContent = new StringContent(apiVersion, System.Text.Encoding.UTF8);
                multiPartContent.Add(apiVersionContent, "apiVersion");
            }

            if (visibility != null)
            {
                StringContent visibilityContent = new StringContent(SafeJsonConvert.SerializeObject(visibility, SerializationSettings).Trim('"'), System.Text.Encoding.UTF8);
                multiPartContent.Add(visibilityContent, "visibility");
            }

            if (backendServiceUrl != null)
            {
                StringContent backendServiceUrlContent = new StringContent(backendServiceUrl, System.Text.Encoding.UTF8);
                multiPartContent.Add(backendServiceUrlContent, "backendServiceUrl");
            }

            if (productIds != null)
            {
                int i = 0;
                foreach (var productId in productIds)
                {
                    var productIdContent = new StringContent(productId.ToString());
                    multiPartContent.Add(productIdContent, $"productIds[{i}]");
                    i++;
                }
            }

            if (logo != null)
            {
                StreamContent logoContent = new StreamContent(logo);
                logoContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                ContentDispositionHeaderValue _contentDispositionHeaderValue = new ContentDispositionHeaderValue("form-data");
                _contentDispositionHeaderValue.Name = "logo";

                // get filename from stream if it's a file otherwise, just use  'unknown'
                var fileStream = logo as FileStream;
                var fileName = (fileStream != null ? fileStream.Name : null) ?? "unknown";
                if (System.Linq.Enumerable.Any(fileName, c => c > 127))
                {
                    // non ASCII chars detected, need UTF encoding:
                    _contentDispositionHeaderValue.FileNameStar = fileName;
                }
                else
                {
                    // ASCII only
                    _contentDispositionHeaderValue.FileName = fileName;
                }

                logoContent.Headers.ContentDisposition = _contentDispositionHeaderValue;
                multiPartContent.Add(logoContent, "logo");
            }

            if (documentation != null)
            {
                StreamContent documentationContent = new StreamContent(documentation);
                documentationContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                ContentDispositionHeaderValue contentDispositionHeaderValue = new ContentDispositionHeaderValue("form-data");
                contentDispositionHeaderValue.Name = "documentation";

                // get filename from stream if it's a file otherwise, just use  'unknown'
                var fileStreamContent = documentation as FileStream;
                var fileNameContent = (fileStreamContent != null ? fileStreamContent.Name : null) ?? "unknown";
                if (System.Linq.Enumerable.Any(fileNameContent, c => c > 127))
                {
                    // non ASCII chars detected, need UTF encoding:
                    contentDispositionHeaderValue.FileNameStar = fileNameContent;
                }
                else
                {
                    // ASCII only
                    contentDispositionHeaderValue.FileName = fileNameContent;
                }
                documentationContent.Headers.ContentDisposition = contentDispositionHeaderValue;
                multiPartContent.Add(documentationContent, "documentation");
            }

            if (status != null)
            {
                StringContent statusContent = new StringContent(SafeJsonConvert.SerializeObject(status, SerializationSettings).Trim('"'), System.Text.Encoding.UTF8);
                multiPartContent.Add(statusContent, "status");
            }

            httpRequest.Content = multiPartContent;

            // Set Credentials
            if (Credentials != null)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Credentials.ProcessHttpRequestAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            }
            // Send Request
            if (shouldTrace)
            {
                ServiceClientTracing.SendRequest(invocationId, httpRequest);
            }

            cancellationToken.ThrowIfCancellationRequested();
            httpResponse = await this.HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            if (shouldTrace)
            {
                ServiceClientTracing.ReceiveResponse(invocationId, httpResponse);
            }

            HttpStatusCode _statusCode = httpResponse.StatusCode;
            cancellationToken.ThrowIfCancellationRequested();
            string responseContent = null;

            if ((int)_statusCode != 200 && (int)_statusCode != 400 && (int)_statusCode != 404 && (int)_statusCode != 409 && (int)_statusCode != 422)
            {
                var ex = new HttpOperationException(string.Format("Operation returned an invalid status code '{0}'", _statusCode));
                if (httpResponse.Content != null)
                {
                    responseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
                else
                {
                    responseContent = string.Empty;
                }

                ex.Request = new HttpRequestMessageWrapper(httpRequest, requestContent);
                ex.Response = new HttpResponseMessageWrapper(httpResponse, responseContent);
                if (shouldTrace)
                {
                    ServiceClientTracing.Error(invocationId, ex);
                }

                httpRequest.Dispose();
                if (httpResponse != null)
                {
                    httpResponse.Dispose();
                }

                throw ex;
            }

            // Create Result
            var result = new HttpOperationResponse<object>();
            result.Request = httpRequest;
            result.Response = httpResponse;

            // Deserialize Response
            if ((int)_statusCode == 200)
            {
                responseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    result.Body = SafeJsonConvert.DeserializeObject<ApiListModel>(responseContent, DeserializationSettings);
                }
                catch (JsonException ex)
                {
                    httpRequest.Dispose();
                    if (httpResponse != null)
                    {
                        httpResponse.Dispose();
                    }

                    throw new SerializationException("Unable to deserialize the response.", responseContent, ex);
                }
            }

            // Deserialize Response
            if ((int)_statusCode == 400)
            {
                responseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    result.Body = SafeJsonConvert.DeserializeObject<IDictionary<string, IList<string>>>(responseContent, DeserializationSettings);
                }
                catch (JsonException ex)
                {
                    httpRequest.Dispose();
                    if (httpResponse != null)
                    {
                        httpResponse.Dispose();
                    }

                    throw new SerializationException("Unable to deserialize the response.", responseContent, ex);
                }
            }

            // Deserialize Response
            if ((int)_statusCode == 409)
            {
                responseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    result.Body = SafeJsonConvert.DeserializeObject<IDictionary<string, IList<string>>>(responseContent, DeserializationSettings);
                }
                catch (JsonException ex)
                {
                    httpRequest.Dispose();
                    if (httpResponse != null)
                    {
                        httpResponse.Dispose();
                    }

                    throw new SerializationException("Unable to deserialize the response.", responseContent, ex);
                }
            }

            // Deserialize Response
            if ((int)_statusCode == 422)
            {
                responseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    result.Body = SafeJsonConvert.DeserializeObject<IDictionary<string, IList<string>>>(responseContent, DeserializationSettings);
                }
                catch (JsonException ex)
                {
                    httpRequest.Dispose();
                    if (httpResponse != null)
                    {
                        httpResponse.Dispose();
                    }

                    throw new SerializationException("Unable to deserialize the response.", responseContent, ex);
                }
            }

            if (shouldTrace)
            {
                ServiceClientTracing.Exit(invocationId, result);
            }

            return result;
        }
    }
}
