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
    public class ArComprimido
    {
        public long IdApontArComprimido { get; set; }
        public long IdLocalMedicao { get; set; }
        public long IdUsuario{get;set;}        
        public string DescLocalMedicao { get; set; }
        public string CodUsuario{get;set;}
        public DateTime? DtMedicao{get;set;}  
        public string Valor{get;set;}
        public DateTime? DtOcorrencia{get;set;}
        public string FatoOcorrencia{get;set;}
        public string AcoesObservacoes{get;set;}
    }

   
    public class ArComprimidoModel
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ArComprimidoModel));
        
        public IEnumerable<ArComprimido> SelectArComprimido(IConfiguration _configuration,long? IdApontArComprimido,long? IdLocalMedicao,string dtIni,string dtFim,bool? Ocorrencia)
        {            
            try
            {
                string sSql = string.Empty;

                sSql = "SELECT IdApontArComprimido,A.IdLocalMedicao,DescLocalMedicao,A.IdUsuario,CodUsuario,DtMedicao,Valor,FatoOcorrencia,AcoesObservacoes";
                sSql = sSql + " FROM TB_APONT_AR_COMPRIMIDO A";
                sSql = sSql + " INNER JOIN TB_LOCAL_MEDICAO L ON A.IdLocalMedicao = L.IdLocalMedicao";
                sSql = sSql + " INNER JOIN TB_USUARIO U ON A.IdUsuario = U.IdUsuario";
                sSql = sSql + " WHERE 1=1";
               
                if(IdApontArComprimido!=0 && IdApontArComprimido!=null)                
                    sSql = sSql + " AND IdApontArComprimido=" + IdApontArComprimido;

                if(IdLocalMedicao!=0 && IdLocalMedicao!=null)                 
                    sSql = sSql + " AND IdLocalMedicao=" + IdLocalMedicao;
                
                if(dtIni !=null && dtIni!="" && dtFim!=null && dtFim!="")
                    sSql = sSql + " AND DtMedicao BETWEEN '" + dtIni + "' AND '" + dtFim + "'";   

                if (Ocorrencia.Value)
                    sSql=sSql + " AND DtOcorrencia IS NULL";      

                IEnumerable <ArComprimido> _arcomp;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                    _arcomp = db.Query<ArComprimido>(sSql,commandTimeout:0);
                }
                return  _arcomp;
            }
            catch (Exception ex)
            {
                log.Error("Erro ArComprimidoModel-SelectArComprimido:" + ex.Message.ToString());
                return null;
            }
        }

        public IEnumerable<ArComprimido> SelectArComprimido(IConfiguration _configuration,long IdLocalMedicao)
        {            
            try
            {
                string sSql = string.Empty;

                sSql = "SELECT TOP 1 IdApontArComprimido,IdLocalMedicao,IdUsuario,DtMedicao,Valor";
                sSql = sSql + " FROM TB_APONT_AR_COMPRIMIDO";
                sSql = sSql + " WHERE IdLocalMedicao=" + IdLocalMedicao;
                sSql = sSql + " ORDER BY DtMedicao DESC";
                
                log.Debug(sSql);


                IEnumerable <ArComprimido> _arcomp;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                    _arcomp = db.Query<ArComprimido>(sSql,commandTimeout:0);
                }
                return  _arcomp;
            }
            catch (Exception ex)
            {
                log.Error("Erro ArComprimidoModel-SelectArComprimido:" + ex.Message.ToString());
                return null;
            }
        }
        public bool UpdateArComprimido(IConfiguration _configuration,ArComprimido _ar)
        {
            try{
                string sSql = string.Empty;

                string dataOcorrencia = (_ar.DtOcorrencia==null) ? "NULL" : _ar.DtOcorrencia.Value.ToString("yyyy-MM-ddTHH:mm:ss");

                sSql = "UPDATE TB_APONT_AR_COMPRIMIDO SET";                
                sSql=sSql+ ((dataOcorrencia=="NULL")? "[DtOcorrencia]="+  dataOcorrencia + "" : "[DtOcorrencia]='"+  dataOcorrencia + "'");
                sSql=sSql+ ",[FatoOcorrencia]='"+ _ar.FatoOcorrencia + "'";
                sSql=sSql+ ",[AcoesObservacoes]='"+ _ar.AcoesObservacoes + "'";
                sSql =sSql+ " WHERE IdApontArComprimido=" + _ar.IdApontArComprimido;

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
                log.Error("Erro ArComprimidoModel-UpdateArComprimido:" + ex.Message.ToString());
                return false;
            }
        }

        public bool InsertArComprimido(IConfiguration _configuration,ArComprimido _arcomp)
        {
            
            string sSql = string.Empty;
            try
            {
             
                sSql= "INSERT INTO TB_APONT_AR_COMPRIMIDO (IdLocalMedicao,IdUsuario,DtMedicao,Valor)";
                sSql = sSql + " VALUES ";
                sSql = sSql + "(" + _arcomp.IdLocalMedicao;
                sSql = sSql + "," + _arcomp.IdUsuario;
                sSql = sSql + ", GETDATE(),'" + _arcomp.Valor + "')";
                sSql = sSql + " SELECT @@IDENTITY";

                long insertId=0;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                   insertId =db.QueryFirstOrDefault<long>(sSql,commandTimeout:0);
                }
                if(insertId>0)
                {
                    _arcomp.IdApontArComprimido=insertId;
                    return true;
                }
                return false;
            }
            
            catch(Exception ex)
            {
                log.Error("Erro ArComprimidoModel-InsertArComprimido:" + ex.Message.ToString());
                return false;
            }        
        }
    }
}