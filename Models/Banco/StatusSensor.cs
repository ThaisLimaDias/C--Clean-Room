using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using Embraer_Backend.Models;
using Microsoft.Extensions.Configuration;

namespace Embraer_Backend.Models
{
    public class StatusSensor
    {    
        public long Id_Wise {get;set;}
        public string Ip {get;set;}
        public int Chanel {get;set;}
        public string Url {get;set;}
        public bool Enable{get;set;}
        public DateTime? Dt_Status{get;set;}

    }
 
    
    public class StatusSensorModel
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(StatusSensorModel));
        
        public IEnumerable<StatusSensor> SelectStatus(IConfiguration _configuration, long idSensor)
        {            
            try
            {
                string sSql = string.Empty;

                sSql = "EXEC SPI_SP_ULTIMO_STATUS_SENSOR " + idSensor;
                       
                IEnumerable <StatusSensor> _st;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                    _st = db.Query<StatusSensor>(sSql,commandTimeout:0);
                }
                return  _st;
            }
            catch (Exception ex)
            {
                log.Error("Erro StatusSensorModel-SelectStatus:" + ex.Message.ToString());
                return null;
            }
        }
    }
}
