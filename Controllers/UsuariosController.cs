using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Embraer_Backend.Models;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.AspNetCore.Cors;
using System.Linq;

namespace C_Embraer_Clean_Room.Controllers
{
    [Route("api/[controller]/[action]")]
    [EnableCors("CorsPolicy")]
    public class UsuariosController : Controller
    {
           private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(UsuariosController));
        private readonly IConfiguration _configuration; 
        UsuarioModel _userModel = new UsuarioModel();    
     
        public UsuariosController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]

        public IActionResult GetUsuario(string codUsuario)
        {
            if (codUsuario != null)
            {
                log.Debug("Get de Apontamento do Usu�rio");
                IEnumerable<Usuario> _user = _userModel.SelectUsuario(_configuration, codUsuario);

                return Ok(_user);
            }
            else
                return StatusCode(505, "N�o Foi Recebido os Dados do Usu�rio!");
        }

        [HttpPut]
        public IActionResult PutUsuario([FromBody]Usuario _user)
        {
            if (ModelState.IsValid)
            {
                var insert = _userModel.UpdateUsuario(_configuration, _user);

                if (insert == true)
                {
                    log.Debug("Post do Usuario OK" + _user);
                    return Json(_user);

                }

                return StatusCode(500, "Houve Um Erro, Verifique o Log!");

            }

            else
                log.Debug("Post n�o efetuado, Bad Request" + ModelState.ToString());
            return BadRequest(ModelState);

        }

        [HttpPut]
        public IActionResult PutDesativarUsuario(long IdUsuario)
        {
            var insert = false;

            if (IdUsuario != 0)
            {
                log.Debug("Put Edit Status!");

                insert = _userModel.DeleteUsuario(_configuration, IdUsuario);

                if (insert == true)
                {
                    log.Debug("Put Status alterado com sucesso");
                    return Ok();
                }
                return StatusCode(500, "Houve um erro, verifique o Log do sistema!");

            }
            else
            {
                return StatusCode(505, "N�o foi recebido o parametro IdUsuario");
            }

        }

         [HttpPost]
        public IActionResult PostUsuario(string codUsuario, string Senha,string Funcao, string Nome, string Status)
        {
            if (codUsuario != null)            
            {
               
                var insert = _userModel.InsertUsuario(_configuration, Nome, codUsuario, Senha, Funcao );      

                if (insert==true){
                    return Ok();               
                }else{
                    return StatusCode(505,"Houve um erro, verifique o Log do sistema!");
                }                                                
            }
            else 
                log.Debug("Post não efetuado, Bad Request" + ModelState.ToString());  
                return BadRequest(ModelState);  

    }

}
}
