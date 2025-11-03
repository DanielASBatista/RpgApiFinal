using RpgApi.Models;
using RpgApi.Data;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace RpgApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DisputasController : ControllerBase
    {
        private readonly DataContext _context;

        public DisputasController(DataContext context)
        {
            _context = context;
        }
        [HttpPost("Arma")]

        public async Task<IActionResult> AtaqueComArmaAsync(Disputa d)
        {
            try
            {
                Personagem? atacante = await _context.TB_PERSONAGENS
                    .Include(p => p.Arma)
                    .FirstOrDefaultAsync(p => p.Id == d.AtacanteId);
                Personagem? oponente = await _context.TB_PERSONAGENS
                    .FirstOrDefaultAsync(p => p.Id == d.OponenteId);

                int dano = atacante.Arma.Dano + (new Random().Next(atacante.Forca));

                dano = dano - new Random().Next(oponente.Defesa);

                if (dano > 0)
                    oponente.PontosVida = oponente.PontosVida - (int)dano;
                if (oponente.PontosVida <= 0)
                    d.Narracao = $"{oponente.Nome} foi derrotado!";
                _context.TB_PERSONAGENS.Update(oponente);
                await _context.SaveChangesAsync();

                StringBuilder dados = new StringBuilder();
                dados.AppendFormat("Atacante: {0}. ", atacante.Nome);
                dados.AppendFormat("Oponente: {0}. ", oponente.Nome);
                dados.AppendFormat("Pontos de vida do atacante: {0}. ", atacante.PontosVida);
                dados.AppendFormat("Pontos de vida do oponente: {0}. ", oponente.PontosVida);
                dados.AppendFormat("Arma utilizada: {0}. ", atacante.Arma.Nome);
                dados.AppendFormat("Dano: {0}. ", dano);

                d.Narracao += dados.ToString();
                d.DataDisputa = DateTime.Now;
                _context.TB_DISPUTAS.Add(d);
                _context.SaveChanges();
                return Ok(d);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("Habilidade")]

        public async Task<IActionResult> AtaqueComHabilidadeAsync(Disputa d)
        {
            try
            {
                Personagem atacante = await _context.TB_PERSONAGENS
                    .Include(p => p.PersonagemHabilidades).ThenInclude(ph => ph.Habilidade)
                    .FirstOrDefaultAsync(p => p.Id == d.AtacanteId);

                Personagem oponente = await _context.TB_PERSONAGENS
                    .FirstOrDefaultAsync(p => p.Id == d.OponenteId);

                PersonagemHabilidade ph = await _context.TB_PERSONAGENS_HABILIDADES
                    .Include(p => p.Habilidade).FirstOrDefaultAsync(phBusca => phBusca.HabilidadeId == d.HabilidadeId && phBusca.PersonagemId == d.AtacanteId);

                if (ph == null)
                    d.Narracao = $"{atacante.Nome} não possui essa habilidade.";
                else
                {
                    int dano = ph.Habilidade.Dano + (new Random().Next(atacante.Inteligencia));
                    dano = dano - new Random().Next(oponente.Defesa);

                    if (dano > 0)
                        oponente.PontosVida = oponente.PontosVida - dano;
                    if (oponente.PontosVida <= 0)
                        d.Narracao += $"{oponente.Nome} foi derrotado!";

                    _context.TB_PERSONAGENS.Update(oponente);
                    await _context.SaveChangesAsync();

                    StringBuilder dados = new StringBuilder();
                    dados.AppendFormat("Atacante: {0}.", atacante.Nome);
                    dados.AppendFormat("Oponente: {0}.", oponente.Nome);
                    dados.AppendFormat("Pontos de vida do atacante: {0}.", atacante.PontosVida);
                    dados.AppendFormat("Pontos de vida do oponente: {0}.", oponente.PontosVida);
                    dados.AppendFormat("Habilidade Utilizada: {0}.", ph.Habilidade.Nome);
                    dados.AppendFormat("Dano: {0}.", dano);

                    d.Narracao += dados.ToString();
                    d.DataDisputa = DateTime.Now;
                    _context.TB_DISPUTAS.Add(d);
                    _context.SaveChanges();
                }
                return Ok(d);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("DisputaEmGrupo")]
        public async Task<IActionResult> DisputaEmGrupoAsync(Disputa d)
        {
            try
            {
                d.Resultados = new List<string>();//Instancia a lista de resultados

                //Busca na base dos personagens informados no parametro incluindo Armas e Habilidades
                List<Personagem> personagens = await _context.TB_PERSONAGENS
                    .Include(p => p.Arma)
                    .Include(p => p.PersonagemHabilidades).ThenInclude(ph => ph.Habilidade)
                    .Where(p => d.ListaIdPersonagens.Contains(p.Id)).ToListAsync();

                //Contagem de personagens vivos na lista obtida do banco de dados
                int qtdPersonagensVivos = personagens.FindAll(p => p.PontosVida > 0).Count;

                //Enquanto houver mais de um personagem vivo haverá disputa
                while (qtdPersonagensVivos > 1)
                {
                    //Seleciona personagens com pontos de vida positivos e depois faz solteio.
                    List<Personagem> atacantes = personagens.Where(p => p.PontosVida > 0).ToList();
                    Personagem atacante = atacantes[new Random().Next(atacantes.Count)];
                    d.AtacanteId = atacante.Id;

                    //Seleciona personagens com pontos de vida positivos, exceto o atacante escolhido e depois faz solteio.
                    List<Personagem> oponentes = personagens.Where(p => p.Id != atacante.Id && p.PontosVida > 0).ToList();
                    Personagem oponente = oponentes[new Random().Next(oponentes.Count)];

                    //Declara e redefine a cada personagem do while o valor as variáveis que serão usadas.
                    int dano = 0;
                    string ataqueUsado = string.Empty;
                    string resultado = string.Empty;

                    //Sorteia entre 0 e 1: 0 é um ataque com arma e 1 é um ataque com habilidades
                    bool ataqueUsaArma = new Random().Next(1) == 0;

                    if (ataqueUsaArma && atacante.Arma != null)
                    {
                        //Programação do ataque com arma caso o atacante possua arma (o != null) do if

                        //Sorteio da força
                        dano = atacante.Arma.Dano + (new Random().Next(atacante.Forca));
                        dano = dano - new Random().Next(oponente.Defesa); //Sorteio da defesa.
                        ataqueUsado = atacante.Arma.Nome;

                        if (dano > 0)
                            oponente.PontosVida = oponente.PontosVida - (int)dano;

                        //Formata a mensagem
                        resultado = string.Format("{0} atacou {1} usando {2} com o dano {3}. ", atacante.Nome, oponente.Nome, ataqueUsado, dano);
                        d.Narracao += resultado; // Concatena o resultado com as narrações existentes.
                        d.Resultados.Add(resultado); //Adiciona o resultado atual na lista de resultados
                    }
                    else if (atacante.PersonagemHabilidades.Count != 0)//Verifica se o personagem tem habilidades
                    {
                        //ataque habilidades

                        //Realiza o sorteio entre as habilidades existentes e na linha seguinte a seleciona.
                        int sorteioHabilidadeId = new Random().Next(atacante.PersonagemHabilidades.Count);
                        Habilidade habilidadeEscolhida = atacante.PersonagemHabilidades[sorteioHabilidadeId].Habilidade;
                        ataqueUsado = habilidadeEscolhida.Nome;

                        //Sorteio da inteligencia somada ao dano
                        dano = habilidadeEscolhida.Dano + (new Random().Next(atacante.Inteligencia));
                        dano = dano - new Random().Next(oponente.Defesa);//Sorteio da defesa.

                        if (dano > 0)
                            oponente.PontosVida = oponente.PontosVida - (int)dano;

                        resultado = string.Format("{0} atacou {1} usando {2} com o dano {3}. ", atacante.Nome, oponente.Nome, ataqueUsado, dano);
                        d.Narracao += resultado;
                        d.Resultados.Add(resultado);
                    }
                    if (!string.IsNullOrEmpty(ataqueUsado))
                    {
                        //incrementa os dados dos combates
                        atacante.Vitorias++;
                        oponente.Derrotas++;
                        atacante.Disputas++;
                        oponente.Disputas++;

                        d.Id = 0; //Zera o Id para poder salvar os dados de disputa sem erro de chave
                        d.DataDisputa = DateTime.Now;
                        _context.TB_DISPUTAS.Add(d);
                        await _context.SaveChangesAsync();
                    }

                    qtdPersonagensVivos = personagens.FindAll(p => p.PontosVida > 0).Count;

                    if (qtdPersonagensVivos == 1)//Havendo só um personagem vivo, haverá um campeão.
                    {
                        string resultadoFinal = $"{atacante.Nome.ToUpper()} é campeão com {atacante.PontosVida} pontos de vida restantes!";

                        d.Narracao += resultadoFinal; //Concatena o resultado final com as demais narrações.
                        d.Resultados.Add(resultadoFinal); // Concatena o resultado final com os demais resultados.

                        break; //break vai parar o while
                    }
                }
                //Código após o fechamento do while. Atualizará os pontos de vida, disputas, vitórias e derrotas de todos os personagens ao final das batalhas.
                _context.TB_PERSONAGENS.UpdateRange(personagens);
                await _context.SaveChangesAsync();

                return Ok(d); //retorna os dados de disputas
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("ApagarDisputas")]
        public async Task<IActionResult> DeleteAsync()
        {
            try
            {
                List<Disputa> disputas = await _context.TB_DISPUTAS.ToListAsync();

                _context.TB_DISPUTAS.RemoveRange(disputas);
                await _context.SaveChangesAsync();

                return Ok("Disputas apagadas");
            }
            catch (System.Exception ex)
            { return BadRequest(ex.Message); }
        }
        [HttpGet("Listar")] 
        public async Task<IActionResult> ListarAsync() 
        { 
            try 
            { 
                List<Disputa> disputas = 
                   await _context.TB_DISPUTAS.ToListAsync();                 
 
                return Ok(disputas); 
            } 
            catch (System.Exception ex) 
            { 
                return BadRequest(ex.Message); 
            } 
        } 
    }
}