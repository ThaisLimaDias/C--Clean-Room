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
    public class TemperaturaController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(TemperaturaController));
        private readonly IConfiguration _configuration; 
        TemperaturaModel _tpModel = new TemperaturaModel();    
        IEnumerable<TemperaturaReport> _report;

        public TemperaturaController(IConfiguration configuration) 
        {            
            _configuration = configuration;
        } 

    
        [HttpGet]      
        public IActionResult  GetTemperaturaReport(long IdLocalMedicao,string Ini, string Fim)
        {
            if (Ini!=null && Fim!=null)
            {                  
                log.Debug("Get Dos Apontamentos de Temperatura para Report!");            
                _report=_tpModel.TemperaturaReport(_configuration, IdLocalMedicao,Ini, Fim);
                              
                return Ok(_report);
            }
            else
                return StatusCode(505,"Não foi recebido o parametro IdLocalMedicao ou os parametros de Data Inicio e Data Fim");
        }

    }
}