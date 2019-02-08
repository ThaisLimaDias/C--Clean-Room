using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Embraer_Backend.Models;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.AspNetCore.Cors;
using System.Linq;

namespace Embraer_Backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [EnableCors("CorsPolicy")]
    public class SecagensController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(SecagensController));
        private readonly IConfiguration _configuration; 
        SecagensModel _secModel = new SecagensModel(); 
        IEnumerable <Secagens> _sec;
        IEnumerable <SecagensOp> _secop;

        public SecagensController(IConfiguration configuration) 
        {            
            _configuration = configuration;
        }

        [HttpGet] 
        public IActionResult Index()
        {
            //Apontamentos Pendentes
           _sec=_secModel.SelectSecagensAbertas(_configuration);
            if(_sec.Count()>0)            
                return Ok(_sec);
            
            return StatusCode(204);
            
        }

        [HttpGet]      
        public IActionResult  GetSecagens(long? id,string status,string Ini, string Fim)
        {
            if (id!=0 || (Ini!=null & Fim!=null) || status!=null)
            {                 
                log.Debug("Get Dos Malotes de Secagens!");            
                _sec=_secModel.SelectSecagens(_configuration, id,status, Ini, Fim);

                return Ok(_sec);
            }
            else
                return StatusCode(505,"Não foi recebido o parametro IdSecagem ou Status de Secagem ou os parametros de Data Inicio e Data Fim!");
        }

        [HttpGet]      
        public IActionResult  GetSecagensOp(long id)
        {
            if (id!=0)
            {                 
                log.Debug("Get Dos Malotes de Secagens!");            
                _secop=_secModel.SelectSecagensOp(_configuration, id);

                return Ok(_secop);
            }
            else
                return StatusCode(505,"Não foi recebido o parametro IdSecagem!");
        }

        [HttpPut]
        public IActionResult PutSecagens([FromBody]Secagens _sec)
        {
             
                ///------------Status possíveis das Secagens-------------------------//
                //Iniciado (Secagens com coleta Iniciada)
                //Aberto (Secagens com coleta finalizada porém podem ainda podem ter ops vinculadas)
                //Encerrado (Secagens com coleta finalizada que não podem mais ter ops vinculadas)
                if(_sec.DtFimMalote==null || _sec.StatusMalote=="Iniciado")
                    return StatusCode(500,"Uma secagem que a coleta não esteja finalizada não pode ser encerrada!");

                else
                {
                    var update = _secModel.UpdateSecagens(_configuration,_sec);
                    if(update==true)
                    {
                    
                        log.Debug("Put da Secagem com sucesso:" + _sec);  
                        return Ok();
                    }
                    else
                        return StatusCode(500,"Houve um erro, verifique o Log do sistema!");
                }
    
        }  
        [HttpPost]
        public IActionResult PostSecagensOp([FromBody]List<SecagensOp> _secop)
        {
            if (ModelState.IsValid)            
            {
                foreach(var item in _secop)
                {
                    _secModel.InsertSecagensOP(_configuration,item); 
                }       
                                                         
                return Json(_secop);               
                
            }
            else 
                log.Debug("Post não efetuado, Bad Request" + ModelState.ToString());  
                return BadRequest(ModelState);
        }
     
    }
}