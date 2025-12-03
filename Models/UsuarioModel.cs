using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SQLite;

namespace AsadorMoron.Models
{
    public class UsuarioModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [JsonProperty("nombre")]
        public string nombre { get; set; }
        [JsonProperty("idUsuario")]
        public int idUsuario { get; set; }
        [JsonProperty("dni")]
        public string dni { get; set; }
        [JsonProperty("apellidos")]
        public string apellidos { get; set; }
        [JsonProperty("cod_postal")]
        public string codPostal { get; set; }
        [JsonProperty("poblacion")]
        public string poblacion { get; set; }
        [JsonProperty("provincia")]
        public string provincia { get; set; }
        [JsonProperty("direccion")]
        public string direccion { get; set; }
        [JsonProperty("foto")]
        public string foto { get; set; }
        [JsonProperty("fechaNacimiento")]
        public DateTime fechaNacimiento { get; set; }
        [JsonProperty("fechaAlta")]
        public DateTime fechaAlta { get; set; }
        [JsonProperty("telefono")]
        public string telefono { get; set; }
        [JsonProperty("email")]
        public string email { get; set; }
        [JsonProperty("rol")]
        public int rol { get; set; }
        [JsonProperty("demo")]
        public int demo { get; set; }
        [JsonProperty("token")]
        public string token { get; set; }
        [JsonProperty("username")]
        public string username { get; set; }
        [JsonProperty("plataforma")]
        public string platform { get; set; }
        public string password { get; set; }
        public string nombreCompleto { get; set; }
        public string pin { get; set; }
        public int idZona { get; set; }
        public string version { get; set; }
        public int idPueblo { get; set; }
        public string idSocial { get; set; }
        public string social { get; set; }
        public int versionFW { get; set; }
        [Ignore]
        public List<Establecimiento> establecimientos { get; set; }
        [Ignore]
        public List<TarjetaModel> tarjetas { get; set; }
        [Ignore]
        public RepartidorModel Repartidor { get; set; }
        [Ignore]
        public int estado { get; set; }
        public string codigo { get; set; }
        public double saldo { get; set; }
        public int bloqueado { get; set; }
        public int kiosko { get; set; }
    }
}