using System;
using System.Collections.Generic;
using System.Reflection;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ProjectCleanning_Backend.Models;

namespace ProjectCleanning_Backend.Models
{
    public class Limpeza
    {
        public long IdApontLimpeza { get; set; }        
        public long IdUsuarioVezani { get; set; }
        public long IdUsuarioProjectCleanning { get; set; }
        public long  IdLocalMedicao {get;set;}
        public string CodUsuario { get; set; }
        public string DescLocalMedicao { get; set; }
        public string TipoControle { get; set; }
        public string MesControle {get;set;}
        public DateTime? DtMedicao{get;set;}
        public DateTime? DtOcorrencia{get;set;}
        public string FatoOcorrencia{get;set;}
        public string AcoesObservacoes{get;set;}
        public string Status {get;set;}
        public bool CorDashboard{get;set;}
    }
    
    public class LimpezaApontamento
    {
        public long IdApontLimpeza { get; set; }
        public long IdUsuarioVezani { get; set; }
        public long IdUsuarioProjectCleanning { get; set; }
        public string UserVz{get;set;}
        public string UserEb{get;set;}
        public long  IdLocalMedicao {get;set;}
        public string CodUsuario { get; set; }
        public string DescLocalMedicao { get; set; }
        public string TipoControle { get; set; }
        public string MesControle {get;set;}
        public DateTime? DtMedicao{get;set;}
        public DateTime? DtOcorrencia{get;set;}
        public string FatoOcorrencia{get;set;}
        public string AcoesObservacoes{get;set;}
        public string Status {get;set;}

        public IEnumerable<LimpezaMedicoes> ApontamentoMedicoes{get;set;}
    }

     public class LimpezaReport
    {
        public long IdApontLimpeza { get; set; }
        public long IdUsuarioVezani { get; set; }
        public long IdUsuarioProjectCleanning { get; set; }
        public string UsuarioVezani {get;set;}
        public string UsuarioProjectCleanning {get;set;}
        public long IdLocalMedicao {get;set;}
        public string DescLocalMedicao {get;set;}
        public string TipoControle {get;set;}
        public string MesControle {get;set;}
        public DateTime? DtMedicao {get;set;}
        public DateTime? DtOcorrencia {get;set;}
        public string FatoOcorrencia {get;set;}
        public string AcoesObservacoes {get;set;}
        public string Status {get;set;}
        public string IdMedicaoLimpeza {get;set;}
        public long IdCadParametroLimpeza {get;set;}
        public string DescOQue {get;set;}
        public string DescMetodoLimpeza {get;set;}
        public string Atividade_Concluida {get;set;}
    }
    public class LimpezaModel
    {        
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(LimpezaModel));

        LimpezaMedicoesModel _lm = new LimpezaMedicoesModel();
        public IEnumerable<LimpezaApontamento> SelectLimpeza(IConfiguration _configuration, long? id, string dtIni, string dtFim,string Status,bool Ocorrencia)
        {            
                try
                {
                    string sSql = string.Empty;

                    sSql = "SELECT IdApontLimpeza,IdUsuarioVezani,U.CodUsuario AS UserVz,EB.CodUsuario AS UserEb,IdUsuarioProjectCleanning,A.IdLocalMedicao,DescLocalMedicao,TipoControle,MesControle,DtMedicao,DtOcorrencia";
                    sSql = sSql + ",FatoOcorrencia,AcoesObservacoes,A.Status FROM TB_APONT_LIMPEZA A";                    
                    sSql = sSql + " INNER JOIN TB_USUARIO U ON A.IdUsuarioVezani = U.IdUsuario";
                    sSql = sSql + " INNER JOIN TB_USUARIO EB ON A.IdUsuarioProjectCleanning = EB.IdUsuario";
                    sSql = sSql + " INNER JOIN TB_LOCAL_MEDICAO L ON A.IdLocalMedicao = L.IdLocalMedicao";
                    sSql = sSql + " WHERE 1=1";
                    
                    if (id!=null && id!=0)
                        sSql = sSql + " AND A.IdApontLimpeza=" + id;

                    if (dtIni!=null && dtIni!="" && dtFim!=null && dtFim!="")
                       sSql = sSql + " AND DtMedicao BETWEEN '" + dtIni + "' AND '" + dtFim + "'";

                    if(Status!=null && Status!="")
                        sSql = sSql + " AND A.Status='" + Status + "'";
                    
                    if (Ocorrencia)
                        sSql=sSql + " AND DtOcorrencia IS NULL";

                    IEnumerable <LimpezaApontamento> Apontamentos;
                    using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_ProjectCleanning_Sala_Limpa")))
                    {
                        
                        Apontamentos = db.Query<LimpezaApontamento>(sSql,commandTimeout:0);
                    }

                    foreach(var item in Apontamentos)
                    {
                        IEnumerable<LimpezaMedicoes> medicoes = _lm.SelectLimpezaMedicoes(_configuration,item.IdApontLimpeza).Item1;
                        item.ApontamentoMedicoes= medicoes;       
                    }                   
                    return Apontamentos;
                }
                catch (Exception ex)
                {
                    log.Error("Erro LimpezaModel-SelectLimpeza:" + ex.Message.ToString());
                    return null;
                }
        }

