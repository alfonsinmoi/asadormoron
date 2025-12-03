using System;
namespace AsadorMoron.Models.PayComet
{
    public class ResponseInfoCardModel
    {
        public string pan { get; set; }
        public string cardBrand { get; set; }
        public int errorCode { get; set; }
        public string cardType { get; set; }
        public string expiryDate { get; set; }
    }
}
