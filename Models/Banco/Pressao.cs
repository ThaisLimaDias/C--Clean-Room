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
    public class Pressao
    {
        public long IdColetaPressao { get; set; }
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
    public class PressaoReport
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
   
    public class PressaoModel
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(PressaoModel));
        
        public IEnumerable<Pressao> SelectPressao(IConfiguration _configuration,long? IdLocalColeta,string dtIni, string dtFim)
        {            
            try
            {
                string sSql = string.Empty;

                sSql = "SELECT IdColetaPressao,IdLocalColeta,DtColeta,P.IdSensores,Descricao AS DescSensor";
                sSql = sSql + ",Valor,UnidMedida,P.IdCadParametroSistema,ControleMin,EspecificacaoMin";
                sSql = sSql + ",EspecificacaoMax,ControleMax";
                sSql = sSql + " FROM TB_COLETA_PRESSAO P INNER JOIN TB_SENSORES S ON P.IdSensores=S.IdSensores";
                sSql = sSql + " WHERE 1=1";

                if(IdLocalColeta !=null)
                    sSql = sSql + " AND IdLocalColeta=" + IdLocalColeta;                                  

                if(dtIni !=null && dtIni!="" && dtFim!=null && dtFim!="")
                    sSql = sSql + " AND DtColeta BETWEEN " + dtIni + " AND " + dtFim + "";
                                

                IEnumerable <Pressao> pressao;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                    pressao = db.Query<Pressao>(sSql,commandTimeout:0);
                }
                return  pressao;
            }
            catch (Exception ex)
            {
                log.Error("Erro PressaoModel-SelectPressao:" + ex.Message.ToString());
                return null;
            }
        }
        public IEnumerable<Pressao> SelectPressao(IConfiguration _configuration,long IdLocalColeta)
        {            
            try
            {
                string sSql = string.Empty;

                sSql = "SELECT TOP 1 IdColetaPressao,IdLocalColeta,DtColeta,P.IdSensores,Descricao AS DescSensor";
                sSql = sSql + ",Valor,UnidMedida,P.IdCadParametroSistema,ControleMin,EspecificacaoMin";
                sSql = sSql + ",EspecificacaoMax,ControleMax";
                sSql = sSql + " FROM TB_COLETA_PRESSAO P INNER JOIN TB_SENSORES S ON P.IdSensores=S.IdSensores";
                sSql = sSql + " WHERE IdLocalColeta=" + IdLocalColeta;  
                sSql = sSql + " ORDER BY DtColeta DESC";               

                log.Debug(sSql);                  

                IEnumerable <Pressao> pressao;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                    pressao = db.Query<Pressao>(sSql,commandTimeout:0);
                }
                return  pressao;
            }
            catch (Exception ex)
            {
                log.Error("Erro PressaoModel-SelectPressao:" + ex.Message.ToString());
                return null;
            }
        }

        public IEnumerable<PressaoReport> PressaoReport(IConfiguration _configuration,long? IdLocalColeta,string dtIni, string dtFim,decimal? Pressao,long? IdSensores )
        {            
            try
            {
                string sSql = string.Empty;

                sSql = "SELECT IdLocalColeta,DescLocalMedicao,DtColeta,IdSensores,Descricao AS DescSensor";
                sSql = sSql + ",Valor,UnidMedida,ControleMin,EspecificacaoMin,EspecificacaoMax";
	            sSql = sSql + ",ControleMax";
                sSql = sSql + " FROM VW_REPORT_PRESSAO";
                sSql = sSql + " WHERE 1=1";

                if (IdSensores!=null && IdSensores!=0)
                    sSql += " AND IdSensores=" + IdSensores;

                if (IdLocalColeta!=null && IdLocalColeta!=0)
                    sSql = sSql + " AND IdLocalColeta=" + IdLocalColeta;

                if (dtIni !=null && dtIni!="" && dtFim!=null && dtFim!="")
                    sSql = sSql + " AND DtColeta BETWEEN " + dtIni + " AND " + dtFim + ""; 

                if (Pressao!=null)
                    sSql += " AND Valor ='" + Pressao.ToString().Replace(",",".") + "'";

                sSql = sSql + " ORDER BY DtColeta ";                    
                                

                IEnumerable <PressaoReport> report;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                    report = db.Query<PressaoReport>(sSql,commandTimeout:0);
                }
                return  report;
            }
            catch (Exception ex)
            {
                log.Error("Erro PressaoModel-PressaoReport:" + ex.Message.ToString());
                return null;
            }
        }

    }
}