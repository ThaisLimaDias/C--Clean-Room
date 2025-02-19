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
    public class ControleApontamentoController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ControleApontamentoController));
         private readonly IConfiguration _configuration;
        
        IEnumerable<ControleApontamento> _ctrl;
        ControleApontamentoModel _ctrlModel= new ControleApontamentoModel();

        public ControleApontamentoController(IConfiguration configuration) 
        {            
            _configuration = configuration;
        }

        [HttpPut]
        public IActionResult PutParametros([FromBody]ControleApontamento _ctrls)
        {
            if (ModelState.IsValid)            
            {
                var insert = _ctrlModel.UpdateControleApontamento(_configuration,_ctrls);

                if(insert==true)
                {                   
                    log.Debug("Post do Apontamento com sucesso:" + _ctrls);  
                    return Json(_ctrls);
                }
                return StatusCode(500,"Houve um erro, verifique o Log do sistema!");
            }
            else 
                log.Debug("Post não efetuado, Bad Request" + ModelState.ToString());  
                return BadRequest(ModelState);
        }       
        
    }
}