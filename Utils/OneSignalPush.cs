using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace AsadorMoron.Utils
{
    public class OneSignalPush
    {
        private const string REST_API_KEY = "os_v2_app_aagpfu46dramtjxgk25p4czzi342yntwnutew754lzskxy3s452x3g5gagacqqrcxqwfrhlq4q5u7dvtqqjyug3q4ddvy3ce6pjxiti";
        private const string APP_ID = "000cf2d3-9e1c-40c9-a6e6-56bafe0b3946";

        public OneSignalPush()
        {
        }

        public static async Task<bool> SendPushToPlayers(string players, string mensaje)
        {
            try
            {
                var payload = new
                {
                    app_id = APP_ID,
                    contents = new { en = mensaje },
                    include_subscription_ids = players.Replace("\"", "").Split(',').Select(p => p.Trim()).ToArray()
                };

                var json = System.Text.Json.JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using var request = new HttpRequestMessage(HttpMethod.Post, "https://onesignal.com/api/v1/notifications");
                request.Headers.Authorization = new AuthenticationHeaderValue("Key", REST_API_KEY);
                request.Content = content;

                using var response = await App.Client.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"[OneSignal] Error enviando push: {response.StatusCode} - {responseContent}");
                    return false;
                }

                Debug.WriteLine($"[OneSignal] Push enviado correctamente: {responseContent}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[OneSignal] Excepción enviando push: {ex.Message}");
                return false;
            }
        }
    }
}
