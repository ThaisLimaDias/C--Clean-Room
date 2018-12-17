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
    public class Parametros
    {
        public long IdCadParametroSistema { get; set; }
        public string DescLocalMedicao {get;set;}
        public string DescParametro { get; set; }
        public decimal  ControleMin {get;set;}
        public decimal  EspecificacaoMin {get;set;}
        public decimal EspecificacaoMax{get;set;}
        public decimal ControleMax{get;set;}
        public int TempoAlarmeMinutos{get;set;}
        public string MsgTempoAlarme{get;set;}
        public string MsgAlarme{get;set;}
        public DateTime? DtUltAtlz{get;set;}
    }


    public class ParametrosModel
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ParametrosModel));
        
        public IEnumerable<Parametros> SelectParametros(IConfiguration _configuration, long IdLocalMedicao,string DescParametro)
        {            
            try
            {
                string sSql = string.Empty;

                sSql = "SELECT IdCadParametroSistema,DescParametro,ControleMin,EspecificacaoMin,EspecificacaoMax,ControleMax,TempoAlarmeMinutos,MsgTempoAlarme,MsgAlarme,DtUltAtlz";
                sSql = sSql + " FROM TB_CADASTRO_PARAMETROS P INNER JOIN TB_LOCAL_MEDICAO L ON P.IdLocalMedicao = L.IdLocalMedicao";
                sSql = sSql + " WHERE 1=1";               

                if(IdLocalMedicao!=0)
                    sSql = sSql + " AND P.IdLocalMedicao=" + IdLocalMedicao;

                if(DescParametro!=null && DescParametro!="")
                    sSql = sSql + " AND DescParametro LIKE '%" + DescParametro + "%'";

                IEnumerable <Parametros> _parametros;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                    _parametros = db.Query<Parametros>(sSql,commandTimeout:0);
                }                 
                return _parametros;
            }
            catch (Exception ex)
            {
                log.Error("Erro ParametrosModel-SelectParametros:" + ex.Message.ToString());
                return null;
            }
        }

        public bool UpdateParametros (IConfiguration _configuration,Parametros _parametros)
        {
            try{
                string sSql = string.Empty;

                sSql = "UPDATE TB_CADASTRO_PARAMETROS SET";  
                sSql=sSql+ ",[ControleMin]="+ _parametros.ControleMin;
                sSql=sSql+ ",[EspecificacaoMin]="+ _parametros.EspecificacaoMin;
                sSql=sSql+ ",[EspecificacaoMax]="+ _parametros.EspecificacaoMax;
                sSql=sSql+ ",[ControleMax]="+ _parametros.ControleMax;
                sSql=sSql+ ",[TempoAlarmeMinutos]="+ _parametros.TempoAlarmeMinutos;
                sSql=sSql+ ",[MsgTempoAlarme]='"+ _parametros.MsgTempoAlarme + "'";
                sSql=sSql+ ",[MsgAlarme]='"+ _parametros.MsgAlarme + "'";
                sSql=sSql+ ",[DtUltAtlz]=GETDATE()";
                sSql =sSql+ " WHERE IdCadParametroSistema=" + _parametros.IdCadParametroSistema;

                long update = 0;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
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
                log.Error("Erro ParametrosModel-UpdateParametros:" + ex.Message.ToString());
                return false;
            }
        }

    }
}