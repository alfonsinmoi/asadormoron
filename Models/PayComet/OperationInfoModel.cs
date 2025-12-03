using System;
using System.Collections.Generic;

namespace AsadorMoron.Models.PayComet
{
        public class History
        {
            public string amount { get; set; }
            public string amountDisplay { get; set; }
            public string authCode { get; set; }
            public string bicCode { get; set; }
            public string cardBrand { get; set; }
            public string cardCategory { get; set; }
            public string cardCountry { get; set; }
            public string cardType { get; set; }
            public string currency { get; set; }
            public int errorCode { get; set; }
            public string errorDescription { get; set; }
            public string feeEuro { get; set; }
            public string feePercent { get; set; }
            public string issuerBank { get; set; }
            public string methodId { get; set; }
            public int operationId { get; set; }
            public string operationName { get; set; }
            public int operationType { get; set; }
            public string order { get; set; }
            public string originalIp { get; set; }
            public string pan { get; set; }
            public string response { get; set; }
            public int secure { get; set; }
            public int state { get; set; }
            public string stateName { get; set; }
            public int terminal { get; set; }
            public string terminalName { get; set; }
            public string timestamp { get; set; }
            public string user { get; set; }
        }

        public class Pago
        {
            public string amount { get; set; }
            public string amountDisplay { get; set; }
            public string authCode { get; set; }
            public string bicCode { get; set; }
            public string cardBrand { get; set; }
            public string cardCategory { get; set; }
            public string cardCountry { get; set; }
            public string cardType { get; set; }
            public string currency { get; set; }
            public int errorCode { get; set; }
            public string errorDescription { get; set; }
            public string feeEuro { get; set; }
            public string feePercent { get; set; }
            public string issuerBank { get; set; }
            public string methodId { get; set; }
            public int operationId { get; set; }
            public string operationName { get; set; }
            public int operationType { get; set; }
            public string order { get; set; }
            public string originalIp { get; set; }
            public string pan { get; set; }
            public string response { get; set; }
            public int secure { get; set; }
            public int state { get; set; }
            public string stateName { get; set; }
            public int terminal { get; set; }
            public string terminalName { get; set; }
            public string timestamp { get; set; }
            public string user { get; set; }
            public List<History> history { get; set; }
        }

        public class OperationInfoModel
        {
            public int errorCode { get; set; }
            public Pago payment { get; set; }
        }

}

