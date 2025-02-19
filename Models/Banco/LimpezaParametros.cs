using System;
using System.Collections.Generic;
using System.Reflection;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ProjectCleanning_Backend.Models;

namespace ProjectCleanning_Backend.Models
{
    public class LimpezaParametros
    {
        public long IdCadParametroLimpeza{get;set;}
        public string DescOQue {get;set;}
        public string DescMetodoLimpeza {get;set;}

        public string TipoControle {get;set;}

        public DateTime?  DtUltAtlz {get;set;}
        public string  Status {get;set;}

    }
    public class LimpezaParametrosModel
    {        
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(LimpezaParametrosModel));
        public IEnumerable<LimpezaParametros> SelectParametros(IConfiguration _configuration,string TipoControle,string Status)
        {
            try
            {
                string sSql = string.Empty;
                sSql = "SELECT IdCadParametroLimpeza,DescOQue,DescMetodoLimpeza,TipoControle,DtUltAtlz,Status";
                sSql += " FROM TB_CADASTRO_PARAMETROS_LIMPEZA";
                sSql += " WHERE 1=1";               

                if(TipoControle!="" && TipoControle!=null)
                    sSql = sSql + " AND TipoControle='" + TipoControle + "'";

                if(Status!="" && Status!=null)
                    sSql = sSql + " AND Status='" + Status +"'";


                IEnumerable <LimpezaParametros> parametros;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_ProjectCleanning_Sala_Limpa")))
                {                    
                    parametros = db.Query<LimpezaParametros>(sSql,commandTimeout:0);
                }
                return parametros;
            }            
            catch (Exception ex)
            {
                log.Error("Erro LimpezaParametrosModel-SelectParametros:" + ex.Message.ToString());
                return null;
            }
        }
        public bool UpdateParametros (IConfiguration _configuration,LimpezaParametros _parametros)
        {
            try{
                string sSql = string.Empty;

                sSql = "UPDATE TB_CADASTRO_PARAMETROS_LIMPEZA SET"; 
                sSql+= " DescOQue='"+ _parametros.DescOQue + "'";
                sSql+= ",DescMetodoLimpeza='"+ _parametros.DescMetodoLimpeza + "'";
                sSql+= ",TipoControle='"+ _parametros.TipoControle + "'";
                sSql+= ",Status='"+ _parametros.Status + "'";
                sSql+=  ",DtUltAtlz=GETDATE()";
                sSql+=  " WHERE IdCadParametroLimpeza=" + _parametros.IdCadParametroLimpeza;

                long update = 0;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_ProjectCleanning_Sala_Limpa")))
                {
                    update = db.Execute(sSql,commandTimeout:0);
                }
                if(update>0)
                {
                    return true;
                }
                return false;

            }
            catch(Exception ex)
            {
                log.Error("Erro LimpezaParametrosModel-UpdateParametros:" + ex.Message.ToString());
                return false;
            }
        }
        public bool InsertParametros(IConfiguration _configuration,LimpezaParametros _prt)
        {
            
            string sSql = string.Empty;
            try
            {
             
                sSql= "INSERT INTO TB_CADASTRO_PARAMETROS_LIMPEZA (DescOQue,DescMetodoLimpeza,TipoControle,DtUltAtlz)";
                sSql +=" VALUES ";
                sSql +="('" + _prt.DescOQue + "'";
                sSql +=",'" + _prt.DescMetodoLimpeza + "'";
                sSql +=",'" + _prt.TipoControle + "'";
                sSql +=", GETDATE())";
                sSql +=" SELECT @@IDENTITY";

                long insertId=0;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_ProjectCleanning_Sala_Limpa")))
                {
                   insertId =db.QueryFirstOrDefault<long>(sSql,commandTimeout:0);
                }
                if(insertId>0)
                {
                    _prt.IdCadParametroLimpeza=insertId;
                    return true;
                }
                return false;
            }
            
            catch(Exception ex)
            {
                log.Error("Erro LimpezaParametrosModel-InsertParametros:" + ex.Message.ToString());
                return false;
            }        
        }
    }

}