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
    public class IluminanciaController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(IluminanciaController));
        private readonly IConfiguration _configuration; 
        IluminanciaModel _ilModel = new IluminanciaModel();    
        EquipamentosModel _equipModel = new EquipamentosModel(); 
        LocalMedicaoModel _localMedicaoModel = new LocalMedicaoModel();
        ParametrosModel _parModel = new ParametrosModel();
        IEnumerable<Iluminancia> _iluminancias;
        IEnumerable<IluminanciaMedicoes> _medicoes;
        IEnumerable<Parametros> _par;
        IEnumerable<IluminanciaReport> _report;
        ControleApontamento _ctrl;
        ControleApontamentoModel _ctrlModel = new ControleApontamentoModel();

        public IluminanciaController(IConfiguration configuration) 
        {            
            _configuration = configuration;
        } 

        public IActionResult  Index(long IdLocalMedicao)
        {
            if (IdLocalMedicao>0)
            {
                log.Debug("Get Do parametro de Iluminancia!"); 
                _par = _parModel.SelectParametros(_configuration,IdLocalMedicao,"Iluminância");
                return Ok(_par);
            }
            else 
                return StatusCode(505,"Não foi recebido o parametro IdLocalMedicao!");
        }

        //Get api/GetIluminancia
        [HttpGet]      
        public IActionResult  GetIluminancia(long id,long IdLocalMedicao,string Ini, string Fim,bool Ocorrencia)
        {
            if (id!=0 || (Ini!=null && Fim!=null))
            {                  
                log.Debug("Get Dos Apontamentos de Iluminancia!");            
                _iluminancias=_ilModel.SelectIluminancia(_configuration, id,Ini, Fim, IdLocalMedicao,Ocorrencia);
                              
                return Ok(_iluminancias);
            }
            else
                return StatusCode(505,"Não foi recebido o parametro IdIluminancia ou os parametros de Data Inicio e Data Fim");
        }
         [HttpGet]      
        public IActionResult  GetIluminanciaReport(long IdLocalMedicao,string Ini, string Fim)
        {
            if (Ini!=null && Fim!=null)
            {                  
                log.Debug("Get Dos Apontamentos de Iluminancia para Report!");            
                _report=_ilModel.IluminanciaReport(_configuration, IdLocalMedicao,Ini, Fim);
                              
                return Ok(_report);
            }
            else
                return StatusCode(505,"Não foi recebido o parametro IdLocalMedicao ou os parametros de Data Inicio e Data Fim");
        }


        //Get api/GetIMedicoes
        [HttpGet("{idLuminancia}")]       
        public IActionResult  GetMedicoes(long idLuminancia)
        {
            if (idLuminancia!=0)
            {
                log.Debug("Get Dos Apontamentos de Iluminancia!");            
                _medicoes=_ilModel.SelectMedicaoIluminancia(_configuration, idLuminancia);
                
                return Ok(_medicoes);              
            }
            return StatusCode(505,"O idLuminancia não pode ser nulo nem Igual a 0!");
        }

        [HttpPost]
        public IActionResult PostIluminancia([FromBody]Iluminancia _iluminancia)
        {
            if (ModelState.IsValid)            
            {                
                var insert = _ilModel.InsertIluminancia(_configuration,_iluminancia);

                if(insert==true)
                {
                    log.Debug("Post do Apontamento com sucesso:" + _iluminancia);  
                    return Json(_iluminancia);
                }
                return StatusCode(500,"Houve um erro, verifique o Log do sistema!");
            }
            else 
                log.Debug("Post não efetuado, Bad Request" + ModelState.ToString());  
                return BadRequest(ModelState);
        }

        [HttpPost]
        public IActionResult PostMedicoes([FromBody]List<IluminanciaMedicoes> _med)
        {
            if (ModelState.IsValid)            
            {
                foreach(var item in _med)
                {
                    _ilModel.InsertMedicaoIluminancia(_configuration,item); 
                }       
                _ctrl = _ctrlModel.SelectControleApontamento(_configuration,"Iluminância").FirstOrDefault();
                _ctrl.ProxApont=_ctrl.ProxApont.Value.AddDays(_ctrl.DiasProximaMed);
                _ctrlModel.UpdateControleApontamento(_configuration,_ctrl);                                           
                return Json(_med);               
                
            }
            else 
                log.Debug("Post não efetuado, Bad Request" + ModelState.ToString());  
                return BadRequest(ModelState);
        }
       
        [HttpPut]
        public IActionResult PutIluminancia([FromBody]Iluminancia _iluminancia)
        {
            if (ModelState.IsValid)            
            {
                var insert = _ilModel.Updateluminancia(_configuration,_iluminancia);

                if(insert==true)
                {
                   
                    log.Debug("Put do Apontamento com sucesso:" + _iluminancia);  
                    return Ok();
                }
                return StatusCode(500,"Houve um erro, verifique o Log do sistema!");
            }
            else 
                log.Debug("Put Iluminância não efetuado, Bad Request" + ModelState.ToString());  
                return BadRequest(ModelState);
        }       

    }
}