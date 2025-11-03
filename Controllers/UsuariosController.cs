using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using RpgApi.Data;
using RpgApi.Models;
using RpgApi.Utils;

namespace RpgApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly DataContext _context;
        public UsuariosController(DataContext contextUsuarios){
            _context = contextUsuarios;
        }

        private async Task<bool> UsuarioExistente(string Username){
            if (await _context.TB_USUARIOS.AnyAsync(x => x.Username.ToLower() == Username.ToLower())){
                return true;
            }
            return false;
        }
        [HttpPost("Registrar")]
        public async Task<IActionResult> RegistrarUsuario(Usuarios User){
            try{
                if (await UsuarioExistente(User.Username))
                    throw new System.Exception("Nome do Usuario ja existe");
                Criptografia.CriarPasswordHash(User.PasswordString,out byte[] hash,out byte[] salt);
                User.PasswordString = string.Empty;
                User.PasswordHash = hash;    
                User.PasswordSalt = salt;
                await _context.TB_USUARIOS.AddAsync(User);
                await _context.SaveChangesAsync();

                return Ok(User.Id);   
            }
            catch(System.Exception ex){
                return BadRequest(ex.Message);
            }
        }
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
        
    
    }

}