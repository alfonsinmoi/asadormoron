using System.IO;
using System.Net;
using System.Text;

namespace AsadorMoron.Utils
{
    public class OneSignalPush
    {
        public OneSignalPush()
        {
        }
        public static bool SendPushToPlayers(string players, string mensaje)
        {
            var request = WebRequest.Create("https://onesignal.com/api/v1/notifications") as HttpWebRequest;

            request.KeepAlive = true;
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";

            byte[] byteArray = Encoding.UTF8.GetBytes("{"
                                                    + "\"app_id\": \"37b05e73-3b40-4510-a828-9d359a80fcf6\","
                                                    + "\"contents\": {\"en\": \"" + mensaje + "\"},"
                                                    + "\"include_player_ids\": [" + players + "]}");
            //string p = "\"6392d91a-b206-4b7b-a620-cd68e32c3a76\",\"76ece62b-bcfe-468c-8a78-839aeaa8c5fa\",\"8e0f21fa-9a5a-4ae7-a9a6-ca1f24294b86\"";
            string responseContent = null;

            try
            {
                using (var writer = request.GetRequestStream())
                {
                    writer.Write(byteArray, 0, byteArray.Length);
                }

                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseContent = reader.ReadToEnd();
                    }
                }

            }
            catch (WebException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(new StreamReader(ex.Response.GetResponseStream()).ReadToEnd());
                return false;
            }

            System.Diagnostics.Debug.WriteLine(responseContent);
            return true;
        }
    }
}
