using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ProjectCleanning_Backend.Models;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.AspNetCore.Cors;
using System.Linq;

namespace ProjectCleanning_Backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [EnableCors("CorsPolicy")]
    public class ArComprimidoController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ArComprimidoController));
        private readonly IConfiguration _configuration; 
        ArComprimidoModel _arModel = new ArComprimidoModel();    
     
        IEnumerable<ArComprimido> _ar;

        public ArComprimidoController(IConfiguration configuration) 
        {            
            _configuration = configuration;
        } 

        //Get api/GetArComprimido
        [HttpGet]      
        public IActionResult  GetArComprimido(long IdApontArComprimido,long IdLocalMedicao,string Ini, string Fim,string status,bool? Ocorrencia)
        {
            if (IdApontArComprimido!=0 || (Ini!=null && Fim!=null) || IdLocalMedicao!=0)
            {                  
                log.Debug("Get Dos Apontamentos de Ar Comprimido!");            
                _ar=_arModel.SelectArComprimido(_configuration, IdApontArComprimido,IdLocalMedicao, Ini, Fim,status,Ocorrencia);                              
                return Ok(_ar);
            }
            else
                return StatusCode(505,"Não foi recebido o parametro IdApontArComprimido ou  IdLocalMedicao ou os parametros de Data Inicio e Data Fim");
        }
        [HttpPut]
        public IActionResult PutArComprimido([FromBody]ArComprimido _arComp)
        {
            if (ModelState.IsValid)            
            {        
                
                var insert = _arModel.UpdateArComprimido(_configuration,_arComp);
                
                if(insert==true)
                {
                   
                    log.Debug("Put do Apontamento Ar Comprimido com sucesso:" + _arComp);  
                    return Ok();
                }
                return StatusCode(500,"Houve um erro, verifique o Log do sistema!");
            }
            else 
                log.Debug("Put Ar Comprimido não efetuado, Bad Request" + ModelState.ToString());  
                return BadRequest(ModelState);
        }       

        [HttpPost]
        public IActionResult PostArComprimido([FromBody]ArComprimido _ar)
        {
            if (ModelState.IsValid)            
            {
                var insert = _arModel.InsertArComprimido(_configuration,_ar);
                if(insert==true)
                {
                   
                    log.Debug("Post do Apontamento com sucesso:" + _ar);  
                    return Json(_ar);
                }
                return StatusCode(500,"Houve um erro, verifique o Log do sistema!");
            }
            else 
                log.Debug("Post não efetuado, Bad Request" + ModelState.ToString());  
                return BadRequest(ModelState);
        }
   }
}