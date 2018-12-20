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
        public string Status {get; set;}
    }
    
    public class UsuarioModel
    {        
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(UsuarioModel));
        public IEnumerable<Usuario> SelectUsuario(IConfiguration _configuration, string codUsuario)
        {            
                try
                {
                    string sSql = string.Empty;

                    sSql = "SELECT IdUsuario,Nome,CodUsuario,Senha,Funcao FROM TB_USUARIO WHERE CodUsuario='"+ codUsuario+"'"; 
                    
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

                sSql = "UPDATE SPI_DB_EMBRAER_COLETA SET";
                sSql+="[Nome]='"+ _user.Nome + "'";
                sSql+="[CodUsuario]='" + _user.CodUsuario + "'";
                sSql+="[Senha]='"+ _user.Senha + "'";
                sSql+="[Funcao]='" + _user.Funcao + "'";
                sSql+="[Status]='" + _user.Status + "'";
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
        public bool InsertUsuario(IConfiguration _configuration, string Nome,string codUsuario, string Senha,string Funcao )
        {
            
            string sSql = string.Empty;
            try
            {
                sSql= "INSERT INTO TB_USUARIO ([Nome],[CodUsuario],[Senha],[Funcao],[Status])";
                sSql += "VALUES";
                sSql += "('" + Nome + "'";
                sSql += ",'" + codUsuario + "'";
                sSql += ",'" +  Senha + "'";
                sSql += ",'" +  Funcao + "'";
                sSql += ",'Bloqueado')";
                sSql += "SELECT @@IDENTITY";

                long insertId = 0;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                    insertId =db.Query<long>(sSql).GetEnumerator().Current;
                }
                if(insertId>=0)
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
        public bool DeleteUsuario (IConfiguration _configuration,long IdUsuario)
        {            
                try{
                string sSql = string.Empty;

                sSql = "UPDATE TB_USUARIO SET";
                sSql+="[Status]='Bloqueado'";
                sSql+= " WHERE IdUsuario=" + IdUsuario;

                int update = 0;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("SPI_DB_EMBRAER_COLETA")))
                {
                    update = db.Execute(sSql,commandTimeout:0);
                }
                if(update<=0)
                {
                    return (false);
                }

                

                    return(true);
            }
            catch(Exception ex)
            {
                log.Error("Erro GruposModel-UpdateGrupos:" + ex.Message.ToString());
                return (false);
            }

        

    }
}
}