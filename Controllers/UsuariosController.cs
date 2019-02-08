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

        public IActionResult IndexUsuario(string status)
        {
            
            log.Debug("Get de Apontamento do Usuario");
                IEnumerable<Usuario> _user = _userModel.SelectUsuario(_configuration, null,status);

                return Ok(_user);            
        }

        [HttpGet]
        public IActionResult GetUsuario(string codUsuario)
        {
            if (codUsuario != null)
            {
                log.Debug("Get de Apontamento do Usuario");
                IEnumerable<Usuario> _user = _userModel.SelectUsuario(_configuration, codUsuario,null);

                return Ok(_user);
            }
            else
                return StatusCode(505, "Não Foi Recebido os Dados do Usuário!");
        }

        [HttpPut]
        public IActionResult PutUsuario([FromBody]Usuario _user)
        {
            if (ModelState.IsValid)
            {
                var put = _userModel.UpdateUsuario(_configuration, _user);

                if (put == true)
                {
                    log.Debug("Post do Usuario OK" + _user);
                    return Ok();

                }
                return StatusCode(500, "Houve Um Erro, Verifique o Log!");
            }

            else
                log.Debug("Post não efetuado, Bad Request" + ModelState.ToString());
                return BadRequest(ModelState);

        }

        [HttpPut]
        public IActionResult PutDesativarUsuario(string CodUsuario)
        {           

            if (CodUsuario != null && CodUsuario!="")
            {
                log.Debug("Put Edit Status!");

                var put = _userModel.DeleteUsuario(_configuration, CodUsuario);

                if (put == true)
                {
                    log.Debug("Put Usuário desativado com sucesso");
                    return Ok("Usuário desativado com sucesso!");
                }
                return StatusCode(500, "Houve um erro, verifique o Log do sistema!");

            }
            else
            {
                return StatusCode(505, "Não foi recebido o parametro CodUsuario");
            }

        }

        [HttpPut]
        public IActionResult PutSenha([FromQuery]string codUsuario,[FromQuery] string SenhaAtual,[FromQuery] string SenhaNova)
        {           

            if (codUsuario!="" && codUsuario!=null)
            {
                log.Debug("Verifica se usuário existe!");
                var usuario = _userModel.SelectUsuario(_configuration,codUsuario,null);
                if(usuario.FirstOrDefault().Senha!=SenhaAtual)
                {
                    return StatusCode(500, "A senha atual digitada, não corresponde com a senha atual cadastrada!");
                }
                else
                {
                    var put = _userModel.UpdateUsuarioSenha(_configuration,SenhaNova,codUsuario);
                    if (put == true)
                    {
                        log.Debug("Put Usuário senha alterado com sucesso");
                        return Ok("Senha Atualizada com sucesso!");
                    }
                }
                return StatusCode(500, "Houve um erro, verifique o Log do sistema!");

            }
            else
            {
                return StatusCode(505, "Não foi recebido o parametro CodUsuario");
            }

        }

        [HttpPut]
        public IActionResult PutResetSenha([FromQuery]string codUsuario,[FromQuery] string SenhaNova)
        {           

            if (codUsuario!="" && codUsuario!=null)
            {
                log.Debug("Verifica se usuário existe!");
                var put = _userModel.UpdateUsuarioSenha(_configuration,SenhaNova,codUsuario);
                if (put == true)
                {
                    log.Debug("Put Usuário senha alterado com sucesso");
                    return Ok("Senha redefinida com sucesso!");
                }                
                return StatusCode(500, "Houve um erro, verifique o Log do sistema!");

            }
            else
            {
                return StatusCode(505, "Não foi recebido o parametro CodUsuario");
            }

        }

         [HttpPost]
        public IActionResult PostUsuario([FromBody]Usuario _user)
        {
            if (_user.CodUsuario != null)            
            {
                var existis = _userModel.SelectUsuario(_configuration,_user.CodUsuario,null);
                if(existis.Count()>0)
                    return StatusCode(505,"Usuário não cadastrado! Já existe um usuário com o login "+ _user.CodUsuario);

                else
                {
                    var insert = _userModel.InsertUsuario(_configuration, _user);      

                    if (insert==true)
                        return Ok("Usuário "+ _user.CodUsuario + " cadastrado com sucesso!");               
                    
                    else
                        return StatusCode(505,"Houve um erro, verifique o Log do sistema!");   
                }                                             
            }
            else 
                log.Debug("Post não efetuado, Bad Request" + ModelState.ToString());  
                return BadRequest(ModelState);  

    }

}
}
