using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace RpgApi.Models
{
    public class PersonagemHabilidade
    {
        public int PersonagemId { get; set; }
        [JsonIgnore]
        public Personagem? Personagem { get; set; } = null!;
        public int HabilidadeId { get; set; }
        public Habilidade? Habilidade { get; set; } = null!;
    }
}