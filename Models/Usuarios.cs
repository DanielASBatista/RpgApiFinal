using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace RpgApi.Models
{
    public class Usuarios
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }
        public byte[]? Foto { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public DateTime? DataAcesso { get; set; }

        [NotMapped]//DataAnnotations
        public string PasswordString { get; set; } = string.Empty;

        [JsonIgnore]
        public List<Personagem>? Personagens { get; set; }
           = new List<Personagem>();

        public string? Perfil { get; set; }
        public string? Email { get; set; } = string.Empty;

        [NotMapped]
        public string Token { get; set; } = string.Empty;
    }
}