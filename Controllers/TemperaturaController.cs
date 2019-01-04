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
        Charts _charts= new Charts();
               

        public TemperaturaController(IConfiguration configuration) 
        {            
            _configuration = configuration;
        } 

    
        [HttpGet]      
        public IActionResult  GetTemperaturaReport(long? IdLocalMedicao,string Ini, string Fim,decimal? Temperatura,string Etapa,long? IdSensores)
        {
            if (Ini!=null && Fim!=null)
            {                  
                log.Debug("Get Dos Apontamentos de Temperatura para Report!");            
                _report=_tpModel.TemperaturaReport(_configuration, IdLocalMedicao,Ini, Fim,Temperatura,Etapa,IdSensores);
                              
                return Ok(_report);
            }
            else
                return StatusCode(505,"Não foi recebido os parametros de Data Inicio e Data Fim");
        }
        
        [HttpGet]      
        public IActionResult  GetTemperaturaChart(long? IdLocalMedicao,string Ini, string Fim,decimal? Temperatura,string Etapa,long? IdSensores)
        {
            if (Ini!=null && Fim!=null)
            {                  
                log.Debug("Get Dos Apontamentos de Temperatura para chart!");            
                _report=_tpModel.TemperaturaReport(_configuration, IdLocalMedicao,Ini, Fim,Temperatura,Etapa,IdSensores);
                if(_report.Count()>0)
                {
                    var locais = _report.Select(ib => new {ib.DescLocalMedicao}).Distinct();
                    foreach(var item in locais)
                    {
                        Pena _pena= new Pena();
                        List<decimal> _valores = new List<decimal>();
                        foreach(var item2 in _report.Where(p=>p.DescLocalMedicao==item.DescLocalMedicao).ToList())
                        {
                            _charts.categories.Add(item2.DtColeta.Value);                            
                           _valores.Add(item2.Valor);                        

                        }
                        _pena.name = item.DescLocalMedicao;
                        _pena.data=_valores;
                        _charts.series.Add(_pena);
                        
                    }
                }

                return Ok(_charts);
            }
            else
                return StatusCode(505,"Não foi recebido o parametro os parametros de Data Inicio e Data Fim");
        }

    }
}