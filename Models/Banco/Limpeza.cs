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
    public class Limpeza
    {
        public long IdApontLimpeza { get; set; }
        public long IdUsuario { get; set; }
        public long  IdLocalMedicao {get;set;}
        public string TipoControle { get; set; }
        public string MesControle {get;set;}
        public DateTime? DtMedicao{get;set;}
        public DateTime? DtOcorrencia{get;set;}
        public string FatoOcorrencia{get;set;}
        public string AcoesObservacoes{get;set;}
        public string Status {get;set;}
    }
    
    public class LimpezaApontamento
    {
        public long IdApontLimpeza { get; set; }
        public long IdUsuario { get; set; }
        public long  IdLocalMedicao {get;set;}
        public string TipoControle { get; set; }
        public string MesControle {get;set;}
        public DateTime DtMedicao{get;set;}
        public DateTime DtOcorrencia{get;set;}
        public string FatoOcorrencia{get;set;}
        public string AcoesObservacoes{get;set;}
        public string Status {get;set;}

        public IEnumerable<LimpezaMedicoes> ApontamentoMedicoes{get;set;}
    }

    public class LimpezaModel
    {        
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(LimpezaModel));

        LimpezaMedicoesModel _lm = new LimpezaMedicoesModel();
        public IEnumerable<LimpezaApontamento> SelectLimpeza(IConfiguration _configuration, long? id, string dtIni, string dtFim,string Status)
        {            
                try
                {
                    string sSql = string.Empty;

                    sSql = "SELECT IdApontLimpeza,IdUsuario,IdLocalMedicao,TipoControle,MesControle,DtMedicao,DtOcorrencia";
                    sSql = sSql + ",FatoOcorrencia,AcoesObservacoes,Status FROM TB_APONT_LIMPEZA";
                    sSql = sSql + " WHERE 1=1";
                    
                    if (id!=null)
                        sSql = sSql + " AND IdApontLimpeza=" + id;

                    if (dtIni!=null && dtIni!="" && dtFim!=null && dtFim!="")
                       sSql = sSql + " AND DtMedicao BETWEEN " + dtIni + " AND " + dtFim + "";

                    if(Status!=null && Status!="")
                        sSql = sSql + " AND Status='" + Status + "'";

                    IEnumerable <LimpezaApontamento> Apontamentos;
                    using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
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

                    sSql = "SELECT TOP 5000 IdApontLimpeza,IdUsuario,IdLocalMedicao,TipoControle,MesControle,DtMedicao,DtOcorrencia";
                    sSql = sSql + ",FatoOcorrencia,AcoesObservacoes,Status FROM TB_APONT_LIMPEZA";
                    sSql = sSql + " WHERE IdLocalMedicao=" + IdLocalMedicao + " AND Status='Finalizado'";
                    sSql = sSql + " ORDER BY DtMedicao DESC "; 


                    IEnumerable <Limpeza> _limpezas;
                    using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
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

        public bool UpdateLimpeza (IConfiguration _configuration,Limpeza _limpeza)
        {
            try{
                string sSql = string.Empty;

                string dataOcorrencia = (_limpeza.DtOcorrencia==null) ? "NULL" : _limpeza.DtOcorrencia.Value.ToString("yyyy-MM-ddTHH:mm:ss");

                sSql = "UPDATE TB_APONT_LIMPEZA SET";                
                sSql=sSql+ ((dataOcorrencia=="NULL")? "[DtOcorrencia]="+  dataOcorrencia + "" : "[DtOcorrencia]='"+  dataOcorrencia + "'");
                sSql=sSql+ ",[FatoOcorrencia]='"+ _limpeza.FatoOcorrencia + "'";
                sSql=sSql+ ",[AcoesObservacoes]='"+ _limpeza.AcoesObservacoes + "'";
                sSql=sSql+ ",[Status]='" + _limpeza.Status + "'";
                sSql =sSql+ " WHERE IdApontLimpeza=" + _limpeza.IdApontLimpeza;

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
                log.Error("Erro LimpezaModel-UpdateLimpeza:" + ex.Message.ToString());
                return false;
            }
        }
        public bool InsertLimpeza(IConfiguration _configuration,Limpeza _limpeza)
        {
            
            string sSql = string.Empty;
            try
            {
                string dataOcorrencia = (_limpeza.DtOcorrencia==null) ? "NULL" : _limpeza.DtOcorrencia.Value.ToString("yyyy-MM-ddTHH:mm:ss");
                
                sSql= "INSERT INTO TB_APONT_LIMPEZA (IdUsuario,IdLocalMedicao,TipoControle,MesControle,DtMedicao,Status,DtOcorrencia,FatoOcorrencia,AcoesObservacoes)";
                sSql = sSql + " VALUES ";
                sSql = sSql + "(" + _limpeza.IdUsuario;
                sSql = sSql + "," + _limpeza.IdLocalMedicao;
                sSql = sSql + ",'" + _limpeza.TipoControle + "'";
                sSql = sSql + ",'" + _limpeza.MesControle + "'";
                sSql = sSql + ", GETDATE()";
                sSql = sSql + ",'" + _limpeza.Status + "'";
                sSql = sSql + ((dataOcorrencia=="NULL")? ","+  dataOcorrencia + "" : ",'"+  dataOcorrencia + "'");
                sSql = sSql +  "," + ((_limpeza.FatoOcorrencia==null) ? "NULL" : ( "'" + _limpeza.FatoOcorrencia +"'"));
                sSql = sSql +  "," + ((_limpeza.AcoesObservacoes==null) ? "NULL" : ( "'" + _limpeza.AcoesObservacoes.ToString() + "'") ) + ")";
                sSql = sSql + " SELECT @@IDENTITY";

                long insertId=0;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
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