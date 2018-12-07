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
    public class AlarmesController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(AlarmesController));
        private readonly IConfiguration _configuration; 
        AlarmesModel _alarModel = new AlarmesModel(); 
        IEnumerable <Alarmes> _alarmes;

        public AlarmesController(IConfiguration configuration) 
        {            
            _configuration = configuration;
        }

        [HttpGet]      
        public IActionResult  Index()
        { 
            log.Debug("Get Dos Apontamentos de Iluminancia em Aberto pela tela Index!");            
            _alarmes=_alarModel.SelectAlarmesAbertos(_configuration);

            if (_alarmes.Count()>0)
                return Ok(_alarmes);
            else 
                return StatusCode(505,"Não foi encotrado nenhum alarme em aberto!");
        }


        [HttpGet]      
        public IActionResult  GetAlarmes(long id,long IdLocalMedicao,string status,string Ini, string Fim)
        {
            if (id!=0 || (Ini!=null & Fim!=null) || status!=null)
            {                 
                log.Debug("Get Dos Apontamentos de Iluminancia!");            
                _alarmes=_alarModel.SelectAlarmes(_configuration, id,IdLocalMedicao,status, Ini, Fim);

                return Ok(_alarmes);
            }
            else
                return StatusCode(505,"Não foi recebido o parametro IdAlarme ou Status de Alarme ou os parametros de Data Inicio e Data Fim");
        }

        [HttpPut]
        public IActionResult PutAlarme([FromBody]Alarmes _alarme)
        {
            if (ModelState.IsValid)            
            {        
                if(_alarme.IdUsuarioReconhecimento!=null)
                    _alarme.StatusAlarme = "Reconhecido";

                if(_alarme.IdUsuarioJustificativa!=null)
                    _alarme.StatusAlarme = "Justificado";

                var insert = _alarModel.UpdateAlarmes(_configuration,_alarme);
                ///Status possíveis do alarme
                //Ativo (Alarmes que foram gerados e não tiveram reconhecimento nem justificativa)
                //Reconhecido (Alarmes que foram reconhecidos pelo operador)
                //Justificado (Alarmes que foram justificados pelo operador)
                //Encerrado Sem Reconhecimento (Alarmes que foram Encerrados por retornarem as condições normais de sistema e não foram reconhecidos)
                //Encerrado Sem Justificativa (Alarmes que foram Encerrados por retornarem as condições normais de sistema e não foram justificados)
                //Encerrado (Alarmes que foram Encerrados por retornarem as condições normais de sistema)

                if(insert==true)
                {
                   
                    log.Debug("Put do Apontamento com sucesso:" + _alarme);  
                    return Ok();
                }
                return StatusCode(500,"Houve um erro, verifique o Log do sistema!");
            }
            else 
                log.Debug("Put Alarmes não efetuado, Bad Request" + ModelState.ToString());  
                return BadRequest(ModelState);
        }       
    }
}