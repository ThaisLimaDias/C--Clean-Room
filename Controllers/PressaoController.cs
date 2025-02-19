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
    public class PressaoController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(PressaoController));
        private readonly IConfiguration _configuration; 
        PressaoModel _prModel = new PressaoModel();    
        IEnumerable<PressaoReport> _report;
        Charts _charts= new Charts();
        public PressaoController(IConfiguration configuration) 
        {            
            _configuration = configuration;
        } 

    
        [HttpGet]      
        public IActionResult  GetPressaoReport(long? IdLocalMedicao,string Ini, string Fim,decimal? Pressao,long? IdSensores)
        {
            if (Ini!=null && Fim!=null)
            {                  
                log.Debug("Get Dos Apontamentos de Pressao para Report!");            
                _report=_prModel.PressaoReport(_configuration, IdLocalMedicao,Ini, Fim,Pressao,IdSensores);
                              
                return Ok(_report);
            }
            else
                return StatusCode(505,"Não foi recebido o parametro IdLocalMedicao ou os parametros de Data Inicio e Data Fim");
        }

        [HttpGet]      
        public IActionResult  GetPressaoChart(long? IdLocalMedicao,string Ini, string Fim,decimal? Pressao,string Etapa,long? IdSensores)
        {
            if (Ini!=null && Fim!=null)
            {                  
                log.Debug("Get Dos Apontamentos de Temperatura para chart!");            
                _report=_prModel.PressaoReport(_configuration, IdLocalMedicao,Ini, Fim,Pressao,IdSensores);
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