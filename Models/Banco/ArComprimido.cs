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
        public DateTime? DtMedicao{get;set;}  

        public string Valor{get;set;}
    }

   
    public class ArComprimidoModel
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ArComprimidoModel));
        
        public IEnumerable<ArComprimido> SelectArComprimido(IConfiguration _configuration,long IdApontArComprimido,long IdLocalMedicao,string dtIni,string dtFim)
        {            
            try
            {
                string sSql = string.Empty;

                sSql = "SELECT IdApontArComprimido,IdLocalMedicao,IdUsuario,DtMedicao,Valor";
                sSql = sSql + " FROM TB_APONT_AR_COMPRIMIDO";
                sSql = sSql + " WHERE 1=1";
               
                if(IdApontArComprimido!=0)                
                    sSql = sSql + " AND IdApontArComprimido=" + IdApontArComprimido;

                if(IdLocalMedicao!=0)                
                    sSql = sSql + " AND IdLocalMedicao=" + IdLocalMedicao;
                
                if(dtIni !=null && dtIni!="" && dtFim!=null && dtFim!="")
                    sSql = sSql + " AND DtMedicao BETWEEN " + dtIni + " AND " + dtFim + "";         

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