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
    public class UmidadeController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(UmidadeController));
        private readonly IConfiguration _configuration; 
        UmidadeModel _umModel = new UmidadeModel();    
        IEnumerable<UmidadeReport> _report;

        public UmidadeController(IConfiguration configuration) 
        {            
            _configuration = configuration;
        } 

    
        [HttpGet]      
        public IActionResult  GetUmidadeReport(long IdLocalMedicao,string Ini, string Fim)
        {
            if (Ini!=null && Fim!=null)
            {                  
                log.Debug("Get Dos Apontamentos de Umidade para Report!");            
                _report=_umModel.UmidadeReport(_configuration, IdLocalMedicao,Ini, Fim);
                              
                return Ok(_report);
            }
            else
                return StatusCode(505,"Não foi recebido o parametro IdLocalMedicao ou os parametros de Data Inicio e Data Fim");
        }

    }
}