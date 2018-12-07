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

        public IEnumerable<Grupos> SelectGrupos(IConfiguration _configuration)
        {            
                try
                {
                    string sSql = string.Empty;

                    sSql = "SELECT IdFuncaoSistema,DescFuncaoSistema FROM TB_TELAS_SISTEMAS"; 
                    
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

        public IEnumerable<Grupos> SelectGruposLiberados(IConfiguration _configuration, Grupos _grupos)
        {            
                try
                {
                    string sSql = string.Empty;

                    sSql = "SELECT IdFuncaoSistema,DescFuncaoSistema,IdFuncoesLiberadas FROM TB_TELAS_LIBERADAS_NIVEL_ACESSO WHERE IdNivelAcesso='"+_grupos.IdNivelAcesso+"'"; 
                    
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

        public (bool,string) UpdateGrupos (IConfiguration _configuration,Grupos _grupos)
        {
            try{
                string sSql = string.Empty;

                sSql = "UPDATE TB_NIVEL_ACESSO SET";
                sSql=sSql="[DescFuncaoSistema]='"+ _grupos.DescFuncaoSistema + "'";
                sSql = "WHERE IdNivelAcesso=" + _grupos.IdNivelAcesso;

                int update = 0;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                    update = db.Execute(sSql,commandTimeout:0);
                }
                if(update<=0)
                {
                    return (false, string.Empty);
                }
                return (true,string.Empty);

            }
            catch(Exception ex)
            {
                log.Error("Erro GruposModel-UpdateGrupos:" + ex.Message.ToString());
                return (false,"UpdateGrupos: Ocorreu um erro ao atualizar informações no banco de dados");
            }
        }
        public (bool,string) InsertGrupos(IConfiguration _configuration,Grupos _grupos)
        {
            
            string sSql = string.Empty;
            try
            {
                sSql= "INSERT INTO TB_TELAS_LIBERADAS_NIVEL_ACESSO ([IdFuncaoSistema],[IdNivelAcesso])";
                sSql = "VALUES";
                sSql = "('" + _grupos.IdFuncaoSistema + "'";
                sSql = ",'" + _grupos.IdNivelAcesso + "')";
                sSql = sSql + "SELECT @@IDENTITY";

                long insertId = 0;

                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                    insertId =db.Query<long>(sSql).GetEnumerator().Current;
                }
                if(insertId>=0)
                {
                    _grupos.IdFuncoesLiberadas=insertId;
                    return (true, string.Empty);
                }
                return (false,string.Empty);
            }
            
            catch(Exception ex)
            {
                log.Error("Erro GruposModel-InsertGrupos:" + ex.Message.ToString());
                return (false,"InsertGrupos: Ocorreu um erro ao inserir informações no banco de dados");
            }
        }

    public IEnumerable<Grupos> DeleteFuncao(IConfiguration _configuration,Grupos _grupos)
        {            
                try
                {
                    string sSql = string.Empty;

                    sSql = "DELETE FROM TB_TELAS_LIBERADAS_NIVEL_ACESSO WHERE IdFuncaoLiberada='" + _grupos.IdFuncoesLiberadas+"'"; 
                    
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

        public (bool,string) DeleteGrupos (IConfiguration _configuration,Grupos _grupos)
        {            
                try{
                string sSql = string.Empty;

                sSql = "UPDATE TB_NIVEL_ACESSO SET";
                sSql=sSql="[Status]='"+ _grupos.Status + "'";
                sSql = "WHERE IdNivelAcesso=" + _grupos.IdNivelAcesso;

                int update = 0;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                    update = db.Execute(sSql,commandTimeout:0);
                }
                if(update<=0)
                {
                    return (false, string.Empty);
                }
                return (true,string.Empty);

            }
            catch(Exception ex)
            {
                log.Error("Erro GruposModel-UpdateGrupos:" + ex.Message.ToString());
                return (false,"UpdateGrupos: Ocorreu um erro ao atualizar informações no banco de dados");
            }
        }     

    }

}