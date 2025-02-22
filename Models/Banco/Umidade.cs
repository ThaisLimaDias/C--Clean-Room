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
    public class Umidade
    {
        public long IdColetaUmidade { get; set; }
        public long IdLocalColeta { get; set; }
        public DateTime? DtColeta{get;set;}  
        public long IdSensores{get;set;}
        public string DescSensor{get;set;}
        public decimal Valor {get;set;}
        public string UnidMedida {get;set;}
        public long IdCadParametroSistema { get; set; }
        public decimal ControleMin {get;set;}
        public decimal EspecificacaoMin {get;set;}
        public decimal EspecificacaoMax {get;set;}
        public decimal ControleMax {get;set;}
        public bool CorDashboard{get;set;}

    }

    public class UmidadeReport
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
    }
   
    public class UmidadeModel
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(UmidadeModel));
        
        public IEnumerable<Umidade> SelectUmidade(IConfiguration _configuration,long? IdLocalColeta,string dtIni, string dtFim)
        {            
            try
            {
                string sSql = string.Empty;

                sSql = "SELECT IdColetaUmidade,IdLocalColeta,DtColeta,U.IdSensores,Descricao AS DescSensor";
                sSql = sSql + ",Valor,UnidMedida,U.IdCadParametroSistema,ControleMin,EspecificacaoMin";
                sSql = sSql + ",EspecificacaoMax,ControleMax";
                sSql = sSql + " FROM TB_COLETA_UMIDADE U INNER JOIN TB_SENSORES S ON U.IdSensores=S.IdSensores";
                sSql = sSql + " WHERE 1=1";
                log.Debug(sSql);  
                if(IdLocalColeta !=null)
                    sSql = sSql + " AND IdLocalColeta=" + IdLocalColeta;                                   

                if(dtIni !=null && dtIni!="" && dtFim!=null && dtFim!="")
                    sSql = sSql + " AND DtColeta BETWEEN " + dtIni + " AND " + dtFim + "";
                                

                IEnumerable <Umidade> umidade;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_ProjectCleanning_Sala_Limpa")))
                {
                    umidade = db.Query<Umidade>(sSql,commandTimeout:0);
                }
                return  umidade;
            }
            catch (Exception ex)
            {
                log.Error("Erro UmidadeModel-SelectUmidade:" + ex.Message.ToString());
                return null;
            }
        }
        public IEnumerable<Umidade> SelectUmidade(IConfiguration _configuration,long IdLocalColeta)
        {            
            try
            {
                string sSql = string.Empty;

                sSql = "SELECT TOP 1 IdColetaUmidade,IdLocalColeta,DtColeta,U.IdSensores,Descricao AS DescSensor";
                sSql = sSql + ",Valor,UnidMedida,U.IdCadParametroSistema,ControleMin,EspecificacaoMin";
                sSql = sSql + ",EspecificacaoMax,ControleMax";
                sSql = sSql + " FROM TB_COLETA_UMIDADE U INNER JOIN TB_SENSORES S ON U.IdSensores=S.IdSensores";
                sSql = sSql + " WHERE IdLocalColeta=" + IdLocalColeta;  
                sSql = sSql + " ORDER BY DtColeta DESC"; 
                                

                IEnumerable <Umidade> umidade;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_ProjectCleanning_Sala_Limpa")))
                {
                    umidade = db.Query<Umidade>(sSql,commandTimeout:0);
                }
                return  umidade;
            }
            catch (Exception ex)
            {
                log.Error("Erro UmidadeModel-SelectUmidade:" + ex.Message.ToString());
                return null;
            }
        }
        public IEnumerable<UmidadeReport> UmidadeReport(IConfiguration _configuration,long? IdLocalColeta,string dtIni, string dtFim,decimal? Umidade,long? IdSensores)
        {            
            try
            {
                string sSql = string.Empty;

                sSql = "SELECT IdLocalColeta,DescLocalMedicao,DtColeta,IdSensores,Descricao as DescSensor";
                sSql +=  ",Valor,UnidMedida,ControleMin,EspecificacaoMin,EspecificacaoMax";
	            sSql = sSql + ",ControleMax";
                sSql = sSql + " FROM VW_REPORT_UMIDADE";
                sSql = sSql + " WHERE 1=1";
                
                if (IdSensores!=null && IdSensores!=0)
                    sSql += " AND IdSensores=" + IdSensores;

                if (IdLocalColeta!=null && IdLocalColeta!=0)
                    sSql += " AND IdLocalColeta=" + IdLocalColeta;

                if (dtIni !=null && dtIni!="" && dtFim!=null && dtFim!="")
                    sSql += " AND DtColeta BETWEEN " + dtIni + " AND " + dtFim + ""; 
                

                if(Umidade!=null)
                    sSql += " AND Valor ='" + Umidade.ToString().Replace(",",".") + "'";

                sSql += " ORDER BY DtColeta ";                 
                                

                IEnumerable <UmidadeReport> report;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_ProjectCleanning_Sala_Limpa")))
                {
                    report = db.Query<UmidadeReport>(sSql,commandTimeout:0);
                }
                return  report;
            }
            catch (Exception ex)
            {
                log.Error("Erro UmidadeModel-UmidadeReport:" + ex.Message.ToString());
                return null;
            }
        }
    }
}