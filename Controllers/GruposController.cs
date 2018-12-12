using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Embraer_Backend.Models;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.AspNetCore.Cors;
using System.Linq;
using C_Embraer_Clean_Room.Models.Banco;

//namespace C_Embraer_Clean_Room.Controllers
namespace Embraer_Backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [EnableCors("CorsPolicy")]
    public class GruposController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(GruposController));
        private readonly IConfiguration _configuration; 
        GruposModel _gruposModel = new GruposModel(); 
        IEnumerable <Grupos> _grupos;

        public GruposController(IConfiguration configuration) 
        {            
            _configuration = configuration;
        }

        [HttpGet]      
        public IActionResult  GetTelasSistema()
        {              
                log.Debug("Get Dos Grupos!");            
                _grupos=_gruposModel.SelectTelasSistema(_configuration);

                return Ok(_grupos);
        }

     [HttpGet]      
        public IActionResult  GetGruposLiberados(long IdGrupos)
        {
            if (IdGrupos!=0)
            {                 
                log.Debug("Get Dos Grupos Liberados!");            
               IEnumerable<Grupos> _grupos=_gruposModel.SelectGruposLiberados(_configuration,IdGrupos);

                return Ok(_grupos);
            }
            else
                return StatusCode(505,"Não foi recebido o parametro Grupos");
        }

        [HttpPut]
        public IActionResult PutGrupos(long IdAcesso, String DescFunc)
        {       
                var insert = false;

                if(IdAcesso!=0 && DescFunc!=null){
                log.Debug("Put Dos Grupos Liberados!");            

                insert = _gruposModel.UpdateGrupos(_configuration,IdAcesso,DescFunc);

                if(insert==true)
                {
                    log.Debug("Put Grupos alterado com sucesso");  
                    return Ok();
                }
                return StatusCode(500,"Houve um erro, verifique o Log do sistema!");

                }else{
                    return StatusCode(505,"Não foi recebido o parametro IdAcesso ou DescFunc");
                }

        } 

        [HttpPut]
        public IActionResult PutDesativarGrupo(long IdAcesso, String Status)
        {       
                var insert = false;

                if(IdAcesso!=0 && Status!=null){
                log.Debug("Put Dos Grupos Liberados!");            

                insert = _gruposModel.DeleteGrupos(_configuration,IdAcesso,Status);

                if(insert==true)
                {
                    log.Debug("Put Grupos alterado com sucesso");  
                    return Ok();
                }
                return StatusCode(500,"Houve um erro, verifique o Log do sistema!");

                }else{
                    return StatusCode(505,"Não foi recebido o parametro IdAcesso ou DescFunc");
                }
                     
        }

        [HttpPut]
        public IActionResult PutDeletarFuncao(long IdFuncaoLiberada)
        {       

                if(IdFuncaoLiberada!=0)
                {
                    log.Debug("Put De Deletar Função!");            

                    var del = _gruposModel.DeleteFuncao(_configuration,IdFuncaoLiberada);
                    if (del == true)
                    {
                        log.Debug("Put De Deletar Função Deletou Com Sucesso");  
                        return Ok();
                    }

                    else
                        return StatusCode(500,"Houve um erro, verifique o Log do sistema!");
                }
                else
                    return StatusCode(505,"Não foi recebido o parametro IdFuncaoLiberada");
                     
        }

        [HttpPost]
        public IActionResult PostGrupos(long IdFuncao, long IdAcesso)
        {
            if (IdFuncao!=0 && IdAcesso!=0)            
            {
               
                var insert = _gruposModel.InsertGrupos(_configuration,IdFuncao, IdAcesso);      

                if (insert==true){
                    return Ok();               
                }else{
                    return StatusCode(505,"Houve um erro, verifique o Log do sistema!");
                }                                                
            }
            else 
                log.Debug("Post não efetuado, Bad Request" + ModelState.ToString());  
                return BadRequest(ModelState);
        } 

    }
}