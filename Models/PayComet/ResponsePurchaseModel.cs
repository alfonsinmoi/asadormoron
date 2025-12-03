using System;
namespace AsadorMoron.Models.PayComet
{
    public class ResponsePurchaseModel
    {
        public int errorCode { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public int methodId { get; set; }
        public string order { get; set; }
        public string authCode { get; set; }
        public string challengeUrl { get; set; }
        public int idUser { get; set; }
        public string tokenUser { get; set; }
        public string cardCountry { get; set; }
    }
}
