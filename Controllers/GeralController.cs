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
    public class GeralController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(GeralController));
         private readonly IConfiguration _configuration;
        IEnumerable<LocalMedicao> _local;
        LocalMedicaoModel _localMedicaoModel= new LocalMedicaoModel();

        IEnumerable<Equipamentos> _equips;
        EquipamentosModel _equipsModel= new EquipamentosModel();

        IEnumerable<ControleApontamento> _ctrl;
        ControleApontamentoModel _ctrlModel= new ControleApontamentoModel();

        public GeralController(IConfiguration configuration) 
        {            
            _configuration = configuration;
        }

        [HttpGet]      
        public IActionResult  LocalMedicao()
        { 
            log.Debug("Get Dos Locais de Apontamento");        
            _local = _localMedicaoModel.SelectLocalMedicao(_configuration);

            if (_local.Count()>0)
                return Ok(_local);
            else 
                return StatusCode(505,"Não foi encotrado nenhum local de Apontamento verifique o log de erros do sistema!");
        }
        
        [HttpGet]      
        public IActionResult  ControleApontamento(string DescApont)
        { 
            log.Debug("Get de quantos pontos de coleta terá um Apontamento.");        
            _ctrl = _ctrlModel.SelectControleApontamento(_configuration,DescApont);

            if (_ctrl.Count()>0)
                return Ok(_ctrl);
            else 
                return StatusCode(505,"Não foi encotrado nenhum controle de apontamento para a descrição informada verifique o log de erros do sistema!");
        }



        [HttpGet]      
        public IActionResult  Equipamentos()
        { 
            log.Debug("Get Dos Locais de Apontamento");        
            _equips= _equipsModel.SelectEquipamentos(_configuration);

            if (_equips.Count()>0)
                return Ok(_equips);
            else 
                return StatusCode(505,"Não foi encotrado nenhum local de Apontamento verifique o log de erros do sistema!");
        }
        
    }
}