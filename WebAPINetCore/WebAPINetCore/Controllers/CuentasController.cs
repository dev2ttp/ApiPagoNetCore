using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebAPINetCore.Models;
using WebAPINetCore.Services;
using WebAPINetCore.PipeServer;

namespace WebAPINetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CuentasController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public CuentasController(IConfiguration configuration)
        {

            _configuration = configuration;
        }

        [HttpPost("Login")]
        public ActionResult<UserToken> Login([FromBody] UserInfo userInfo)
        {
            var mantisa = userInfo.RuT.Substring(0, userInfo.RuT.Length-1);
            var dv = userInfo.RuT.Substring(userInfo.RuT.Length - 1, 1);
            Globals.data = new List<string>();
            Globals.data.Add(mantisa);
            Globals.data.Add(dv);
            Globals.data.Add(userInfo.Password);
            Globals.Servicio1.Message = Globals.servicio.BuildMessage(ServicioPago.Comandos.logueo, Globals.data);
            var vuelta =  Globals.Servicio1.SendMessage(ServicioPago.Comandos.logueo);
           
            if (vuelta)
            {
                var mensaje = Globals.Servicio1.Resultado.Data[0].Split('~');
                UserToken Usuariorespuesta = BuildTokenUserPass(userInfo);
                if (mensaje.Length >2)
                {
                    
                    Usuariorespuesta.Rut = mantisa + "-" + dv;
                    Usuariorespuesta.IdUser = mantisa;
                    Usuariorespuesta.Nombre = mensaje[1] + " " + mensaje[2];
                    Usuariorespuesta.TipoUser = mensaje[0];
                    Usuariorespuesta.IdSession = mensaje[4];
                    return Usuariorespuesta;
                }
                Usuariorespuesta.Nombre = "Error";
                return Usuariorespuesta;
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return BadRequest(ModelState);
            }
        }


        private UserToken BuildTokenUserPass(UserInfo userInfo)
        {
            var claims = new List<Claim>
            {
        new Claim(JwtRegisteredClaimNames.UniqueName, userInfo.RuT),
        new Claim("Pass", userInfo.Password),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Tiempo de expiración del token. En nuestro caso lo hacemos de una hora.
            var expiration = DateTime.UtcNow.AddHours(1);

            JwtSecurityToken token = new JwtSecurityToken(
               issuer: null,
               audience: null,
               claims: claims,
               expires: expiration,
               signingCredentials: creds);

            return new UserToken()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }
    }
}
