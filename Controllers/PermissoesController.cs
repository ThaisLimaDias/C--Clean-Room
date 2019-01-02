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
    public class PermissoesController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(PermissoesController));
        private readonly IConfiguration _configuration; 
        PermissoesModel _permissaoModel = new PermissoesModel();    
        IEnumerable<Permissoes> _permissoes;

        public PermissoesController(IConfiguration configuration) 
        {            
            _configuration = configuration;
        } 

    
        [HttpGet]      
        public IActionResult  GetPermissoes(string codUsuario)
        {
            if (codUsuario!=null && codUsuario!=null)
            {                  
                log.Debug("Get Dos Apontamentos de Permissoes para Report!");            
                _permissoes=_permissaoModel.SelectPermissoes(_configuration,codUsuario);
                              
                return Ok(_permissoes);
            }
            else
                return StatusCode(505,"Não foi recebido o parametro codUsuario!");
        }

    }
}