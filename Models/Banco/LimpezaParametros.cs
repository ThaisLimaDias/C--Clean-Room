using System;
using System.Collections.Generic;
using System.Reflection;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Embraer_Backend.Models;

namespace Embraer_Backend.Models
{
    public class LimpezaParametros
    {
        public long IdCadParametroLimpeza{get;set;}
        public string DescOQue {get;set;}
        public string DescMetodoLimpeza {get;set;}

        public string TipoControle {get;set;}

        public DateTime DtUltAtlz {get;set;}

    }
    public class LimpezaParametrosModel
    {        
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(LimpezaParametrosModel));
        public IEnumerable<LimpezaParametros> SelectParametros(IConfiguration _configuration,string TipoControle)
        {
            try
            {
                string sSql = string.Empty;
                sSql = "SELECT IdCadParametroLimpeza,DescOQue,DescMetodoLimpeza,TipoControle,DtUltAtlz";
                sSql = sSql + " FROM TB_CADASTRO_PARAMETROS_LIMPEZA";
                sSql = sSql + " WHERE  TipoControle='" + TipoControle + "'";


                IEnumerable <LimpezaParametros> parametros;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
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
    }

}