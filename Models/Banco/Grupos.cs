using System;
using System.Collections.Generic;
using System.Reflection;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace C_Embraer_Clean_Room.Models.Banco
{
    public class Grupos
    {
        public long IdFuncaoSistema { get; set; }

        public long IdFuncoesLiberadas { get; set; }

        public long IdNivelAcesso { get; set; }

        public string DescFuncaoSistema { get; set; } 

        public string Status { get; set; } 

    }

    public class GruposModel{
        
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(GruposModel));

        //Select telas do sistema
        public IEnumerable<Grupos> SelectTelasSistema(IConfiguration _configuration)
        {            
                try
                {
                    string sSql = string.Empty;

                    sSql = "SELECT IdFuncaoSistema,DescFuncaoSistema FROM TB_TELAS_SISTEMA"; 
                    
                    IEnumerable<Grupos> grupos;
         
                    using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                    {
                        grupos = db.Query<Grupos>(sSql,commandTimeout:0);
                    }

                    return grupos;
                }
                catch (Exception ex)
                {
                    log.Error("Erro GruposModel-SelectGrupo:" + ex.Message.ToString());
                    return null;
                }
        }


        public IEnumerable<Grupos> SelectGruposLiberados(IConfiguration _configuration, long IdGrupos)
        {            
                try
                {
                    string sSql = string.Empty;

                    sSql = "SELECT IdFuncaoSistema,IdFuncoesLiberadas FROM TB_TELAS_LIBERADAS_NIVEL_ACESSO WHERE IdNivelAcesso="+IdGrupos; 
                    
                    IEnumerable<Grupos> grupos;
         
                    using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                    {
                        grupos = db.Query<Grupos>(sSql,commandTimeout:0);
                    }

                    return grupos;
                }
                catch (Exception ex)
                {
                    log.Error("Erro GruposModel-SelectGruposLiberados:" + ex.Message.ToString());
                    return null;
                }
        }

        public bool UpdateGrupos (IConfiguration _configuration, long IdAcesso, String DescFunc)
        {
            try{
                string sSql = string.Empty;

                sSql = "UPDATE TB_NIVEL_ACESSO SET";
                sSql+="[DescNivelAcesso]='"+ DescFunc + "'";
                sSql+= "WHERE IdNivelAcesso=" + IdAcesso;

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
        public bool InsertGrupos(IConfiguration _configuration, long IdFuncao, long IdAcesso)
        {
            
            string sSql = string.Empty;
            try
            {
                sSql = "INSERT INTO TB_TELAS_LIBERADAS_NIVEL_ACESSO ([IdFuncaoSistema],[IdNivelAcesso])";
                sSql+= "VALUES";
                sSql+= "(" + IdFuncao;
                sSql+= "," + IdAcesso + ")";
                sSql+= "SELECT @@IDENTITY";

                long insertId = 0;

                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                    insertId=db.Query<long>(sSql).GetEnumerator().Current;
                }
                if(insertId>=0)
                {
                    return (true);
                }
                return (false);
            }
            
            catch(Exception ex)
            {
                log.Error("Erro GruposModel-InsertGrupos:" + ex.Message.ToString());
                return (false);
            }
        }

        public IEnumerable<Grupos> DeleteFuncao(IConfiguration _configuration,long IdFuncaoLiberada)
        {            
                try
                {
                    string sSql = string.Empty;

                    sSql = "DELETE FROM TB_TELAS_LIBERADAS_NIVEL_ACESSO WHERE IdFuncaoLiberada=" + IdFuncaoLiberada; 
                    
                    IEnumerable<Grupos> grupos;
         
                    using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                    {
                        grupos = db.Query<Grupos>(sSql,commandTimeout:0);
                    }

                    return grupos;
                }
                catch (Exception ex)
                {
                    log.Error("Erro GruposModel-DeleteFuncao:" + ex.Message.ToString());
                    return null;
                }
        }   

        public bool DeleteGrupos (IConfiguration _configuration,long IdAcesso,String Status)
        {            
                try{
                string sSql = string.Empty;

                sSql = "UPDATE TB_NIVEL_ACESSO SET";
                sSql+="[Status]='"+ Status + "'";
                sSql+= " WHERE IdNivelAcesso=" + IdAcesso;

                int update = 0;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                    update = db.Execute(sSql,commandTimeout:0);
                }
                if(update<=0)
                {
                    return (false);
                }

                try{
                sSql = string.Empty;

                sSql = "UPDATE TB_USUARIO SET";
                sSql+="[Status]='BLOQUEADO'";
                sSql+= "WHERE IdNivelAcesso=" + IdAcesso;

                update = 0;
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
            catch(Exception ex)
            {
                log.Error("Erro GruposModel-UpdateGrupos:" + ex.Message.ToString());
                return (false);
            }

        }     

    }

}