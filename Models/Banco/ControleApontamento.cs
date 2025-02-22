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
    public class ControleApontamento
    {
        public long IdControleApont { get; set; }
        public string DescApont { get; set; }
        public int QtdeApont{get;set;}
        public DateTime? ProxApont{get;set;}
        public int DiasProximaMed{get;set;}
    }

   
    public class ControleApontamentoModel
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ControleApontamentoModel));
        
        public IEnumerable<ControleApontamento> SelectControleApontamento(IConfiguration _configuration,string DescApont)
        {            
            try
            {
                string sSql = string.Empty;

                sSql = "SELECT IdControleApont,DescApont,QtdeApont,ProxApont,DiasProximaMed";
                sSql = sSql + " FROM TB_CONTROLE_APONTAMENTO";
                sSql = sSql + " WHERE 1=1";
               
                if(DescApont!="" && DescApont!=null)                
                    sSql = sSql + " AND DescApont='" + DescApont + "'";
         

                IEnumerable <ControleApontamento> _ctrlApont;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_ProjectCleanning_Sala_Limpa")))
                {
                    _ctrlApont = db.Query<ControleApontamento>(sSql,commandTimeout:0);
                }
                return  _ctrlApont;
            }
            catch (Exception ex)
            {
                log.Error("Erro ControleApontamentoModel-SelectControleApontamento:" + ex.Message.ToString());
                return null;
            }
        }
          public bool UpdateControleApontamento(IConfiguration _configuration,ControleApontamento _ctrl)
        {
            try{
                string sSql = string.Empty;

                sSql = "UPDATE TB_CONTROLE_APONTAMENTO SET"; 
                sSql += " QtdeApont=" + _ctrl.QtdeApont;
                sSql += ",DiasProximaMed=" + _ctrl.DiasProximaMed;
                sSql += ",ProxApont='" + _ctrl.ProxApont.Value.ToString("yyyy-MM-ddTHH:mm:ss") + "'";
                sSql += " WHERE IdControleApont=" + _ctrl.IdControleApont;

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
                log.Error("Erro ControleApontamentoModel-UpdateControleApontamento::" + ex.Message.ToString());
                return false;
            }
        }
    }
}