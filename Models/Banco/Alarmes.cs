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
    public class Alarmes
    {
        public long IdAlarme { get; set; }
        public string DescLocalMedicao { get; set; }
        public string  TipoAlarme {get;set;}
        public DateTime? DtInicio{get;set;}  
        public DateTime? DtFim{get;set;} 
        public string  Mensagem {get;set;}
        public long? IdUsuarioReconhecimento { get; set; }
        public string  DescReconhecimento {get;set;}
        public DateTime? DtReconhecimento{get;set;} 
        public long? IdUsuarioJustificativa { get; set; }
        public string  DescJustificativa {get;set;}
        public DateTime? DtJustificativa{get;set;}    
        public long IdCadParametroSistema {get;set;}
        public decimal ControleMin {get;set;}
        public decimal EspecificacaoMin {get;set;}
        public decimal EspecificacaoMax {get;set;}
        public decimal ControleMax {get;set;}
        public string StatusAlarme  {get;set;} 

    }

   
    public class AlarmesModel
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(AlarmesModel));
        
        public IEnumerable<Alarmes> SelectAlarmes(IConfiguration _configuration,long id,long  IdLocalMedicao,string status, string dtIni,string dtFim)
        {            
            try
            {
                string sSql = string.Empty;

                sSql = "SELECT IdAlarme,DescLocalMedicao,TipoAlarme,DtInicio,DtFim,Mensagem,IdUsuarioReconhecimento";
                sSql = sSql + ",DescReconhecimento,DtReconhecimento,IdUsuarioJustificativa,DescJustificativa,DtJustificativa";
                sSql = sSql + ",IdCadParametroSistema,ControleMin,EspecificacaoMin,EspecificacaoMax,ControleMax,StatusAlarme";
                sSql = sSql + " FROM TB_ALARMES A INNER JOIN TB_LOCAL_MEDICAO L ON A.IdLocalMedicao=L.IdLocalMedicao";
                sSql = sSql + " WHERE 1=1";

                if(id!=0)                
                    sSql = sSql + " AND IdAlarme=" + id;

                if(IdLocalMedicao!=0)                
                    sSql = sSql + " AND IdLocalMedicao=" + IdLocalMedicao;
                
                if(status !=null && status!="")
                    sSql = sSql + " AND StatusAlarme='" + status + "'";

                if(dtIni !=null && dtIni!="" && dtFim!=null && dtFim!="")
                    sSql = sSql + " AND DtInicio BETWEEN " + dtIni + " AND " + dtFim + "";
                                

                IEnumerable <Alarmes> Alarmes;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                    Alarmes = db.Query<Alarmes>(sSql,commandTimeout:0);
                }
                return  Alarmes;
            }
            catch (Exception ex)
            {
                log.Error("Erro AlarmesModel-SelectAlarmes:" + ex.Message.ToString());
                return null;
            }
        }

        public IEnumerable<Alarmes> SelectAlarmesAbertos(IConfiguration _configuration)
        {            
            try
            {
                string sSql = string.Empty;

                sSql = "SELECT IdAlarme,DescLocalMedicao,TipoAlarme,DtInicio,DtFim,Mensagem,IdUsuarioReconhecimento";
                sSql = sSql + ",DescReconhecimento,DtReconhecimento,IdUsuarioJustificativa,DescJustificativa,DtJustificativa";
                sSql = sSql + ",IdCadParametroSistema,ControleMin,EspecificacaoMin,EspecificacaoMax,ControleMax,StatusAlarme";
                sSql = sSql + " FROM TB_ALARMES A INNER JOIN TB_LOCAL_MEDICAO L ON A.IdLocalMedicao=L.IdLocalMedicao";
                sSql = sSql + " WHERE StatusAlarme in ('Ativo','Reconhecido','Justificado')";

                IEnumerable <Alarmes> Alarmes;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                    Alarmes = db.Query<Alarmes>(sSql,commandTimeout:0);
                }
                return  Alarmes;
            }
            catch (Exception ex)
            {
                log.Error("Erro AlarmesModel-SelectAlarmes:" + ex.Message.ToString());
                return null;
            }
        }

         public bool UpdateAlarmes(IConfiguration _configuration,Alarmes _alarmes)
        {
            try{
                string sSql = string.Empty;

                string usRec = (_alarmes.IdUsuarioReconhecimento==null) ?"NULL" :_alarmes.IdUsuarioReconhecimento.Value.ToString();
                string usJus = (_alarmes.IdUsuarioJustificativa==null) ? "NULL" :_alarmes.IdUsuarioJustificativa.Value.ToString();
                
                sSql = "UPDATE TB_ALARMES SET";  
                sSql = sSql + " IdUsuarioReconhecimento=" + usRec;  
                sSql = sSql + ",DescReconhecimento=" + ((_alarmes.DescReconhecimento==null) ? "NULL" : "'" + _alarmes.DescReconhecimento +"'") + "";
                sSql = sSql + ",DtReconhecimento=" + ((_alarmes.DtReconhecimento==null) ? "NULL" :  "'" + _alarmes.DtReconhecimento.Value.ToString("yyyy-MM-ddTHH:mm:ss") +"'") + "";
                sSql = sSql + ",IdUsuarioJustificativa=" + usJus;
                sSql = sSql + ",DescJustificativa=" + ((_alarmes.DescJustificativa==null) ? "NULL" : "'" + _alarmes.DescJustificativa +"'") + "";
                sSql = sSql + ",DtJustificativa="+ ((_alarmes.DtJustificativa==null) ? "NULL" : "'" +  _alarmes.DtJustificativa.Value.ToString("yyyy-MM-ddTHH:mm:ss") +"'") + "";  
                sSql = sSql + ",StatusAlarme='"  + _alarmes.StatusAlarme + "'"; 
                sSql = sSql + " WHERE IdAlarme=" + _alarmes.IdAlarme;

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
                log.Error("Erro AlarmesModel-UpdateAlarmes:" + ex.Message.ToString());
                return false;
            }
        }
    }
}