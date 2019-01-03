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
        public IActionResult  GetLogin(string codUsuario,string senha)
        {
            if (codUsuario!=null && codUsuario!=null && senha!=null && senha!=null)
            {                  
                log.Debug("Get das permissões por usuario de acordo com seu nível de acesso!");            
                _funcUser=_gruposModel.SelectFuncoesUsuario(_configuration,codUsuario,null);

                if(_funcUser.Count()==0)
                    return StatusCode(505,"O usuário não possui cadastro no sistema verifique!");
                else 
                {
                    if(_funcUser.FirstOrDefault().Senha!=senha)
                        return StatusCode(505,"A senha digitada não confere com a senha cadastrada, digite novamente ou solicite a um admnistrador o reset de sua senha!");
                    else if(_funcUser.FirstOrDefault().Status!="Ativo")
                        return StatusCode(505,"Este usuário está com acesso " + _funcUser.FirstOrDefault().Status + ", consulte um Admnistrador de Sistema!");
                    else
                        return Ok(_funcUser);
                }     
            }
            else
                return StatusCode(505,"Não foi recebido o parametro codUsuario ou a senha!");
        }

        [HttpGet]      
        public IActionResult  GetGrupos(long? IdNivelAcesso,string descNivelAcesso)
        {                        
            log.Debug("Get dos Grupos Existentes no sistema que podem ser editáveis!");            
            _grupos=_gruposModel.SelectGrupos(_configuration,IdNivelAcesso,"Ativo",descNivelAcesso);
                            
            return Ok(_grupos);           
        }

        [HttpGet]      
        public IActionResult  GetPermissoes()
        {                        
            log.Debug("Get das Funções Existentes no sistema que podem ter seu acesso liberado!");            
            _funcSist=_gruposModel.SelectFuncoesSistema(_configuration);
                            
            return Ok(_funcSist);           
        }

               [HttpPost]
        public IActionResult PostGrupos([FromBody]GruposAcesso _grupo)
        {
            if (ModelState.IsValid)            
            {
                var existis = _gruposModel.SelectGrupos(_configuration,null,null,_grupo.DescNivelAcesso);
                if(existis.Count()>0)
                    return StatusCode(505,"Cadastro não efetuado! Já existe um Grupo de Acesso com esta Descrição!");
                else
                {                    
                    var insert = _gruposModel.InsertGrupoAcesso(_configuration,_grupo);      

                    if (insert==true){
                        foreach(var item in _grupo.PermissoesLiberadas)
                        {
                            item.IdNivelAcesso = _grupo.IdNivelAcesso;
                            _gruposModel.InsertGrupoFuncoes(_configuration,item);

                        }
                        return Ok(_grupo);               
                    }
                    else
                        return StatusCode(505,"Houve um erro, verifique o Log do sistema!");
                }
                                                                
            }
            else 
                log.Debug("Post não efetuado, Bad Request" + ModelState.ToString());  
                return BadRequest(ModelState);
        } 

        [HttpPost]
        public IActionResult PostGrupoFuncoes([FromBody]List<GrupoFuncoes> _func)
        {       
            if (ModelState.IsValid)            
            {
                foreach(var item in _func)
                {
                    _gruposModel.InsertGrupoFuncoes(_configuration,item); 
                }                                                                
                return Ok(_func);    
            }
            else 
                log.Debug("Post não efetuado, Bad Request" + ModelState.ToString());  
                return BadRequest(ModelState);

        } 

        [HttpPut]
        public IActionResult PutDesativarGrupo([FromBody]GruposAcesso _grupo)
        {   
            _grupo.Status = "Inativo";
            var inativo = _gruposModel.UpdateGrupos(_configuration,_grupo);

            var users = _gruposModel.SelectFuncoesUsuario(_configuration,null,_grupo.IdNivelAcesso);
            if(users.Count()>0){
                var usuariosDesativar = users.Select(p=>p.CodUsuario).Distinct();

                UsuarioModel _userModel = new UsuarioModel();

                foreach(var item in usuariosDesativar)
                {
                    _userModel.DeleteUsuario(_configuration,item);
                }
            }
            return Ok("O grupo e os usuários pertencentes a este grupo foram desativados com sucesso!");
                             
        }

        [HttpPut]
        public IActionResult PutDeletarFuncao([FromBody]List<GrupoFuncoes> _func)
        {       
            if (ModelState.IsValid)            
            {    
                foreach(var item in _func)
                {    
                    var del = _gruposModel.DeleteGrupoFuncoes(_configuration,item);
                    if (del == true)
                    {
                        log.Debug("Put De Deletar Função Com Sucesso  "+ item);  
                        
                    }
                }
                return Ok();
            }
            else
                log.Debug("Post não efetuado, Bad Request" + ModelState.ToString());  
                return BadRequest(ModelState);;
                     
        }

    }
}