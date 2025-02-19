using System;
using System.Collections.Generic;
using System.Reflection;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ProjectCleanning_Backend.Models
{
    public class LimpezaMedicoes
    {
        public long IdMedicaoLimpeza {get;set;}
        public long IdApontLimpeza {get;set;}
        public long IdCadParametroLimpeza {get;set;}
        public string DescOQue {get;set;}
        public string DescMetodoLimpeza {get;set;}
        public bool Realizado {get;set;}
 
    }

    public class LimpezaMedicoesModel
    {        
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(LimpezaMedicoesModel));
        public (IEnumerable<LimpezaMedicoes>,string) SelectLimpezaMedicoes(IConfiguration _configuration, long idApontLimpeza)
        {            
                try
                {
                    string sSql = string.Empty;

                    sSql = "SELECT M.IdMedicaoLimpeza,M.IdApontLimpeza,P.IdCadParametroLimpeza,P.DescOQue,P.DescMetodoLimpeza,M.Realizado";
                    sSql = sSql +  " FROM TB_MEDICAO_LIMPEZA M";
                    sSql = sSql +  " INNER JOIN TB_CADASTRO_PARAMETROS_LIMPEZA P ON M.IdCadParametroLimpeza = P.IdCadParametroLimpeza";
                    sSql = sSql + " WHERE IdApontLimpeza=" + idApontLimpeza;
             
                    
                    IEnumerable<LimpezaMedicoes> medicoes;
         
                    using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_ProjectCleanning_Sala_Limpa")))
                    {
                        medicoes = db.Query<LimpezaMedicoes>(sSql,commandTimeout:0);
                    }
                                        
                    return (medicoes,string.Empty);
                }
                catch (Exception ex)
                {
                    log.Error("Erro LimpezaMedicoesModel-SelectLimpezaMedicoes:" + ex.Message.ToString());
                    return (null,"SelectLimpezaMedicoes: Ocorreu um erro ao pesquisar informações no banco de dados");
                }
        }
        public (bool,string) UpdateLimpezaMedicoes (IConfiguration _configuration,LimpezaMedicoes _medicoes)
        {
            try
            {
                string sSql = string.Empty;
                sSql = "UPDATE TB_MEDICAO_LIMPEZA SET";
                sSql = sSql + " Realizado='"+ _medicoes.Realizado + "'";
                sSql = sSql + " WHERE IdMedicaoLimpeza=" + _medicoes.IdMedicaoLimpeza;

                long update = 0;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_ProjectCleanning_Sala_Limpa")))
                {
                    update =db.Execute(sSql);
                }
                if(update>0)
                {                    
                    return (true, string.Empty);
                }
                return (false,string.Empty);

            }
            catch(Exception ex)
            {
                log.Error("Erro LimpezaMedicoesModel-UpdateLimpezaMedicoes:" + ex.Message.ToString());
                return (false,"UpdateLimpezaMedicoes: Ocorreu um erro ao atualizar informações no banco de dados");
            }
        }
        
        public bool InsertLimpeza(IConfiguration _configuration,LimpezaMedicoes _medicoes)
        {        
            string sSql = string.Empty;
            try
            {
                sSql= "INSERT INTO TB_MEDICAO_LIMPEZA (IdApontLimpeza,IdCadParametroLimpeza,Realizado)";
                sSql = sSql + " VALUES ";
                sSql = sSql + "(" + _medicoes.IdApontLimpeza;
                sSql = sSql + "," + _medicoes.IdCadParametroLimpeza;
                sSql = sSql + ",'" + _medicoes.Realizado + "')";
                sSql = sSql + " SELECT @@IDENTITY";

                long insertId = 0;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_ProjectCleanning_Sala_Limpa")))
                {
                    insertId =db.QueryFirstOrDefault<long>(sSql,commandTimeout:0);
                }
                if(insertId>0)
                {
                    _medicoes.IdMedicaoLimpeza=insertId;
                    return true;
                }
                return false;
            }
         
            catch(Exception ex)
            {
                log.Error("Erro LimpezaMedicoesModel-InsertLimpezaMedicoes:" + ex.Message.ToString());
                return false;
            }
        }
    }
}