using estudo_api.Models;
using Microsoft.AspNetCore.Mvc;
using estudo_api.Data;
using System.Text;
using System.Linq;
using System;
//using Microsoft.AspNetCore.Identity;
//using System.IdentityModel.Tokens;
//using System.Security;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Collections.Generic;
using System.Security.Claims;

namespace estudo_api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext database;

        public UsuariosController(ApplicationDbContext database)
        {
            this.database = database;
        } 

        //registrar um usuario
        //api/v1/usuarios/registro
        [HttpPost("registro")]
        public IActionResult Registro([FromBody] Usuario usuarios)
        {
            // verificar se as credenciais são validas
            // verificar se o e-mail já esta cadastrado no banco
            // Encriptar a senha
            database.Add(usuarios);
            database.SaveChanges();
            return Ok(new{msg="Usuario cadastrado com sucesso"});
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] Usuario credencial)
        {
            //buscar um usuario por e-mail
            // verificar se a senha está correta
            // gerar um token TWT e retornar esse token para o usario

            try{
                Usuario usuario = database.Usuarios.First(user => user.Email.Equals(credencial.Email));

                if(usuario != null)
                {
                    //achou um usario com cadastro valido
                    if(usuario.Senha.Equals(credencial.Senha))
                    {
                        //usuario acertou a senha, entao logou

                        //chave de segurança
                        string chaveDeSegurana = "kemylly_cavalcante_santos";
                        var chaveSimetrica = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveDeSegurana));
                        var credenciaisDeAcesso = new SigningCredentials(chaveSimetrica,SecurityAlgorithms.HmacSha256Signature);

                        var claims = new List<Claim>();
                        claims.Add(new Claim("id",usuario.Id.ToString())); //claim que guarad o id do usuario aqui pra mim
                        claims.Add(new Claim("email",usuario.Email)); //pegar o email do usuario e colocar em uma claim
                        claims.Add(new Claim(ClaimTypes.Role,"Admin")); //pegar o cargo - tipo do usuario

                        //criando o token e coisas necessarias
                        var JWT = new JwtSecurityToken(
                            issuer: "supermecado.com",  //issuer = quem esta fornecendo o jwt ao usuario 
                            expires: DateTime.Now.AddHours(1), //quando expira
                            audience: "usuario_comum", //para quem esta destinado esse token
                            signingCredentials: credenciaisDeAcesso,  //credenciais de acesso de token
                            claims : claims
                        );

                        return Ok(new JwtSecurityTokenHandler().WriteToken(JWT)); //gerar token
                    }else{
                        //usuario errou a senha
                        Response.StatusCode = 401;
                        return new ObjectResult(new {msg = "Senha invalida"});
                    }
                }else{
                    //não existe nenhum usuario com esse email
                    Response.StatusCode = 401;  //401 = não autorizado
                    return new ObjectResult(new {msg = "Email invalido"});
                }
            }
            catch(Exception e){
                Response.StatusCode = 401;  //401 = não autorizado
                return new ObjectResult(new {msg = "Usuario invalido"});
            }
            

            
        }
    }
}