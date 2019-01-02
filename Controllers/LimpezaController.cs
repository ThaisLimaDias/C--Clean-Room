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
    public class LimpezaController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(LimpezaController));
        private readonly IConfiguration _configuration; 
        LimpezaModel _lpzModel = new LimpezaModel();     
        LimpezaMedicoesModel _limpezaMedicoesModel = new LimpezaMedicoesModel();
        LimpezaParametrosModel _parametrosModel = new LimpezaParametrosModel();

        LocalMedicaoModel _localMedicaoModel = new LocalMedicaoModel();

        IEnumerable<LimpezaApontamento> _apontamentos;
        LimpezaApontamento _apontamento = new LimpezaApontamento();

        IEnumerable<LimpezaParametros> _parametrosLimpeza;
        
        LimpezaIndex _limpezaIndex = new LimpezaIndex();
        
        public LimpezaController(IConfiguration configuration) 
        {            
            _configuration = configuration;
        }
        
        [HttpGet] 
        public IActionResult Index()
        {
            //Verifica se existe um apontamento pendente caso contrário carrega a tela para iniciar um novo apontamento.
            _apontamentos=_lpzModel.SelectLimpeza(_configuration, null, null, null,"Apontado");
            if(_apontamentos.Count()>0)
            {
                return Ok(_apontamentos);
            }
            //Carrega tela para novo apontamento
            else
            {
                _limpezaIndex.Local = _localMedicaoModel.SelectLocalMedicao(_configuration);
                _limpezaIndex.TipoControle.Add("DIARIO");
                _limpezaIndex.TipoControle.Add("SEMANAL");
                _limpezaIndex.TipoControle.Add("QUINZENAL");
                _limpezaIndex.TipoControle.Add("MENSAL");
                _limpezaIndex.TipoControle.Add("SEMESTRAL");

                return Ok(_limpezaIndex);
            }
            
        }


        //Get api/Limpeza
        [HttpGet]       
        public IEnumerable<LimpezaApontamento>  GetLimpeza(long id,string status,string Ini, string Fim)
        {
            if (id!=0 || (Ini!=null & Fim!=null) || status!=null )
            {

                log.Debug("Get Dos Apontamentos de Limpeza!");            
                _apontamentos=_lpzModel.SelectLimpeza(_configuration, id, Ini, Fim,status);
            }
            return (_apontamentos);
        }

        [HttpPost]
        public IActionResult PostLimpeza([FromBody]Limpeza _limpeza)
        {
            if (ModelState.IsValid)            
            {
                var insert = _lpzModel.InsertLimpeza(_configuration,_limpeza);
                _parametrosLimpeza =  _parametrosModel.SelectParametros( _configuration,_limpeza.TipoControle);

                if(insert==true)
                {
                    //Adiciona os parametros ao Apontamento
                    foreach(var item in _parametrosLimpeza)
                    {
                        LimpezaMedicoes md = new LimpezaMedicoes();
                        md.IdApontLimpeza = _limpeza.IdApontLimpeza;
                        md.Realizado = false;
                        md.IdCadParametroLimpeza = item.IdCadParametroLimpeza;
                        md.DescOQue = item.DescOQue;
                        md.DescMetodoLimpeza = item.DescMetodoLimpeza;
                        
                        _limpezaMedicoesModel.InsertLimpeza(_configuration,md); 
                    }
                    _apontamentos=_lpzModel.SelectLimpeza(_configuration, _limpeza.IdApontLimpeza, null, null,null);
                    log.Debug("Post do Apontamento com sucesso:" + _apontamento);  
                    return Json(_apontamentos);
                }
                log.Debug("Post do Apontamento com sucesso:" + _apontamento);  
                return StatusCode(500,"Houve um erro, verifique o Log do sistema!");
            }
            else 
                log.Debug("Post não efetuado, Bad Requuest"+ ModelState.ToString());  
                return BadRequest(ModelState);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public IActionResult PutLimpeza([FromBody]List<Limpeza> _limpeza)
        {
            if (ModelState.IsValid)  
            {     
                foreach(Limpeza item in _limpeza)      
                    _lpzModel.UpdateLimpeza(_configuration,item);                  
                
               log.Debug("Put com sucesso");  
                return Ok();
               
            }
            log.Debug("Put de Limpeza não efetuado, Bad Request"+ ModelState.ToString());              
            return BadRequest(ModelState);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public IActionResult PutMedicaoLimpeza([FromBody]List<LimpezaMedicoes> _medicoes)
        {
            if (ModelState.IsValid)  
            {     
                foreach(var item in _medicoes)                
                    _limpezaMedicoesModel.UpdateLimpezaMedicoes(_configuration,item);              
                
               log.Debug("Put com sucesso");  
                return Ok();
               
            }
            log.Debug("Put de Limpeza não efetuado, Bad Request" + ModelState.ToString());              
            return BadRequest(ModelState);
        }
        
    
    }
}
