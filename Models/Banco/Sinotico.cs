using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using Embraer_Backend.Models;
using Microsoft.Extensions.Configuration;

namespace Embraer_Backend.Models
{
    public class Sinotico
    {    
         public string Local {get;set;}
        public long Sensor {get;set;}
        public string Valor {get;set;}
        public string UnidadeMedida {get;set;}
        public string Status {get;set;}
        public DateTime? DtColeta{get;set;}
    
        public Sinotico()
        {
            this.Local = string.Empty;
            this.Sensor = 0;
            this.Valor =string.Empty;
            this.UnidadeMedida = string.Empty;
            this.Status = string.Empty;
        }
    }
 
    
    public class SinoticoModel
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(SinoticoModel));
        
        public IEnumerable<Sinotico> SelectSinotico(IConfiguration _configuration, int porta, int temperatura, int pressao, int umidade,long local )
        {            
            try
            {
                string sSql = string.Empty;

                sSql = "EXEC SPI_SP_SINOTICO " + porta +","+ temperatura +","+ pressao +","+ umidade +","+local;
                       
                IEnumerable <Sinotico> _sn;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                    _sn = db.Query<Sinotico>(sSql,commandTimeout:0);
                }
                return  _sn;
            }
            catch (Exception ex)
            {
                log.Error("Erro SinoticoModel-SelectSinoticoPintura:" + ex.Message.ToString());
                return null;
            }
        }
         public IEnumerable<Sinotico> SelectSinotico(IConfiguration _configuration)
        {            
            try
            {
                string sSql = string.Empty;

                sSql = "EXEC SPI_SP_SINOTICO_SL";
                       
                IEnumerable <Sinotico> _snSl;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                    _snSl = db.Query<Sinotico>(sSql,commandTimeout:0);
                }
                return  _snSl;
            }
            catch (Exception ex)
            {
                log.Error("Erro SinoticoModel-SelectSinoticoSL:" + ex.Message.ToString());
                return null;
            }
        }
    }
}
