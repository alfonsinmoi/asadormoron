using System;
using SQLite;

namespace AsadorMoron.Models.PayComet
{
    public class CodeErrors
    {
        [PrimaryKey,AutoIncrement]
        public int id { get; set; }
        public int errorCode { get; set; }
        public string textError { get; set; }
    }
}
