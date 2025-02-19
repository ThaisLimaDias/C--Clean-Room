using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ProjectCleanning_Backend.Models;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.AspNetCore.Cors;
using System.Linq;

namespace C_ProjectCleanning_Clean_Room.Controllers
{
    [Route("api/[controller]/[action]")]
    [EnableCors("CorsPolicy")]
    public class TokenController : Controller
    {
           private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(TokenController));
        private readonly IConfiguration _configuration; 
        TokenModel _tokenModel = new TokenModel();    
     
        public TokenController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]

        public IActionResult GetToken(long IdUsuario)
        {  
            if (IdUsuario != 0)
            {          
                log.Debug("Get de Apontamento do Usuario");
                Token token = _tokenModel.SelectToken(_configuration, IdUsuario);

                return Ok(token);       
                }     
            else
                return StatusCode(505, "Não Foi Recebido o IdUsuário do Usuário!");
        }
       
        [HttpPut]
        public IActionResult PutToken([FromBody]Token _token)
        {
            if (ModelState.IsValid)
            {
                var put = _tokenModel.UpdateToken(_configuration, _token);

                if (put == true)
                {
                    log.Debug("Post do Usuario OK" + _token);
                    return Ok();

                }
                return StatusCode(500, "Houve Um Erro, Verifique o Log!");
            }

            else
                log.Debug("Post não efetuado, Bad Request" + ModelState.ToString());
                return BadRequest(ModelState);

        }

        [HttpDelete]
        public IActionResult DeleteToken(long IdUsuario)
        {           

            if (IdUsuario != 0)
            {
                log.Debug("Delete Status!");

                var put = _tokenModel.DeleteToken(_configuration, IdUsuario);

                if (put == true)
                {
                    log.Debug("Put Token excluído com sucesso");
                    return Ok("Token excluído com sucesso!");
                }
                return StatusCode(500, "Houve um erro, verifique o Log do sistema!");

            }
            else
            {
                return StatusCode(505, "Não foi recebido o parametro IdUsuario");
            }

        }

        [HttpPost]
        public IActionResult PostToken([FromBody]Token _token)
        {
            if (ModelState.IsValid)
            {
                var existis = _tokenModel.SelectToken(_configuration,_token.IdUsuario);
                if(existis!=null)
                    return StatusCode(505,"Token não cadastrado! Já existe um Token com o IdUsuario, Token Ativo: "+ _token.TokenUser);

                else
                {
                    var insert = _tokenModel.InsertToken(_configuration, _token);      

                    if (insert==true)
                        return Ok("IdUsuário: "+ _token.IdUsuario + " e Token: "+ _token.TokenUser + "cadastrado com sucesso!");               
                    
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
