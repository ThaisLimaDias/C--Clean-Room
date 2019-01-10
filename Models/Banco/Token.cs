using System;
using System.Collections.Generic;
using System.Reflection;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Embraer_Backend.Models
{
    public class Token
    {
        public long IdToken { get; set; }
        public long IdUsuario {get; set;}
        public string TokenUser { get; set; }
    }
    
    public class TokenModel
    {        
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(TokenModel));
        public Token SelectToken(IConfiguration _configuration, long IdUsuario)
        {            
                try
                {
                    string sSql = string.Empty;

                    sSql = "SELECT IdToken,IdUsuario,TokenUser FROM TB_TOKEN WHERE IdUsuario=" + IdUsuario; 

                    Token token;
         
                    using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                    {
                        token = db.QueryFirstOrDefault<Token>(sSql,commandTimeout:0);
                    }

                    return token;
                }
                catch (Exception ex)
                {
                    log.Error("Erro TokenModel-SelectToken:" + ex.Message.ToString());
                    return null;
                }
        }

        public bool UpdateToken (IConfiguration _configuration,Token _tk)
        {
            try{
                string sSql = string.Empty;

                sSql = "UPDATE TB_TOKEN SET";
                sSql+=" [IdUsuario]="+ _tk.IdUsuario;
                sSql+=",[TokenUser]='"+ _tk.TokenUser + "'";
                sSql+= " WHERE IdToken=" + _tk.IdToken;

                int update = 0;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                    update = db.Execute(sSql,commandTimeout:0);
                }
                if(update<=0)
                {
                    return (false);
                }
                return (true);

            }
            catch(Exception ex)
            {
                log.Error("Erro TokenModel-UpdateToken:" + ex.Message.ToString());
                return (false);
            }
        }
        public bool InsertToken(IConfiguration _configuration, Token _tk)
        {
            
            string sSql = string.Empty;
            try
            {
                sSql= "INSERT INTO TB_TOKEN ([IdUsuario],[TokenUser])";
                sSql += "VALUES";
                sSql += "('" + _tk.IdUsuario + "'";
                sSql += ",'" + _tk.TokenUser + "')";
                sSql += "SELECT @@IDENTITY";

                long insertId = 0;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                   insertId =db.QueryFirstOrDefault<long>(sSql,commandTimeout:0);
                }
                if(insertId>0)
                {
                    return (true);
                }
                return (false);
            }
            
            catch(Exception ex)
            {
                log.Error("Erro TokenModel-InsertToken:" + ex.Message.ToString());
                return (false);
            }


        }
        public bool DeleteToken (IConfiguration _configuration,long IdUsuario)
        {            
            try
            {
                string sSql = string.Empty;

                sSql = "DELETE TB_TOKEN WHERE IdUsuario=" + IdUsuario;

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
                log.Error("Erro TokenModel-DeleteToken:" + ex.Message.ToString());
                return (false);
            }      

    }
}
}