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
        IEnumerable <GruposAcesso> _grupos;
        IEnumerable <FuncoesSistema> _funcoes;
        IEnumerable <FuncoesUsuario> _funcUser;
        IEnumerable <FuncoesSistema> _funcSist;


        public GruposController(IConfiguration configuration) 
        {            
            _configuration = configuration;
        }
        [HttpGet]      
        public IActionResult  GetPermissoesUsuario(string codUsuario)
        {
            if (codUsuario!=null && codUsuario!=null)
            {                  
                log.Debug("Get das permissões por usuario de acordo com seu nível de acesso!");            
                _funcUser=_gruposModel.SelectFuncoesUsuario(_configuration,codUsuario);
                              
                return Ok(_funcUser);
            }
            else
                return StatusCode(505,"Não foi recebido o parametro codUsuario!");
        }

        [HttpGet]      
        public IActionResult  GetPermissoes()
        {                        
            log.Debug("Get das Funções Existentes no sistema que podem ter seu acesso liberado!");            
            _funcSist=_gruposModel.SelectFuncoesSistema(_configuration);
                            
            return Ok(_funcSist);           
        }

        [HttpPost]
        public IActionResult PutGrupoFuncoes([FromBody]List<GrupoFuncoes> _grupo)
        {       
               if (ModelState.IsValid)            
            {
                foreach(var item in _grupo)
                {
                    _gruposModel.InsertGrupoFuncoes(_configuration,item); 
                }                                                                
                return Ok();    
            }
            else 
                log.Debug("Post não efetuado, Bad Request" + ModelState.ToString());  
                return BadRequest(ModelState);

        } 

        [HttpPut]
        public IActionResult PutDesativarGrupo(GruposAcesso _grupo)
        {   
            _grupo.Status = "Inativo";
            var inativo = _gruposModel.UpdateGrupos(_configuration,_grupo);

            var users = _gruposModel.SelectFuncoesUsuario(_configuration,null);

            var usuariosDesativar = users.Select(p=>p.CodUsuario).Distinct();

            UsuarioModel _userModel = new UsuarioModel();

            foreach(var item in usuariosDesativar)
            {
                var delete = _userModel.DeleteUsuario(_configuration,item);
            }

            return Ok("O grupo e os usuários pertencentes a este grupo foram desativados com sucesso!");
                             
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