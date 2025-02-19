using System;
using System.Collections.Generic;
using System.Reflection;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace C_ProjectCleanning_Clean_Room.Models.Banco
{
    public class Liberar
    {
        public long IdUsuario { get; set; }

        public long IdNivelAcesso { get; set; }

        public long CodUsuario { get; set; }

        public long? Senha { get; set; }

        public long Nome { get; set; }

        public long NumChapa { get; set; }

        public long Status { get; set; }
    
    }

    public class LiberarModel{

    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(GruposModel));

    public IEnumerable<Liberar> SelectUsuarios(IConfiguration _configuration, Liberar _liberar)
        {            
                try
                {
                    string sSql = string.Empty;

                    sSql = "SELECT IdUsuario,IdNivelAcesso,CodUsuario,Nome,Status,NumChapa FROM TB_USUARIO"; 
                    
                    IEnumerable<Liberar> liberar;
         
                    using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_ProjectCleanning_Sala_Limpa")))
                    {
                        liberar = db.Query<Liberar>(sSql,commandTimeout:0);
                    }

                    return liberar;
                }
                catch (Exception ex)
                {
                    log.Error("Erro GruposModel-SelectGrupo:" + ex.Message.ToString());
                    return null;
                }
        }

    }
}