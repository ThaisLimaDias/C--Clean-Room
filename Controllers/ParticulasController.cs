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
    public class ParticulasController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ParticulasController));
        private readonly IConfiguration _configuration; 
        ParticulasModel _prtModel = new ParticulasModel();    
        LocalMedicaoModel _localMedicaoModel = new LocalMedicaoModel();
        ParametrosModel _parModel = new ParametrosModel();
        IEnumerable<Particulas> _particulas;
        IEnumerable<ParticulasMedicoes> _medicoes;
        IEnumerable<Parametros> _par;

        IEnumerable<ParticulasTam> _tampart;

        public ParticulasController(IConfiguration configuration) 
        {            
            _configuration = configuration;
        } 
        public IActionResult  Index(long IdLocalMedicao)
        {
            if (IdLocalMedicao>0)
            {
                log.Debug("Get Do parametro de Particulas!"); 
                _par = _parModel.SelectParametros(_configuration,IdLocalMedicao,"Particulas");
                return Ok(_par);
            }
            else 
                return StatusCode(505,"Não foi recebido o parametro IdLocalMedicao!");
        }

        //Get api/GetParticulas
        [HttpGet]       
        public IActionResult  GetParticulas(long id, string Ini, string Fim,bool Ocorrencias)
        {
            if (id!=0 || (Ini!=null && Fim!=null))
            {
                    
                log.Debug("Get Dos Apontamentos de Particulas!");            
                _particulas=_prtModel.SelectParticulas(_configuration, id, Ini, Fim,Ocorrencias);
                
                return Ok(_particulas);
            }
            else
                return StatusCode(505,"Não foi recebido o parametro IdIApontParticulas ou os parametros de Data Inicio e Data Fim");
        }

        //Get api/GetMedicoes
        [HttpGet("{IdIApontParticulas}")]       
        public IActionResult  GetMedicoes(long IdIApontParticulas)
        {
            if (IdIApontParticulas!=0)
            {
                log.Debug("Get Dos Apontamentos de Particulas!");            
                _medicoes=_prtModel.SelectMedicaoParticulas(_configuration, IdIApontParticulas);
                
                return Ok(_medicoes);              
            }
            return StatusCode(505,"O IdIApontParticulas não pode ser nulo nem Igual a 0!");
        }

                //Get api/GetMedicoes
        [HttpGet("{IdMedicaoParticulas}")]       
        public IActionResult  GetMedicoesTamParticulas(long IdMedicaoParticulas)
        {
            if (IdMedicaoParticulas!=0)
            {
                log.Debug("Get Dos Apontamentos de Tamanhos Particulas!");            
                _tampart=_prtModel.SelectParticulasTam(_configuration, IdMedicaoParticulas);
                
                return Ok(_tampart);              
            }
            return StatusCode(505,"O IdTamParticulas não pode ser nulo nem Igual a 0!");
        }


        [HttpPost]
        public IActionResult PostParticulas([FromBody]Particulas _Particulas)
        {
            if (ModelState.IsValid)            
            {
                var insert = _prtModel.InsertIParticulas(_configuration,_Particulas);

                if(insert==true)
                {
                   
                    log.Debug("Post do Apontamento com sucesso:" + _Particulas);  
                    return Json(_Particulas);
                }
                return StatusCode(500,"Houve um erro, verifique o Log do sistema!");
            }
            else 
                log.Debug("Post não efetuado, Bad Request" + ModelState.ToString());  
                return BadRequest(ModelState);
        }

        [HttpPost]
        public IActionResult PostMedicoes([FromBody]List <ParticulasMedicoes> _medicoes)
        {
            if (ModelState.IsValid)            
            {
                foreach(var item in _medicoes)
                {
                    _prtModel.InsertMedicaoParticulas(_configuration,item); 
                }       
                                                         
                return Json(_medicoes);               
                
            }
            else 
                log.Debug("Post não efetuado, Bad Request" + ModelState.ToString());  
                return BadRequest(ModelState);
        }

        [HttpPost]
        public IActionResult PostMedicoesTamParticulas([FromBody]List <ParticulasTam> _medicoes)
        {
            if (ModelState.IsValid)            
            {
                foreach(var item in _medicoes)
                {
                    _prtModel.InsertParticulasTam(_configuration,item); 
                }       
                                                         
                return Json(_medicoes);               
                
            }
            else 
                log.Debug("Post não efetuado, Bad Request" + ModelState.ToString());  
                return BadRequest(ModelState);
        }

        [HttpPut]
        public IActionResult PutParticulas([FromBody]Particulas _Particulas)
        {
            if (ModelState.IsValid)            
            {
                var insert = _prtModel.UpdateParticulas(_configuration,_Particulas);

                if(insert==true)
                {
                   
                    log.Debug("Put do Apontamento com sucesso:" + _Particulas);  
                    return Ok();
                }
                return StatusCode(500,"Houve um erro, verifique o Log do sistema!");
            }
            else 
                log.Debug("Put não efetuado, Bad Request" + ModelState.ToString());  
                return BadRequest(ModelState);
        }       

    }
}