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
    public class Temperatura
    {
        public long? IdColetaTemperatura { get; set; }
        public long? IdLocalColeta { get; set; }
        public DateTime? DtColeta{get;set;}  
        public long IdSensores{get;set;} 
        public string DescSensor{get;set;}
        public decimal Valor {get;set;}
        public string UnidMedida {get;set;}
        public long? IdCadParametroSistema { get; set; }
        public decimal? ControleMin {get;set;}
        public decimal? EspecificacaoMin {get;set;}
        public decimal? EspecificacaoMax {get;set;}
        public decimal? ControleMax {get;set;}
        public string Etapa{get;set;}
        public bool CorDashboard{get;set;}

    }
    public class TemperaturaReport
    {
         
        public long IdLocalColeta { get; set; }

        public string DescLocalMedicao { get; set; }
        public DateTime? DtColeta{get;set;}  
        public long IdSensores{get;set;} 
        public string DescSensor{get;set;}
        public decimal Valor {get;set;}
        public string UnidMedida {get;set;}
        public decimal ControleMin {get;set;}
        public decimal EspecificacaoMin {get;set;}
        public decimal EspecificacaoMax {get;set;}
        public decimal ControleMax {get;set;}
        public string Etapa{get;set;}
    }
   
    public class TemperaturaModel
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(TemperaturaModel));
        
        public IEnumerable<Temperatura> SelectTemperatura(IConfiguration _configuration,long? IdLocalColeta,string dtIni, string dtFim)
        {            
            try
            {
                string sSql = string.Empty;

                sSql = "SELECT IdColetaTemperatura,IdLocalColeta,DtColeta,T.IdSensores,Descricao AS DescSensor";
                sSql = sSql + ",Valor,UnidMedida,T.IdCadParametroSistema,ControleMin,EspecificacaoMin";
                sSql = sSql + ",EspecificacaoMax,ControleMax,Etapa";
                sSql = sSql + " FROM TB_COLETA_TEMPERATURA T INNER JOIN TB_SENSORES S ON T.IdSensores=S.IdSensores";
                sSql = sSql + " WHERE 1=1";

                if(IdLocalColeta !=null)
                    sSql = sSql + " AND IdLocalColeta=" + IdLocalColeta;                                 

                if(dtIni !=null && dtIni!="" && dtFim!=null && dtFim!="")
                    sSql = sSql + " AND DtColeta BETWEEN " + dtIni + " AND " + dtFim + "";
                                

                IEnumerable <Temperatura> temperaturas;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_ProjectCleanning_Sala_Limpa")))
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
        public IEnumerable<Temperatura> SelectTemperaturaSL(IConfiguration _configuration,long IdLocalColeta)
        {            
            try
            {
                string sSql = string.Empty;

                sSql = "SPI_SP_TEMPERATURA_MIN_MAX_SL " + IdLocalColeta ;                
                              
                                
                IEnumerable <Temperatura> temperaturas;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_ProjectCleanning_Sala_Limpa")))
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

                sSql = "SELECT TOP 1 IdColetaTemperatura,IdLocalColeta,DtColeta,T.IdSensores,Descricao AS DescSensor";
                sSql = sSql + ",Valor,UnidMedida,T.IdCadParametroSistema,ControleMin,EspecificacaoMin";
                sSql = sSql + ",EspecificacaoMax,ControleMax,Etapa";
                sSql = sSql + " FROM TB_COLETA_TEMPERATURA T INNER JOIN TB_SENSORES S ON T.IdSensores=S.IdSensores";
                sSql = sSql + " WHERE IdLocalColeta=" + IdLocalColeta;  
                sSql = sSql + " ORDER BY DtColeta DESC";                    

                log.Debug(sSql);                

                IEnumerable <Temperatura> temperaturas;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_ProjectCleanning_Sala_Limpa")))
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
        public IEnumerable<TemperaturaReport> TemperaturaReport(IConfiguration _configuration,long? IdLocalColeta,string dtIni, string dtFim,decimal? Temperatura,string Etapa,long? IdSensores)
        {            
            try
            {
                string sSql = string.Empty;

                sSql = "SELECT IdLocalColeta,DescLocalMedicao,DtColeta,IdSensores,Descricao as DescSensor";
                sSql +=  ",Valor,UnidMedida,ControleMin,EspecificacaoMin,EspecificacaoMax";
	            sSql +=  ",ControleMax,Etapa";
                sSql +=  " FROM VW_REPORT_TEMPERATURA";
                sSql +=  " WHERE 1=1";

                if (IdSensores!=null && IdSensores!=0)
                    sSql += " AND IdSensores=" + IdSensores;

                if (IdLocalColeta!=null && IdLocalColeta!=0)
                    sSql += " AND IdLocalColeta=" + IdLocalColeta;

                if (dtIni !=null && dtIni!="" && dtFim!=null && dtFim!="")
                    sSql += " AND DtColeta BETWEEN " + dtIni + " AND " + dtFim + ""; 
                
                if (Etapa !=null && Etapa!="")
                    sSql += " AND Etapa ='" + Etapa + "'"; 

                if(Temperatura!=null)
                    sSql += " AND Valor ='" + Temperatura.ToString().Replace(",",".") + "'";

                sSql += " ORDER BY DtColeta ";                    
                                

                IEnumerable <TemperaturaReport> report;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_ProjectCleanning_Sala_Limpa")))
                {
                    report = db.Query<TemperaturaReport>(sSql,commandTimeout:0);
                }
                return  report;
            }
            catch (Exception ex)
            {
                log.Error("Erro TemperaturaModel-TemperaturaReport:" + ex.Message.ToString());
                return null;
            }
        }
    }
}