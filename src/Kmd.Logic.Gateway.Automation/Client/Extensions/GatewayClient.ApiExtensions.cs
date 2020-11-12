namespace Kmd.Logic.Gateway.Automation.Client
{
    using System.Collections.Generic;
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1312:Variable names should begin with lower-case letter", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1101:Prefix local calls with this", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1404:Code analysis suppression should have justification", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1307:Specify StringComparison", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        private async Task<HttpOperationResponse<object>> CustomCreateApiWithHttpMessagesAsync(System.Guid subscriptionId, string name, string path, string apiVersion, Stream openApiSpec, System.Guid? apiVersionSetId = default(System.Guid?), string providerId = default(string), string visibility = default(string), string backendServiceUrl = default(string), IList<System.Guid?> productIds = default(IList<System.Guid?>), Stream logo = default(Stream), Stream documentation = default(Stream), string status = default(string), bool? isCurrent = default(bool?), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
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

            // Tracing
            bool _shouldTrace = ServiceClientTracing.IsEnabled;
            string _invocationId = null;
            if (_shouldTrace)
            {
                _invocationId = ServiceClientTracing.NextInvocationId.ToString();
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
                ServiceClientTracing.Enter(_invocationId, this, "CreateApi", tracingParameters);
            }

            // Construct URL
            var _baseUrl = BaseUri.AbsoluteUri;
            var _url = new System.Uri(new System.Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "subscriptions/{subscriptionId}/gateway/apis").ToString();
            _url = _url.Replace("{subscriptionId}", System.Uri.EscapeDataString(SafeJsonConvert.SerializeObject(subscriptionId, SerializationSettings).Trim('"')));
            List<string> _queryParameters = new List<string>();
            if (apiVersionSetId != null)
            {
                _queryParameters.Add(string.Format("apiVersionSetId={0}", System.Uri.EscapeDataString(SafeJsonConvert.SerializeObject(apiVersionSetId, SerializationSettings).Trim('"'))));
            }

            if (_queryParameters.Count > 0)
            {
                _url += "?" + string.Join("&", _queryParameters);
            }

            // Create HTTP transport objects
            var _httpRequest = new HttpRequestMessage();
            HttpResponseMessage _httpResponse = null;
            _httpRequest.Method = new HttpMethod("POST");
            _httpRequest.RequestUri = new System.Uri(_url);

            // Set Headers


            if (customHeaders != null)
            {
                foreach (var _header in customHeaders)
                {
                    if (_httpRequest.Headers.Contains(_header.Key))
                    {
                        _httpRequest.Headers.Remove(_header.Key);
                    }

                    _httpRequest.Headers.TryAddWithoutValidation(_header.Key, _header.Value);
                }
            }

            // Serialize Request
            string _requestContent = null;
            MultipartFormDataContent _multiPartContent = new MultipartFormDataContent();
            if (name != null)
            {
                StringContent _name = new StringContent(name, System.Text.Encoding.UTF8);
                _multiPartContent.Add(_name, "name");
            }

            if (providerId != null)
            {
                StringContent _providerId = new StringContent(providerId, System.Text.Encoding.UTF8);
                _multiPartContent.Add(_providerId, "providerId");
            }

            if (path != null)
            {
                StringContent _path = new StringContent(path, System.Text.Encoding.UTF8);
                _multiPartContent.Add(_path, "path");
            }

            if (visibility != null)
            {
                StringContent _visibility = new StringContent(SafeJsonConvert.SerializeObject(visibility, SerializationSettings).Trim('"'), System.Text.Encoding.UTF8);
                _multiPartContent.Add(_visibility, "visibility");
            }

            if (apiVersion != null)
            {
                StringContent _apiVersion = new StringContent(apiVersion, System.Text.Encoding.UTF8);
                _multiPartContent.Add(_apiVersion, "apiVersion");
            }

            if (openApiSpec != null)
            {
                StreamContent _openApiSpec = new StreamContent(openApiSpec);
                _openApiSpec.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                ContentDispositionHeaderValue _contentDispositionHeaderValue = new ContentDispositionHeaderValue("form-data");
                _contentDispositionHeaderValue.Name = "openApiSpec";

                // get filename from stream if it's a file otherwise, just use  'unknown'
                var _fileStream = openApiSpec as FileStream;
                var _fileName = (_fileStream != null ? _fileStream.Name : null) ?? "unknown";
                if (System.Linq.Enumerable.Any(_fileName, c => c > 127))
                {
                    // non ASCII chars detected, need UTF encoding:
                    _contentDispositionHeaderValue.FileNameStar = _fileName;
                }
                else
                {
                    // ASCII only
                    _contentDispositionHeaderValue.FileName = _fileName;
                }

                _openApiSpec.Headers.ContentDisposition = _contentDispositionHeaderValue;
                _multiPartContent.Add(_openApiSpec, "openApiSpec");
            }

            if (backendServiceUrl != null)
            {
                StringContent _backendServiceUrl = new StringContent(backendServiceUrl, System.Text.Encoding.UTF8);
                _multiPartContent.Add(_backendServiceUrl, "backendServiceUrl");
            }

            if (productIds != null)
            {
                int i = 0;
                foreach (var productId in productIds)
                {
                    StringContent _productIds = new StringContent(SafeJsonConvert.SerializeObject(productIds, SerializationSettings).Trim('"'), System.Text.Encoding.UTF8);
                    _multiPartContent.Add(new StringContent(productId.ToString()), $"productIds[{i}]");
                    i++;
                }
            }

            if (logo != null)
            {
                StreamContent _logo = new StreamContent(logo);
                _logo.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                ContentDispositionHeaderValue _contentDispositionHeaderValue = new ContentDispositionHeaderValue("form-data");
                _contentDispositionHeaderValue.Name = "logo";

                // get filename from stream if it's a file otherwise, just use  'unknown'
                var _fileStream = logo as FileStream;
                var _fileName = (_fileStream != null ? _fileStream.Name : null) ?? "unknown";
                if (System.Linq.Enumerable.Any(_fileName, c => c > 127))
                {
                    // non ASCII chars detected, need UTF encoding:
                    _contentDispositionHeaderValue.FileNameStar = _fileName;
                }
                else
                {
                    // ASCII only
                    _contentDispositionHeaderValue.FileName = _fileName;
                }

                _logo.Headers.ContentDisposition = _contentDispositionHeaderValue;
                _multiPartContent.Add(_logo, "logo");
            }

            if (documentation != null)
            {
                StreamContent _documentation = new StreamContent(documentation);
                _documentation.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                ContentDispositionHeaderValue _contentDispositionHeaderValue = new ContentDispositionHeaderValue("form-data");
                _contentDispositionHeaderValue.Name = "documentation";

                // get filename from stream if it's a file otherwise, just use  'unknown'
                var _fileStream = documentation as FileStream;
                var _fileName = (_fileStream != null ? _fileStream.Name : null) ?? "unknown";
                if (System.Linq.Enumerable.Any(_fileName, c => c > 127))
                {
                    // non ASCII chars detected, need UTF encoding:
                    _contentDispositionHeaderValue.FileNameStar = _fileName;
                }
                else
                {
                    // ASCII only
                    _contentDispositionHeaderValue.FileName = _fileName;
                }

                _documentation.Headers.ContentDisposition = _contentDispositionHeaderValue;
                _multiPartContent.Add(_documentation, "documentation");
            }

            if (status != null)
            {
                StringContent _status = new StringContent(SafeJsonConvert.SerializeObject(status, SerializationSettings).Trim('"'), System.Text.Encoding.UTF8);
                _multiPartContent.Add(_status, "status");
            }

            if (isCurrent != null)
            {
                StringContent _isCurrent = new StringContent(SafeJsonConvert.SerializeObject(isCurrent, SerializationSettings).Trim('"'), System.Text.Encoding.UTF8);
                _multiPartContent.Add(_isCurrent, "isCurrent");
            }

            _httpRequest.Content = _multiPartContent;

            // Set Credentials
            if (Credentials != null)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Credentials.ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
            }

            // Send Request
            if (_shouldTrace)
            {
                ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
            }

            cancellationToken.ThrowIfCancellationRequested();
            _httpResponse = await HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
            if (_shouldTrace)
            {
                ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
            }

            HttpStatusCode _statusCode = _httpResponse.StatusCode;
            cancellationToken.ThrowIfCancellationRequested();
            string _responseContent = null;
            if ((int)_statusCode != 201 && (int)_statusCode != 400 && (int)_statusCode != 409 && (int)_statusCode != 422)
            {
                var ex = new HttpOperationException(string.Format("Operation returned an invalid status code '{0}'", _statusCode));
                if (_httpResponse.Content != null)
                {
                    _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
                else
                {
                    _responseContent = string.Empty;
                }

                ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
                ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
                if (_shouldTrace)
                {
                    ServiceClientTracing.Error(_invocationId, ex);
                }

                _httpRequest.Dispose();
                if (_httpResponse != null)
                {
                    _httpResponse.Dispose();
                }

                throw ex;
            }

            // Create Result
            var _result = new HttpOperationResponse<object>();
            _result.Request = _httpRequest;
            _result.Response = _httpResponse;

            // Deserialize Response
            if ((int)_statusCode == 201)
            {
                _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    _result.Body = SafeJsonConvert.DeserializeObject<ApiListModel>(_responseContent, DeserializationSettings);
                }
                catch (JsonException ex)
                {
                    _httpRequest.Dispose();
                    if (_httpResponse != null)
                    {
                        _httpResponse.Dispose();
                    }

                    throw new SerializationException("Unable to deserialize the response.", _responseContent, ex);
                }
            }

            // Deserialize Response
            if ((int)_statusCode == 400)
            {
                _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    _result.Body = SafeJsonConvert.DeserializeObject<IDictionary<string, IList<string>>>(_responseContent, DeserializationSettings);
                }
                catch (JsonException ex)
                {
                    _httpRequest.Dispose();
                    if (_httpResponse != null)
                    {
                        _httpResponse.Dispose();
                    }

                    throw new SerializationException("Unable to deserialize the response.", _responseContent, ex);
                }
            }

            // Deserialize Response
            if ((int)_statusCode == 409)
            {
                _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    _result.Body = SafeJsonConvert.DeserializeObject<IDictionary<string, IList<string>>>(_responseContent, DeserializationSettings);
                }
                catch (JsonException ex)
                {
                    _httpRequest.Dispose();
                    if (_httpResponse != null)
                    {
                        _httpResponse.Dispose();
                    }

                    throw new SerializationException("Unable to deserialize the response.", _responseContent, ex);
                }
            }

            // Deserialize Response
            if ((int)_statusCode == 422)
            {
                _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    _result.Body = SafeJsonConvert.DeserializeObject<IDictionary<string, IList<string>>>(_responseContent, DeserializationSettings);
                }
                catch (JsonException ex)
                {
                    _httpRequest.Dispose();
                    if (_httpResponse != null)
                    {
                        _httpResponse.Dispose();
                    }

                    throw new SerializationException("Unable to deserialize the response.", _responseContent, ex);
                }
            }

            if (_shouldTrace)
            {
                ServiceClientTracing.Exit(_invocationId, _result);
            }

            return _result;
        }
    }
}
