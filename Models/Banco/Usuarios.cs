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
        public string CodUsuario { get; set; }
        public string Nome {get;set;}
        public string NumChapa { get; set; }
        public string IdNivelAcesso {get;set;}
        public string Status{get;set;}
    }
    
    public class UsuarioModel
    {        
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(UsuarioModel));
        public IEnumerable<Usuario> SelectUsuario(IConfiguration _configuration, string codUser)
        {            
                try
                {
                    string sSql = string.Empty;

                    sSql = "SELECT IdUsuario,CodUsuario,Nome,NumChapa,IdNivelAcesso,Status FROM SPI_DB_EMBRAER_COLETA WHERE CodUsuario='"+ codUser + "'"; 
                    
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

        public (bool,string) UpdateUsuario (IConfiguration _configuration,Usuario _user)
        {
            try{
                string sSql = string.Empty;

                sSql = "UPDATE SPI_DB_EMBRAER_COLETA SET";
                sSql=sSql="[CodUsuario]='"+ _user.CodUsuario + "'";
                sSql=sSql="[Nome]='"+ _user.Nome + "'";
                sSql=sSql="[NumChapa]='"+ _user.NumChapa + "'";
                sSql=sSql="[IdNivelAcesso]='" + _user.IdNivelAcesso + "'";
                sSql=sSql="[Status]='" + _user.Status + "'";
                sSql = "WHERE IdUsuario=" + _user.IdUsuario;

                int update = 0;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                    update = db.Execute(sSql,commandTimeout:0);
                }
                if(update<=0)
                {
                    return (false, string.Empty);
                }
                return (true,string.Empty);

            }
            catch(Exception ex)
            {
                log.Error("Erro UsuarioModel-UpdateUsuario:" + ex.Message.ToString());
                return (false,"UpdateUsuario: Ocorreu um erro ao atualizar informações no banco de dados");
            }
        }
        public (bool,string) InsertUsuario(IConfiguration _configuration,Usuario _user)
        {
            
            string sSql = string.Empty;
            try
            {
                sSql= "INSERT INTO SPI_DB_EMBRAER_COLETA ([CodUsuario],[Nome],[NumChapa],[IdNivelAcesso],[Status])";
                sSql = "VALUES";
                sSql = "('" + _user.CodUsuario + "'";
                sSql = ",'" + _user.Nome + "'";
                sSql = ",'" + _user.NumChapa + "'";
                sSql = "," + _user.IdNivelAcesso;
                sSql = ",'" + _user.Status + "')";
                sSql = sSql + "SELECT @@IDENTITY";

                long insertId = 0;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                    insertId =db.Query<long>(sSql).GetEnumerator().Current;
                }
                if(insertId>=0)
                {
                    _user.IdUsuario=insertId;
                    return (true, string.Empty);
                }
                return (false,string.Empty);
            }
            
            catch(Exception ex)
            {
                log.Error("Erro UsuarioModel-InsertUsuario:" + ex.Message.ToString());
                return (false,"InsertUsuario: Ocorreu um erro ao inserir informações no banco de dados");
            }
        }
    }
}