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
        public string Valor {get;set;}  
        public string DescPorta{get;set;}     
    }

   
    public class PortaModel
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(PortaModel));
        
        public IEnumerable<Porta> SelectPorta(IConfiguration _configuration,long? IdLocalColeta,string dtIni, string dtFim)
        {            
            try
            {
                string sSql = string.Empty;

                sSql = "SELECT IdColetaPorta,IdLocalColeta,DtColeta,Valor,IdSensores";
                sSql = sSql + " FROM TB_COLETA_PORTA";
                sSql = sSql + " WHERE 1=1";


                if(IdLocalColeta !=null)
                    sSql = sSql + " AND IdLocalColeta=" + IdLocalColeta;                                 

                if(dtIni !=null && dtIni!="" && dtFim!=null && dtFim!="")
                    sSql = sSql + " AND DtColeta BETWEEN " + dtIni + " AND " + dtFim + "";
                                

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

                sSql = "SELECT IdColetaPorta,IdLocalColeta, DtColeta,IdSensores,Valor,DescPorta FROM TB_COLETA_PORTA";
                sSql = sSql + " WHERE  IdColetaPorta IN (SELECT TOP 1 IdColetaPorta FROM TB_COLETA_PORTA";
                sSql = sSql + " WHERE IdLocalColeta=" + IdLocalColeta + " AND IdSensores =" + IdSensores + " ORDER BY DtColeta DESC)";                                
                                

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