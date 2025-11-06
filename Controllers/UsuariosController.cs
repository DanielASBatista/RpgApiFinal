using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using RpgApi.Data;
using RpgApi.Models;
using RpgApi.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace RpgApi.Controllers
{   
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public UsuariosController(DataContext contextUsuarios, IConfiguration configuration)
        {
            _context = contextUsuarios;
            _configuration = configuration; 
        }

        private async Task<bool> UsuarioExistente(string Username)
        {
            if (await _context.TB_USUARIOS.AnyAsync(x => x.Username.ToLower() == Username.ToLower()))
            {
                return true;
            }
            return false;
        }
        private string CriarToken(Usuarios usuario)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Username),
                new Claim(ClaimTypes.Role, usuario.Perfil)
            };
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("ConfiguracaoToken:Chave").Value));

            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        [AllowAnonymous]
        [HttpPost("Registrar")]
        public async Task<IActionResult> RegistrarUsuario(Usuarios User)
        {
            try
            {
                if (await UsuarioExistente(User.Username))
                    throw new System.Exception("Nome do Usuario ja existe");
                Criptografia.CriarPasswordHash(User.PasswordString, out byte[] hash, out byte[] salt);
                User.PasswordString = string.Empty;
                User.PasswordHash = hash;
                User.PasswordSalt = salt;
                await _context.TB_USUARIOS.AddAsync(User);
                await _context.SaveChangesAsync();

                return Ok(User.Id);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [AllowAnonymous]
        [HttpPost("Autenticar")]
        public async Task<IActionResult> AutenticarUsuario(Usuarios credenciais)
        {
            try
            {
                Usuarios? usuarios = await _context.TB_USUARIOS.FirstOrDefaultAsync(x => x.Username.ToLower().Equals(credenciais.Username.ToLower()));
                if (usuarios == null)
                {
                    throw new System.Exception("Usuario não encontrado");
                }
                else if (!Criptografia.VerificarPasswordHash(credenciais.PasswordString, usuarios.PasswordHash, usuarios.PasswordSalt))
                {
                    throw new System.Exception("Senha incorreta.");
                }
                else
                {
                    usuarios.DataAcesso = System.DateTime.Now;
                    _context.TB_USUARIOS.Update(usuarios);
                    await _context.SaveChangesAsync(); //Confirma a alteração no banco

                    usuarios.PasswordHash = null;
                    usuarios.PasswordSalt = null;
                    usuarios.Token = CriarToken(usuarios);

                    return Ok(usuarios);
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        //Método para alteração de senha.
        [HttpPut("AlterarSenha")]
        public async Task<IActionResult> AlterarSenhaUsuário(Usuarios credenciais)
        {
            try
            {
                Usuarios? usuario = await _context.TB_USUARIOS //Busca o usuário no banco através do login
                    .FirstOrDefaultAsync(x => x.Username.ToLower().Equals(credenciais.Username.ToLower()));

                if (usuario == null)
                    throw new System.Exception("Usuário não encontrado.");

                Criptografia.CriarPasswordHash(credenciais.PasswordString, out byte[] hash, out byte[] salt);
                usuario.PasswordHash = hash;//Se o usuário existir, executa a criptografia
                usuario.PasswordSalt = salt;//guardando o hash e o salt nas propriedades do usuário

                _context.TB_USUARIOS.Update(usuario);
                int linhasAfetadas = await _context.SaveChangesAsync();
                return Ok(linhasAfetadas);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetUsuarios()
        {
            try
            {
                List<Usuarios> lista = await _context.TB_USUARIOS.ToListAsync();
                return Ok(lista);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("{usuarioId}")]
        public async Task<IActionResult> GetUsuario(int usuarioId)
        {
            try
            {
                //List exigirá o using System.Collections.Generic 
                Usuarios usuario = await _context.TB_USUARIOS //Busca o usuário no banco através do Id 
                   .FirstOrDefaultAsync(x => x.Id == usuarioId);

                return Ok(usuario);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetByLogin/{login}")]
        public async Task<IActionResult> GetUsuario(string login)
        {
            try
            {
                //List exigirá o using System.Collections.Generic 
                Usuarios usuario = await _context.TB_USUARIOS //Busca o usuário no banco através do login 
                   .FirstOrDefaultAsync(x => x.Username.ToLower() == login.ToLower());

                return Ok(usuario);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        //Método para alteração da geolocalização 
        [HttpPut("AtualizarLocalizacao")]
        public async Task<IActionResult> AtualizarLocalizacao(Usuarios u)
        {
            try
            {
                Usuarios usuario = await _context.TB_USUARIOS //Busca o usuário no banco através do Id 
                   .FirstOrDefaultAsync(x => x.Id == u.Id);

                usuario.Latitude = u.Latitude;
                usuario.Longitude = u.Longitude;

                var attach = _context.Attach(usuario);
                attach.Property(x => x.Id).IsModified = false;
                attach.Property(x => x.Latitude).IsModified = true;
                attach.Property(x => x.Longitude).IsModified = true;

                int linhasAfetadas = await _context.SaveChangesAsync(); //Confirma a alteração no banco 
                return Ok(linhasAfetadas); //Retorna as linhas afetadas (Geralmente sempre 1 linha msm) 
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("AtualizarEmail")]
        public async Task<IActionResult> AtualizarEmail(Usuarios u)
        {
            try
            {
                Usuarios usuario = await _context.TB_USUARIOS //Busca o usuário no banco através do Id 
                   .FirstOrDefaultAsync(x => x.Id == u.Id);

                usuario.Email = u.Email;

                var attach = _context.Attach(usuario);
                attach.Property(x => x.Id).IsModified = false;
                attach.Property(x => x.Email).IsModified = true;

                int linhasAfetadas = await _context.SaveChangesAsync(); //Confirma a alteração no banco 
                return Ok(linhasAfetadas); //Retorna as linhas afetadas (Geralmente sempre 1 linha msm) 
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        //Método para alteração da foto 
        [HttpPut("AtualizarFoto")] 
        public async Task<IActionResult> AtualizarFoto(Usuarios u) 
        { 
            try 
            { 
                Usuarios usuario = await _context.TB_USUARIOS  
                   .FirstOrDefaultAsync(x => x.Id == u.Id); 
 
                usuario.Foto = u.Foto;                 
 
                var attach = _context.Attach(usuario); 
                attach.Property(x => x.Id).IsModified = false; 
                attach.Property(x => x.Foto).IsModified = true;                 
 
                int linhasAfetadas = await _context.SaveChangesAsync();  
                return Ok(linhasAfetadas);  
            } 
            catch (System.Exception ex) 
            { 
                return BadRequest(ex.Message); 
            } 
        }  
    }
}