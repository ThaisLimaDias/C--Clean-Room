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
    public class Sensores
    {
        public long IdSensores { get; set; }
        public long IdLocalMedicao { get; set; }
        public string  Descricao {get;set;}
        public string Ip{get;set;}  
        public string TipoSensor{get;set;} 
        public int  Canal {get;set;}

    }

   
    public class SensoresModel
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(SensoresModel));
        
        public IEnumerable<Sensores> SelectSensor(IConfiguration _configuration,long IdSensores,long  IdLocalMedicao,string TipoSensor)
        {            
            try
            {
                string sSql = string.Empty;

                sSql = "SELECT IdSensores,IdLocalMedicao,Descricao,Ip,TipoSensor,Canal";
                sSql = sSql + " FROM TB_SENSORES";
                sSql = sSql + " WHERE 1=1";

                if(IdSensores!=0)                
                    sSql = sSql + " AND IdSensores=" + IdSensores;

                if(IdLocalMedicao!=0)                
                    sSql = sSql + " AND IdLocalMedicao=" + IdLocalMedicao;  

                                
                if(TipoSensor !=null && TipoSensor!="")
                    sSql = sSql + " AND TipoSensor='" + TipoSensor + "'";             

                IEnumerable <Sensores> _sen;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_ProjectCleanning_Sala_Limpa")))
                {
                    _sen = db.Query<Sensores>(sSql,commandTimeout:0);
                }
                return  _sen;
            }
            catch (Exception ex)
            {
                log.Error("Erro SensoresModel-Sensores:" + ex.Message.ToString());
                return null;
            }
        }

    }
}