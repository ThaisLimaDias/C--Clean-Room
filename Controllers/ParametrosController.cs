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
    public class ParametrosController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ParametrosController));
        private readonly IConfiguration _configuration; 
        ParametrosModel _prtmModel = new ParametrosModel();  
        IEnumerable <Parametros> prtm;  
        public ParametrosController(IConfiguration configuration) 
        {            
            _configuration = configuration;
        } 

        [HttpGet] 
        public IActionResult Index()
        {           
            return Ok(_prtmModel.SelectParametros(_configuration,0,null));            
        }

        //Get api/GetParametros
        [HttpGet]       
        public IActionResult  GetParametros(long IdLocalMedicao, string DescParam)
        {
            log.Debug("Get Dos parametros pelo IdLocalMedicao "+ IdLocalMedicao + "E descrição:"+ DescParam);            
            prtm=_prtmModel.SelectParametros(_configuration,IdLocalMedicao,DescParam);

            return Json(prtm);           
            
        }
      
        [HttpPut]
        public IActionResult PutParametros([FromBody]Parametros _parametros)
        {
            if (ModelState.IsValid)            
            {
                var insert = _prtmModel.UpdateParametros(_configuration,_parametros);

                if(insert==true)
                {                   
                    log.Debug("Post do Apontamento com sucesso:" + _parametros);  
                    return Json(_parametros);
                }
                return StatusCode(500,"Houve um erro, verifique o Log do sistema!");
            }
            else 
                log.Debug("Post não efetuado, Bad Request" + ModelState.ToString());  
                return BadRequest(ModelState);
        }
    }
}