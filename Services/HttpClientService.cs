using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Diagnostics;

namespace AsadorMoron.Services
{
    /// <summary>
    /// Servicio HTTP optimizado con singleton HttpClient y métodos async.
    /// Reemplaza las llamadas bloqueantes .Result por async/await.
    /// </summary>
    public sealed class HttpClientService : IDisposable
    {
        private static readonly Lazy<HttpClientService> _instance =
            new Lazy<HttpClientService>(() => new HttpClientService());

        public static HttpClientService Instance => _instance.Value;

        private readonly HttpClient _httpClient;
        private readonly JsonSerializerSettings _jsonSettings;
        private bool _disposed;

        private HttpClientService()
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate,
                MaxConnectionsPerServer = 10,
                // Configuración SSL/TLS para compatibilidad con servidores
                SslProtocols = System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls13,
                // En desarrollo, puede ser necesario aceptar certificados no verificados
                // IMPORTANTE: Solo para desarrollo, no usar en producción
#if DEBUG
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                {
                    // Log del error SSL para diagnóstico
                    if (errors != System.Net.Security.SslPolicyErrors.None)
                    {
                        Debug.WriteLine($"[SSL] Certificate error: {errors}");
                        Debug.WriteLine($"[SSL] Certificate subject: {cert?.Subject}");
                        Debug.WriteLine($"[SSL] Certificate issuer: {cert?.Issuer}");
                    }
                    // En DEBUG aceptamos todos los certificados para testing
                    return true;
                }
#endif
            };

            _httpClient = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };

            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.AcceptEncoding.Add(
                new StringWithQualityHeaderValue("gzip"));

            _jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DateFormatHandling = DateFormatHandling.IsoDateFormat
            };
        }

        /// <summary>
        /// GET async con deserialización automática
        /// </summary>
        public async Task<T> GetAsync<T>(string url, CancellationToken cancellationToken = default) where T : class, new()
        {
            try
            {
                using var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"[HTTP] GET Error {response.StatusCode}: {url}");
                    return new T();
                }

                var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

                if (string.IsNullOrEmpty(json) || json.ToLower().Equals("false"))
                    return new T();

                return JsonConvert.DeserializeObject<T>(json, _jsonSettings) ?? new T();
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine($"[HTTP] GET Cancelled: {url}");
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[HTTP] GET Exception: {ex.Message} - {url}");
                return new T();
            }
        }

        /// <summary>
        /// GET async que devuelve el JSON raw
        /// </summary>
        public async Task<string> GetStringAsync(string url, CancellationToken cancellationToken = default)
        {
            try
            {
                using var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"[HTTP] GET String Error {response.StatusCode}: {url}");
                    return string.Empty;
                }

                return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine($"[HTTP] GET String Cancelled: {url}");
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[HTTP] GET String Exception: {ex.Message} - {url}");
                return string.Empty;
            }
        }

        /// <summary>
        /// POST async con serialización/deserialización automática
        /// </summary>
        public async Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest data, CancellationToken cancellationToken = default)
            where TResponse : class, new()
        {
            try
            {
                var json = JsonConvert.SerializeObject(data, _jsonSettings);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");
                using var response = await _httpClient.PostAsync(url, content, cancellationToken).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"[HTTP] POST Error {response.StatusCode}: {url}");
                    return new TResponse();
                }

                var responseJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

                if (string.IsNullOrEmpty(responseJson) || responseJson.ToLower().Equals("false"))
                    return new TResponse();

                return JsonConvert.DeserializeObject<TResponse>(responseJson, _jsonSettings) ?? new TResponse();
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine($"[HTTP] POST Cancelled: {url}");
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[HTTP] POST Exception: {ex.Message} - {url}");
                return new TResponse();
            }
        }

        /// <summary>
        /// POST async que devuelve bool indicando éxito
        /// </summary>
        public async Task<bool> PostAsync<TRequest>(string url, TRequest data, CancellationToken cancellationToken = default)
        {
            try
            {
                var json = JsonConvert.SerializeObject(data, _jsonSettings);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");
                using var response = await _httpClient.PostAsync(url, content, cancellationToken).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"[HTTP] POST Bool Error {response.StatusCode}: {url}");
                    return false;
                }

                var responseJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                return !string.IsNullOrEmpty(responseJson) && !responseJson.ToLower().Equals("false");
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine($"[HTTP] POST Bool Cancelled: {url}");
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[HTTP] POST Bool Exception: {ex.Message} - {url}");
                return false;
            }
        }

        /// <summary>
        /// PUT async con serialización/deserialización automática
        /// </summary>
        public async Task<TResponse> PutAsync<TRequest, TResponse>(string url, TRequest data, CancellationToken cancellationToken = default)
            where TResponse : class, new()
        {
            try
            {
                var json = JsonConvert.SerializeObject(data, _jsonSettings);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");
                using var response = await _httpClient.PutAsync(url, content, cancellationToken).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"[HTTP] PUT Error {response.StatusCode}: {url}");
                    return new TResponse();
                }

                var responseJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

                if (string.IsNullOrEmpty(responseJson) || responseJson.ToLower().Equals("false"))
                    return new TResponse();

                return JsonConvert.DeserializeObject<TResponse>(responseJson, _jsonSettings) ?? new TResponse();
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine($"[HTTP] PUT Cancelled: {url}");
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[HTTP] PUT Exception: {ex.Message} - {url}");
                return new TResponse();
            }
        }

        /// <summary>
        /// PUT async que devuelve bool indicando éxito
        /// </summary>
        public async Task<bool> PutAsync<TRequest>(string url, TRequest data, CancellationToken cancellationToken = default)
        {
            try
            {
                var json = JsonConvert.SerializeObject(data, _jsonSettings);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");
                using var response = await _httpClient.PutAsync(url, content, cancellationToken).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"[HTTP] PUT Bool Error {response.StatusCode}: {url}");
                    return false;
                }

                var responseJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                return !string.IsNullOrEmpty(responseJson) && !responseJson.ToLower().Equals("false");
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine($"[HTTP] PUT Bool Cancelled: {url}");
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[HTTP] PUT Bool Exception: {ex.Message} - {url}");
                return false;
            }
        }

        /// <summary>
        /// DELETE async
        /// </summary>
        public async Task<bool> DeleteAsync(string url, CancellationToken cancellationToken = default)
        {
            try
            {
                using var response = await _httpClient.DeleteAsync(url, cancellationToken).ConfigureAwait(false);
                return response.IsSuccessStatusCode;
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine($"[HTTP] DELETE Cancelled: {url}");
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[HTTP] DELETE Exception: {ex.Message} - {url}");
                return false;
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _httpClient?.Dispose();
            _disposed = true;
        }
    }
}
