using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RpgApi.Models
{
    public class Habilidade
    {
        public int Id { get; set; }
        public string Nome { get; set; } = String.Empty;
        public int Dano { get; set; }
        [JsonIgnore]
        public List<PersonagemHabilidade> PersonagemHabilidades {get;set;} = new();
        
    }
}