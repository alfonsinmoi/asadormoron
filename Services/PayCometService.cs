using System;
using System.Diagnostics;

namespace AsadorMoron.Services
{
    public class PayCometService
    {
        public static void addCard()
        {
            try
            {
                if (App.DAUtil.DoIHaveInternet())
                {
                    //var client = new RestClient("https://rest.paycomet.com/v1/cards");
                    //client.Timeout = -1;
                    //var request = new RestRequest(Method.POST);
                    //request.AddHeader("PAYCOMET-API-TOKEN", "bd11bf510b5e7ec9ac5c172989401cf81bee66f1");
                    //request.AddParameter("text/plain", "{\n    \"terminal\": -73633929,\n    \"cvc2\": \"012\",\n    \"jetToken\": \"rApGWqLdH8ExOyNiLukyMMs78PZRzDm5\",\n    \"expiryYear\": \"23\",\n    \"expiryMonth\": \"11\",\n    \"pan\": \"Lorem laborum elit si\",\n    \"order\": \"PAY987654321\",\n    \"productDescription\": \"Random product\",\n    \"language\": \"fr\",\n    \"notify\": 1\n}", ParameterType.RequestBody);
                    //IRestResponse response = client.Execute(request);
                    //Console.WriteLine(response.Content);

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error getTokens: " + ex.ToString());
            }
        }
    }
}
