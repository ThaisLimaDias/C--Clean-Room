using System;
using System.Collections.Generic;
using System.Reflection;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Embraer_Backend.Models
{
    public class Usuario
    {
        public long IdUsuario { get; set; }
        public string Nome {get; set;}
        public string CodUsuario { get; set; }
        public string Senha {get; set;}
        public string Funcao {get; set;}
        public string NumChapa {get;set;}
        public string Status {get; set;}
    }
    
    public class UsuarioModel
    {        
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(UsuarioModel));
        public IEnumerable<Usuario> SelectUsuario(IConfiguration _configuration, string codUsuario,string status)
        {            
                try
                {
                    string sSql = string.Empty;

                    sSql = "SELECT IdUsuario,Nome,CodUsuario,Senha,Funcao FROM TB_USUARIO WHERE 1=1";

                    if (codUsuario!="" && codUsuario!=null)
                        sSql += " AND CodUsuario='"+ codUsuario+"'"; 

                    if (status!="" && status!=null)
                        sSql += " AND Status='"+ status+"'"; 
                    
                    IEnumerable<Usuario> usuarios;
         
                    using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                    {
                        usuarios = db.Query<Usuario>(sSql,commandTimeout:0);
                    }

                    return usuarios;
                }
                catch (Exception ex)
                {
                    log.Error("Erro UsuarioModel-SelectUsuario:" + ex.Message.ToString());
                    return null;
                }
        }

        public bool UpdateUsuario (IConfiguration _configuration,Usuario _user)
        {
            try{
                string sSql = string.Empty;

                sSql = "UPDATE TB_USUARIO SET";
                sSql+="[Nome]='"+ _user.Nome + "'";
                sSql+=",[Senha]='"+ _user.Senha + "'";
                sSql+=",[Funcao]='" + _user.Funcao + "'";
                sSql+=",[NumChapa]='" + _user.NumChapa + "'";
                sSql+=",[Status]='" + _user.Status + "'";
                sSql+= "WHERE IdUsuario=" + _user.IdUsuario;

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
                log.Error("Erro UsuarioModel-UpdateUsuario:" + ex.Message.ToString());
                return (false);
            }
        }
        public bool InsertUsuario(IConfiguration _configuration, Usuario _user)
        {
            
            string sSql = string.Empty;
            try
            {
                sSql= "INSERT INTO TB_USUARIO ([Nome],[CodUsuario],[Senha],[Funcao],[NumChapa],[Status])";
                sSql += "VALUES";
                sSql += "('" + _user.Nome + "'";
                sSql += ",'" + _user.CodUsuario + "'";
                sSql += ",'" +  _user.Senha + "'";
                sSql += ",'" + _user.Funcao + "'";
                sSql += ",'" + _user.NumChapa + "'";
                sSql += ",'Bloqueado')";
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
                log.Error("Erro UsuarioModel-InsertUsuario:" + ex.Message.ToString());
                return (false);
            }


        }
        public bool DeleteUsuario (IConfiguration _configuration,string CodUsuario)
        {            
            try
            {
                string sSql = string.Empty;

                sSql = "UPDATE TB_USUARIO SET [Status]='Inativo' WHERE CodUsuario=" + CodUsuario;

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
                log.Error("Erro UsuarioModel-DeleteUsuario:" + ex.Message.ToString());
                return (false);
            }      

    }
}
}