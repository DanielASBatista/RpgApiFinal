using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace RpgApi.Models
{
    public class Arma
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int Dano { get; set; }
        [JsonIgnore]
        public Personagem? Personagem{ get; set;} = null!;
        public int? PersonagemId { get; set; }
    }
}