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
    public class Temperatura
    {
        public long IdColetaTemperatura { get; set; }
        public long IdLocalColeta { get; set; }
        public DateTime? DtColeta{get;set;}  
        public long IdSensores{get;set;} 
        public decimal Valor {get;set;}
        public long IdCadParametroSistema { get; set; }
        public decimal ControleMin {get;set;}
        public decimal EspecificacaoMin {get;set;}
        public decimal EspecificacaoMax {get;set;}
        public decimal ControleMax {get;set;}

    }

   
    public class TemperaturaModel
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(TemperaturaModel));
        
        public IEnumerable<Temperatura> SelectTemperatura(IConfiguration _configuration,long? IdLocalColeta,string dtIni, string dtFim)
        {            
            try
            {
                string sSql = string.Empty;

                sSql = "SELECT IdColetaTemperatura,IdLocalColeta,DtColeta,IdSensores";
                sSql = sSql + ",Valor,IdCadParametroSistema,ControleMin,EspecificacaoMin";
                sSql = sSql + ",EspecificacaoMax,ControleMax";
                sSql = sSql + " FROM TB_COLETA_TEMPERATURA";
                sSql = sSql + " WHERE 1=1";

                if(IdLocalColeta !=null)
                    sSql = sSql + " AND IdLocalColeta=" + IdLocalColeta;                                 

                if(dtIni !=null && dtIni!="" && dtFim!=null && dtFim!="")
                    sSql = sSql + " AND DtColeta BETWEEN " + dtIni + " AND " + dtFim + "";
                                

                IEnumerable <Temperatura> temperaturas;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                    temperaturas = db.Query<Temperatura>(sSql,commandTimeout:0);
                }
                return  temperaturas;
            }
            catch (Exception ex)
            {
                log.Error("Erro TemperaturaModel-SelectTemperatura:" + ex.Message.ToString());
                return null;
            }
        }
        public IEnumerable<Temperatura> SelectTemperatura(IConfiguration _configuration,long IdLocalColeta)
        {            
            try
            {
                string sSql = string.Empty;

                sSql = "SELECT TOP 1 IdColetaTemperatura,IdLocalColeta,DtColeta,IdSensores";
                sSql = sSql + ",Valor,IdCadParametroSistema,ControleMin,EspecificacaoMin";
                sSql = sSql + ",EspecificacaoMax,ControleMax";
                sSql = sSql + " FROM TB_COLETA_TEMPERATURA";
                sSql = sSql + " WHERE IdLocalColeta=" + IdLocalColeta;  
                sSql = sSql + " ORDER BY DtColeta DESC";                    
                                

                IEnumerable <Temperatura> temperaturas;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                    temperaturas = db.Query<Temperatura>(sSql,commandTimeout:0);
                }
                return  temperaturas;
            }
            catch (Exception ex)
            {
                log.Error("Erro TemperaturaModel-SelectTemperatura:" + ex.Message.ToString());
                return null;
            }
        }
    }
}