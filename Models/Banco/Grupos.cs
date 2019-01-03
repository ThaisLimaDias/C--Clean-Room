using System;
using System.Collections.Generic;
using System.Reflection;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace C_Embraer_Clean_Room.Models.Banco
{
    public class GruposAcesso
    {
        public long IdNivelAcesso { get; set; }

        public string DescNivelAcesso { get; set; }

        public string Status { get; set; }

        public IEnumerable <GrupoFuncoes> PermissoesLiberadas {get;set;}

    }
    public class GrupoFuncoes
    {
        public long? IdFuncoesLiberadas {get;set;}
        public long IdFuncaoSistema{get;set;}
        public string DescFuncaoSistema {get;set;}
        public long IdNivelAcesso {get;set;}
    }
    public class FuncoesUsuario
    {
        public string CodUsuario {get;set;}   
        public string Funcao {get;set;}
        public string NumChapa {get;set;}
        public string Nome {get;set;}
        public string Senha {get;set;}
        public string Status {get;set;}
        public long IdFuncaoSistema{get;set;}
        public string DescFuncaoSistema{get;set;}
    }
    public class FuncoesSistema
    { 
        public long IdFuncaoSistema{get;set;}
        public string DescFuncaoSistema{get;set;}
    }
    

    public class GruposModel
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(GruposModel));
        public IEnumerable<FuncoesUsuario> SelectFuncoesUsuario(IConfiguration _configuration, string CodUsuario,long? IdNivelAcesso)
        {            
            try
            {
                string sSql = string.Empty;

                sSql = "SELECT CodUsuario,Funcao,NumChapa,Nome,Status,Senha,IdFuncaoSistema,DescFuncaoSistema";
                sSql += " FROM VW_PERMISSOES_USUARIO";
                sSql += " WHERE 1=1";

                if(CodUsuario!=null && CodUsuario!="")
                    sSql+= " AND CodUsuario='" + CodUsuario + "'";

                if(IdNivelAcesso!=null)
                    sSql+= " AND IdNivelAcesso='" + IdNivelAcesso + "'";
                
                IEnumerable <FuncoesUsuario> permissoes;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                    permissoes = db.Query<FuncoesUsuario>(sSql,commandTimeout:0);
                }                 
                return permissoes;
            }
            catch (Exception ex)
            {
                log.Error("Erro GruposModel-SelectFuncoesUsuario:" + ex.Message.ToString());
                return null;
            }
        }
        public IEnumerable<FuncoesSistema> SelectFuncoesSistema(IConfiguration _configuration)
        {            
            try
            {
                string sSql = string.Empty;

                sSql = "SELECT IdFuncaoSistema,DescFuncaoSistema FROM TB_TELAS_SISTEMA";
                
                IEnumerable <FuncoesSistema> permissoes;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                    permissoes = db.Query<FuncoesSistema>(sSql,commandTimeout:0);
                }                 
                return permissoes;
            }
            catch (Exception ex)
            {
                log.Error("Erro GruposModel-SelectFuncoesSistema:" + ex.Message.ToString());
                return null;
            }
        }
        public IEnumerable<GruposAcesso> SelectGrupos(IConfiguration _configuration,long? IdNivelAcesso, string status,string descNivelAcesso)
        {            
            try
            {
                string sSql = string.Empty;
                string sSql2 = string.Empty;

                sSql = "SELECT IdNivelAcesso,DescNivelAcesso,Status FROM TB_NIVEL_ACESSO WHERE 1=1";

                if(IdNivelAcesso!=null)
                    sSql+= "AND IdNivelAcesso=" + IdNivelAcesso;

                if(status!=null && status!="")
                    sSql+= "AND Status='" + status + "'";

                if(descNivelAcesso!=null && descNivelAcesso!="")
                    sSql+= "AND DescNivelAcesso='" + descNivelAcesso + "'";
                
                IEnumerable <GruposAcesso> _grupo;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                    _grupo = db.Query<GruposAcesso>(sSql,commandTimeout:0);
                }
                //Para Setar permissoes do Grupo
                
                foreach(var item in _grupo)
                {
                    sSql2 = "SELECT IdFuncoesLiberadas,A.IdFuncaoSistema,IdNivelAcesso,DescFuncaoSistema";
                    sSql2 += " FROM TB_TELAS_LIBERADAS_NIVEL_ACESSO A";
                    sSql2 += " INNER JOIN TB_TELAS_SISTEMA T ON A.IdFuncaoSistema=T.IdFuncaoSistema";
                    sSql2 += " WHERE IdNivelAcesso=" + item.IdNivelAcesso;
                    IEnumerable <GrupoFuncoes> _funcoes;
                    using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                    {
                        _funcoes = db.Query<GrupoFuncoes>(sSql2,commandTimeout:0);
                    }
                    item.PermissoesLiberadas = _funcoes;
                }
                return _grupo;
            }
            catch (Exception ex)
            {
                log.Error("Erro GruposModel-SelectGrupos:" + ex.Message.ToString());
                return null;
            }
        }
        public bool UpdateGrupos (IConfiguration _configuration,GruposAcesso _grupo)
        {
            try{
                string sSql = string.Empty;

                sSql = "UPDATE TB_NIVEL_ACESSO SET ";
                sSql+="[DescNivelAcesso]='"+ _grupo.DescNivelAcesso + "'";
                sSql+=",[Status]='"+ _grupo.Status + "'";
                sSql+= " WHERE IdNivelAcesso=" + _grupo.IdNivelAcesso;

                int update = 0;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                    update = db.Execute(sSql,commandTimeout:0);
                }
                if(update<=0)
                {
                    return (false);
                }
                return (true);

            }
            catch(Exception ex)
            {
                log.Error("Erro GruposModel-UpdateGrupos:" + ex.Message.ToString());
                return (false);
            }
        }
        public bool InsertGrupoAcesso(IConfiguration _configuration, GruposAcesso _grupos)
        {
            
            string sSql = string.Empty;
            try
            {
                sSql = "INSERT INTO TB_NIVEL_ACESSO ([DescNivelAcesso],[Status])";
                sSql+= "VALUES ";
                sSql+= "('" + _grupos.DescNivelAcesso + "'";
                sSql+= ",'Ativo')";
                sSql+= "SELECT @@IDENTITY";

                long insertId = 0;

                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                    insertId=db.QueryFirstOrDefault<long>(sSql,commandTimeout:0);
                }
                if(insertId>0)
                {
                    _grupos.IdNivelAcesso= insertId;
                    return (true);
                }
                return (false);
            }
            
            catch(Exception ex)
            {
                log.Error("Erro GruposModel-InsertGrupoFuncoes:" + ex.Message.ToString());
                return (false);
            }
        }
        public bool InsertGrupoFuncoes(IConfiguration _configuration, GrupoFuncoes _funcoes)
        {
            
            string sSql = string.Empty;
            try
            {
                sSql = "INSERT INTO TB_TELAS_LIBERADAS_NIVEL_ACESSO ([IdFuncaoSistema],[IdNivelAcesso])";
                sSql+= "VALUES";
                sSql+= "(" + _funcoes.IdFuncaoSistema;
                sSql+= "," + _funcoes.IdNivelAcesso + ")";
                sSql+= "SELECT @@IDENTITY";

                long insertId = 0;

                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                    insertId=db.QueryFirstOrDefault<long>(sSql,commandTimeout:0);
                }
                if(insertId>=0)
                {
                    _funcoes.IdFuncoesLiberadas=insertId;
                    return (true);
                }
                return (false);
            }
            
            catch(Exception ex)
            {
                log.Error("Erro GruposModel-InsertGrupoFuncoes:" + ex.Message.ToString());
                return (false);
            }
        }

        public bool DeleteGrupoFuncoes(IConfiguration _configuration,GrupoFuncoes _funcoes)
        {            
                try
                {
                    string sSql = string.Empty;

                    sSql = "DELETE FROM TB_TELAS_LIBERADAS_NIVEL_ACESSO WHERE IdFuncaoSistema=" + _funcoes.IdFuncaoSistema + " AND idNivelAcesso=" + _funcoes.IdNivelAcesso; 
                    
                    IEnumerable<GruposAcesso> grupos;
         
                    using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                    {
                        grupos = db.Query<GruposAcesso>(sSql,commandTimeout:0);
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    log.Error("Erro GruposModel-DeleteGrupoFuncoes:" + ex.Message.ToString());
                    return false;
                }
        }   

    }

}