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
    public class Secagens
    {
        public long IdSecagem { get; set; }
        public string DescMalote { get; set; }
        public DateTime  DtIniMalote {get;set;}
        public DateTime? DtFimMalote{get;set;} 
        public DateTime? DtEncMalote{get;set;}         
        public string  StatusMalote {get;set;}
    }

    public class SecagensOp
    {
        public long IdSecagemOp{get;set;}
        public long IdSecagem{get;set;}
        public string NumOp {get;set;}
        public DateTime? DtInsercao {get;set;}
    }

   
    public class SecagensModel
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(SecagensModel));
        
        public IEnumerable<Secagens> SelectSecagens(IConfiguration _configuration,long? id,string status, string dtIni,string dtFim)
        {            
            try
            {
                string sSql = string.Empty;

                sSql = "SELECT IdSecagem,DescMalote,DtIniMalote,DtFimMalote,DtEncMalote,StatusMalote";
                sSql = sSql + " FROM TB_SECAGEM";
                sSql = sSql + " WHERE 1=1";

                if(id!=null)                
                    sSql = sSql + " AND IdSecagem=" + id;
                
                if(status !=null && status!="")
                    sSql = sSql + " AND StatusMalote='" + status + "'";

                if(dtIni !=null && dtIni!="" && dtFim!=null && dtFim!="")
                    sSql = sSql + " AND DtIniMalote BETWEEN " + dtIni + " AND " + dtFim + "";
                                

                IEnumerable <Secagens> Secagens;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                    Secagens = db.Query<Secagens>(sSql,commandTimeout:0);
                }
                return  Secagens;
            }
            catch (Exception ex)
            {
                log.Error("Erro SecagensModel-SelectSecagens:" + ex.Message.ToString());
                return null;
            }
        }

        public IEnumerable<Secagens> SelectSecagensAbertas(IConfiguration _configuration)
        {            
            try
            {
                string sSql = string.Empty;

                sSql = "SELECT IdSecagem,DescMalote,DtIniMalote,DtFimMalote,DtEncMalote,StatusMalote";
                sSql = sSql + " FROM TB_SECAGEM";
                sSql = sSql + " WHERE StatusMalote in ('Iniciado','Aberto')";               

                IEnumerable <Secagens> Secagens;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                    Secagens = db.Query<Secagens>(sSql,commandTimeout:0);
                }
                return  Secagens;
            }
            catch (Exception ex)
            {
                log.Error("Erro SecagensModel-SelectSecagens:" + ex.Message.ToString());
                return null;
            }
        }

         public bool UpdateSecagens(IConfiguration _configuration,Secagens _Secagens)
        {
            try{
                string sSql = string.Empty;

                sSql = "UPDATE TB_SECAGEM SET";  
                sSql = sSql + " DtEncMalote=GETDATE()";
                sSql = sSql + ",StatusMalote='" + _Secagens.StatusMalote + "'";
                sSql = sSql + " WHERE IdSecagem=" + _Secagens.IdSecagem;

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
                log.Error("Erro SecagensModel-UpdateSecagens:" + ex.Message.ToString());
                return false;
            }
        }
        public IEnumerable<SecagensOp> SelectSecagensOp(IConfiguration _configuration,long id)
        {            
            try
            {
                string sSql = string.Empty;

                sSql = "SELECT IdSecagemOp,IdSecagem,NumOp,DtInsercao";
                sSql = sSql + " FROM TB_SECAGEM_OP";
                sSql = sSql + " WHERE IdSecagem="+ id;                                

                IEnumerable <SecagensOp> SecagensOp;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                    SecagensOp = db.Query<SecagensOp>(sSql,commandTimeout:0);
                }
                return  SecagensOp;
            }
            catch (Exception ex)
            {
                log.Error("Erro SecagensModel-SelectSecagensOp:" + ex.Message.ToString());
                return null;
            }
        }

          public bool InsertSecagensOP(IConfiguration _configuration,SecagensOp _secop)
        {
            
            string sSql = string.Empty;
            try
            {
                sSql= "INSERT INTO TB_SECAGEM_OP (IdSecagem,NumOp,DtInsercao)";
                sSql = sSql + " VALUES ";
                sSql = sSql + "(" + _secop.IdSecagem;
                sSql = sSql + ",'" + _secop.NumOp + "', GETDATE())";
                sSql = sSql + " SELECT @@IDENTITY";

                long insertId=0;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                   insertId =db.QueryFirstOrDefault<long>(sSql,commandTimeout:0);
                }
                if(insertId>0)
                {
                    _secop.IdSecagemOp=insertId;
                    return true;
                }
                return false;
            }
            
            catch(Exception ex)
            {
                log.Error("Erro SecagensModel-InsertSecagensOP:" + ex.Message.ToString());
                return false;
            }
        }
    }
}