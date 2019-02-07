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
    public class Porta
    {
        public long? IdColetaPorta { get; set; }
        public long? IdLocalColeta { get; set; }
        public DateTime? DtColeta{get;set;}  
        public long? IdSensores{get;set;}
        public string DescSensor{get;set;}
        public string Valor {get;set;}  
        public string DescPorta{get;set;} 
        public bool CorDashboard{get;set;}    
    }

   
    public class PortaModel
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(PortaModel));
        
        public IEnumerable<Porta> SelectPorta(IConfiguration _configuration,long? IdLocalColeta,string dtIni, string dtFim)
        {            
            try
            {
                string sSql = string.Empty;

                sSql = "SELECT IdColetaPorta,IdLocalColeta,DtColeta,Valor,DescPorta,P.IdSensores,Descricao AS DescSensor";
                sSql = sSql + " FROM TB_COLETA_PORTA P INNER JOIN TB_SENSORES S ON P.IdSensores=S.IdSensores";
                sSql = sSql + " WHERE 1=1";


                if(IdLocalColeta !=null)
                    sSql = sSql + " AND IdLocalColeta=" + IdLocalColeta;                                 

                if(dtIni !=null && dtIni!="" && dtFim!=null && dtFim!="")
                    sSql = sSql + " AND DtColeta BETWEEN " + dtIni + " AND " + dtFim + "";
                                
                log.Debug(sSql);  
                IEnumerable <Porta> _portas;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                    _portas = db.Query<Porta>(sSql,commandTimeout:0);
                }
                return  _portas;
            }
            catch (Exception ex)
            {
                log.Error("Erro PortaModel-SelectPorta:" + ex.Message.ToString());
                return null;
            }
        }
        public IEnumerable<Porta> SelectPorta(IConfiguration _configuration,long IdLocalColeta,long IdSensores)
        {            
            try
            {
                string sSql = string.Empty;

                sSql = "SELECT TOP 1 IdColetaPorta,IdLocalColeta, DtColeta,Valor,DescPorta,P.IdSensores,Descricao AS DescSensor";
                sSql += " FROM TB_COLETA_PORTA P INNER JOIN TB_SENSORES S ON P.IdSensores=S.IdSensores";
                sSql += " WHERE 1=1";                                

                if(IdLocalColeta!=0)                
                    sSql += " AND IdLocalColeta=" + IdLocalColeta;

                if(IdSensores!=0)                
                    sSql += " AND P.IdSensores=" + IdSensores;

                sSql+= " ORDER BY DtColeta DESC";

                log.Debug(sSql);  
                IEnumerable <Porta> _portas;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                    _portas = db.Query<Porta>(sSql,commandTimeout:0);
                }
                return  _portas;
            }
            catch (Exception ex)
            {
                log.Error("Erro PortaModel-SelectPorta:" + ex.Message.ToString());
                return null;
            }
        }
        
    }
}