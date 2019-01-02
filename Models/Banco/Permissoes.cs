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
    public class Permissoes
    {
        public string CodUsuario {get;set;}     
        public long IdFuncaoSistema{get;set;}
        public string DescFuncaoSistema{get;set;}
    }
    public class PermissoesModel
    {
         private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(PermissoesModel));
        
        public IEnumerable<Permissoes> SelectPermissoes(IConfiguration _configuration, string CodUsuario)
        {            
            try
            {
                string sSql = string.Empty;

                sSql = "SELECT CodUsuario,IdFuncaoSistema,DescFuncaoSistema";
                sSql = sSql + " FROM VW_PERMISSOES_USUARIO";
                sSql = sSql + " WHERE CodUsuario='" + CodUsuario + "'";
                
                IEnumerable <Permissoes> permissoes;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                    permissoes = db.Query<Permissoes>(sSql,commandTimeout:0);
                }                 
                return permissoes;
            }
            catch (Exception ex)
            {
                log.Error("Erro PermissoesModel-SelectPermissoes:" + ex.Message.ToString());
                return null;
            }
        }
    }
}