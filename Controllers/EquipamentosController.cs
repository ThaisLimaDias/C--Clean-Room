using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Embraer_Backend.Models;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.AspNetCore.Cors;
using System.Linq;
using System.Threading.Tasks;

namespace Embraer_Backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [EnableCors("CorsPolicy")]
    public class EquipamentosController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(DashboardSLController));
        private readonly IConfiguration _configuration;

        EquipamentosModel _equip = new EquipamentosModel();

        public DateDiferenceModel _funcDate = new DateDiferenceModel();
        public EquipamentosController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet()]
        public IActionResult GetEquipamentos()
        {
            try
            {
                var equip = _equip.SelectEquipamentos(_configuration, null);


                if (equip.Count() > 0)
                    return Ok(equip);
                else
                    return NotFound("Equipamento não encontrado");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetEquipamentos(string cod)
        {
            try
            {
                var equip = _equip.SelectEquipamentos(_configuration, cod);

                if (equip != null)
                    return Ok(equip);
                else
                    return NotFound("ERRO! Equipamento não encontrado!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        public IActionResult PutEquipamento([FromBody] Equipamentos equip)
        {
            try
            {
                var equipDb = _equip.UpdateEquipamentos(_configuration, equip);

                if (equipDb != null)
                {
                    return Ok(equipDb);
                }
                else
                {
                    return StatusCode(500, "Erro ao tentar atualizar o equipamento");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult PostEquipamentos([FromBody]Equipamentos equip)
        {
            if (equip.CodEquip != null)
            {
                var existis = _equip.SelectEquipamentos(_configuration, equip.CodEquip);
                if (existis == null)
                    return StatusCode(505, "ERRO! Equipamento já cadastrado! " + equip.CodEquip);

                else
                {
                    var insert = _equip.InsertEquipamentos(_configuration, equip);

                    if (insert == true)
                        return Ok("Equipamento " + equip.CodEquip.ToString() + " cadastrado com sucesso!");

                    else
                        return StatusCode(505, "Houve um erro, verifique o Log do sistema!");
                }
            }
            else
                log.Debug("Post não efetuado, Bad Request" + ModelState.ToString());
            return BadRequest(ModelState);

        }
    }
}