        public IEnumerable<Limpeza> SelectLimpezaDashboard(IConfiguration _configuration, long IdLocalMedicao)
        {            
                try
                {
                    string sSql = string.Empty;

                    sSql = "SELECT TOP 5000 IdApontLimpeza,IdUsuarioVezani,IdUsuarioProjectCleanning,IdLocalMedicao,TipoControle,MesControle,DtMedicao,DtOcorrencia";
                    sSql = sSql + ",FatoOcorrencia,AcoesObservacoes,Status FROM TB_APONT_LIMPEZA";
                    sSql = sSql + " WHERE IdLocalMedicao=" + IdLocalMedicao + " AND Status='Finalizado'";
                    sSql = sSql + " ORDER BY DtMedicao DESC "; 


                    IEnumerable <Limpeza> _limpezas;
                    using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_ProjectCleanning_Sala_Limpa")))
                    {
                        
                        _limpezas = db.Query<Limpeza>(sSql,commandTimeout:0);
                    }
                    
                    return _limpezas;
                }
                catch (Exception ex)
                {
                    log.Error("Erro LimpezaModel-SelectLimpezaDashboard:" + ex.Message.ToString());
                    return null;
                }
        }
        public IEnumerable<LimpezaReport> SelectLimpezaReport(IConfiguration _configuration, long? IdLocalMedicao,string dtIni, string dtFim,string TipoControle)
        {            
                try
                {
                    string sSql = string.Empty;

                    sSql = "SELECT IdApontLimpeza,IdUsuarioVezani,UsuarioVezani,IdUsuarioProjectCleanning,UsuarioProjectCleanning,IdLocalMedicao,DescLocalMedicao";
                    sSql += ",TipoControle,MesControle,DtMedicao,DtOcorrencia,FatoOcorrencia,AcoesObservacoes,Status,IdMedicaoLimpeza";
                    sSql += ",IdCadParametroLimpeza,DescOQue,DescMetodoLimpeza,Atividade_Concluida";
                    sSql += " FROM VW_APONTAMENTO_LIMPEZA";
                    sSql += " WHERE 1=1";

                    if (IdLocalMedicao!=null && IdLocalMedicao!=0)
                        sSql += " AND IdLocalMedicao=" + IdLocalMedicao;

                    if (dtIni!=null && dtIni!="" && dtFim!=null && dtFim!="")
                       sSql += " AND DtMedicao BETWEEN '" + dtIni + "' AND '" + dtFim + "'";

                    if (TipoControle!=null && TipoControle!="")
                        sSql += " AND TipoControle='" + TipoControle + "'";

                    IEnumerable <LimpezaReport> _report;
                    using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_ProjectCleanning_Sala_Limpa")))
                    {                        
                        _report = db.Query<LimpezaReport>(sSql,commandTimeout:0);
                    }
                    
                    return _report;
                }
                catch (Exception ex)
                {
                    log.Error("Erro LimpezaModel-SelectLimpezaReport:" + ex.Message.ToString());
                    return null;
                }
        }
        public bool UpdateLimpeza (IConfiguration _configuration,Limpeza _limpeza)
        {
            try{
                string sSql = string.Empty;


                sSql = "UPDATE TB_APONT_LIMPEZA SET";                
                sSql=sSql+ "[DtOcorrencia]=" +( (_limpeza.DtOcorrencia==null) ? "NULL" : "GETDATE()");
                sSql=sSql+ ",[FatoOcorrencia]='"+ _limpeza.FatoOcorrencia + "'";
                sSql=sSql+ ",[AcoesObservacoes]='"+ _limpeza.AcoesObservacoes + "'";
                sSql=sSql+ ",[Status]='" + _limpeza.Status + "'";
                sSql =sSql+ " WHERE IdApontLimpeza=" + _limpeza.IdApontLimpeza;

                long update = 0;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_ProjectCleanning_Sala_Limpa")))
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
                log.Error("Erro LimpezaModel-UpdateLimpeza:" + ex.Message.ToString());
                return false;
            }
        }
        public bool InsertLimpeza(IConfiguration _configuration,Limpeza _limpeza)
        {
            
            string sSql = string.Empty;
            try
            {
                string dataOcorrencia = (_limpeza.DtOcorrencia==null) ? "NULL" : _limpeza.DtOcorrencia.Value.ToString();
                
                sSql= "INSERT INTO TB_APONT_LIMPEZA (IdUsuarioProjectCleanning,IdUsuarioVezani,IdLocalMedicao,TipoControle,MesControle,DtMedicao,Status,DtOcorrencia,FatoOcorrencia,AcoesObservacoes)";
                sSql = sSql + " VALUES ";
                sSql = sSql + "(" + _limpeza.IdUsuarioProjectCleanning;
                 sSql = sSql + "," + _limpeza.IdUsuarioVezani;
                sSql = sSql + "," + _limpeza.IdLocalMedicao;
                sSql = sSql + ",'" + _limpeza.TipoControle + "'";
                sSql = sSql + ",'" + _limpeza.MesControle + "'";
                sSql = sSql + ", GETDATE()";
                sSql = sSql + ",'" + _limpeza.Status + "',NULL,NULL,NULL)";
                sSql = sSql + " SELECT @@IDENTITY";

                long insertId=0;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_ProjectCleanning_Sala_Limpa")))
                {
                   insertId =db.QueryFirstOrDefault<long>(sSql,commandTimeout:0);
                }
                if(insertId>0)
                {
                    _limpeza.IdApontLimpeza=insertId;
                    return true;
                }
                return false;
            }
            
            catch(Exception ex)
            {
                log.Error("Erro LimpezaModel-InsertLimpeza:" + ex.Message.ToString());
                return false;
            }
        }
    }
}