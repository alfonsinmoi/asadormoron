using System;
namespace AsadorMoron.Models.PayComet
{
    public class Payment
    {
        public string amount { get; set; }
        public string currency { get; set; }
        public int idUser { get; set; }
        public int methodId { get; set; }
        public string order { get; set; }
        public string originalIp { get; set; }
        public int secure { get; set; }
        public int terminal { get; set; }
        public string tokenUser { get; set; }
        public string authCode { get; set; }
    }
}
