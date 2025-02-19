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
    public class PortaController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ControleApontamentoController));
         private readonly IConfiguration _configuration;
        
        IEnumerable<Porta> _ctrl;
        PortaModel _model= new PortaModel();

        public PortaController(IConfiguration configuration) 
        {            
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult GetPortas(string Ini, string Fim,long? IdLocalColeta, string status)
        {
            if (Ini!=null && Fim!=null)          
            {
                var portas = _model.SelectPorta(_configuration,IdLocalColeta,Ini,Fim,status);

                if(portas.Count()>0)
                {                   
                    log.Debug("Get Portas com sucesso:" + portas);  
                    return Ok(portas);
                }
                return StatusCode(204);
            }
            else
                return StatusCode(505,"Não foi recebido os parametros de Data Inicio e Data Fim");
        }       
        
    }
}