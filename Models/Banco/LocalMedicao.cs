using System;
using System.Collections.Generic;
using System.Reflection;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ProjectCleanning_Backend.Models
{
    public class LocalMedicao
    {
        public long IdLocalMedicao {get;set;}
        public string DescLocalMedicao {get;set;}
        
    }
public class LocalMedicaoModel
    {        
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(LocalMedicaoModel));
        public IEnumerable<LocalMedicao> SelectLocalMedicao(IConfiguration _configuration)
        {            
                try
                {
                    string sSql = string.Empty;

                    sSql = "SELECT IdLocalMedicao,DescLocalMedicao";
                    sSql = sSql +  " FROM TB_LOCAL_MEDICAO";
             
                    
                    IEnumerable<LocalMedicao> locais;
         
                    using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_ProjectCleanning_Sala_Limpa")))
                    {
                        locais = db.Query<LocalMedicao>(sSql,commandTimeout:0);
                    }
                                        
                    return locais;
                }
                catch (Exception ex)
                {
                    log.Error("Erro LocalMedicaoModel-SelectLimpezaMedicoes:" + ex.Message.ToString());
                    return null;
                }
        }
    }
}