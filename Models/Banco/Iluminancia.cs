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
    public class Iluminancia
    {
        public long IdApontIluminancia { get; set; }
        public long IdUsuario { get; set; }
        public string CodUsuario { get; set; }
        public string DescLocalMedicao { get; set; }
        public long  IdLocalMedicao {get;set;}
        public string DescEquip { get; set; }
        public long  IdEquip {get;set;}
        public DateTime? DtMedicao{get;set;}
        public DateTime? DtOcorrencia{get;set;}
        public string FatoOcorrencia{get;set;}
        public string AcoesObservacoes{get;set;}
    }

    public class IluminanciaMedicoes
    {
        public long IdMedicaoIluminancia {get;set;}
        public long IdApontIluminancia {get;set;}
        public string DescPonto {get;set;}
        public decimal Valor {get;set;}
        public long IdCadParametroSistema {get;set;}
        public decimal ControleMin {get;set;}
        public decimal EspecificacaoMin {get;set;}
        public decimal EspecificacaoMax {get;set;}
        public decimal ControleMax {get;set;}

    }

    public class IluminanciaReport
    {
        public string CodUsuario { get; set; }
        public string DescLocalMedicao { get; set; }
        public string DescEquip { get; set; }
         public DateTime? DtMedicao{get;set;}
        public DateTime? DtOcorrencia{get;set;}
        public string FatoOcorrencia{get;set;}
        public string AcoesObservacoes{get;set;}
        public string DescPonto {get;set;}
        public decimal Valor {get;set;}        
        public decimal EspecificacaoMin {get;set;}
        public decimal EspecificacaoMax {get;set;}

    }
    public class IluminanciaModel
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(IluminanciaModel));
        
        public IEnumerable<Iluminancia> SelectIluminancia(IConfiguration _configuration, long id, string dtIni, string dtFim,long IdLocalMedicao, bool?  Ocorrencia)
        {            
            try
            {
                string sSql = string.Empty;

                sSql = "SELECT IdApontIluminancia,CodUsuario,A.IdUsuario,DescLocalMedicao,A.IdLocalMedicao,DescEquip,A.IdEquip,DtMedicao,DtOcorrencia,FatoOcorrencia,AcoesObservacoes";
                sSql = sSql + " FROM TB_APONT_ILUMINANCIA A";
                sSql = sSql + " INNER JOIN TB_USUARIO U ON A.IdUsuario = U.IdUsuario";
                sSql = sSql + " INNER JOIN TB_LOCAL_MEDICAO L ON A.IdLocalMedicao = L.IdLocalMedicao";
                sSql = sSql + " INNER JOIN TB_EQUIPAMENTOS E ON A.IdEquip = E.IdEquip";
                sSql = sSql + " WHERE 1=1";
                
                if (id!=0)
                    sSql = sSql + " AND A.IdApontIluminancia=" + id;
                
                if (IdLocalMedicao!=0)
                    sSql = sSql + " AND A.IdLocalMedicao=" + IdLocalMedicao;

                if(dtIni !=null && dtIni!="" && dtFim!=null && dtFim!="")
                    sSql = sSql + " AND DtMedicao BETWEEN '" + dtIni + "' AND '" + dtFim + "'";

                if (Ocorrencia.Value)
                    sSql=sSql + " AND DtOcorrencia IS NULL";

                IEnumerable <Iluminancia> iluminancias;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                    iluminancias = db.Query<Iluminancia>(sSql,commandTimeout:0);
                }                 
                return iluminancias;
            }
            catch (Exception ex)
            {
                log.Error("Erro IluminanciaModel-SelectIluminancia:" + ex.Message.ToString());
                return null;
            }
        }
        public IEnumerable<Iluminancia> SelectIluminancia(IConfiguration _configuration,long IdLocalMedicao)
        {            
            try
            {
                string sSql = string.Empty;

                sSql = "SELECT TOP 1 IdApontIluminancia,CodUsuario,A.IdUsuario,DescLocalMedicao,A.IdLocalMedicao,DescEquip,A.IdEquip,DtMedicao,DtOcorrencia,FatoOcorrencia,AcoesObservacoes";
                sSql = sSql + " FROM TB_APONT_ILUMINANCIA A";
                sSql = sSql + " INNER JOIN TB_USUARIO U ON A.IdUsuario = U.IdUsuario";
                sSql = sSql + " INNER JOIN TB_LOCAL_MEDICAO L ON A.IdLocalMedicao = L.IdLocalMedicao";
                sSql = sSql + " INNER JOIN TB_EQUIPAMENTOS E ON A.IdEquip = E.IdEquip";
                sSql = sSql + " WHERE A.IdLocalMedicao=" + IdLocalMedicao;
                sSql = sSql + " ORDER BY DtMedicao DESC";

                
                IEnumerable <Iluminancia> iluminancias;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                    iluminancias = db.Query<Iluminancia>(sSql,commandTimeout:0);
                }                 
                return iluminancias;
            }
            catch (Exception ex)
            {
                log.Error("Erro IluminanciaModel-SelectIluminancia:" + ex.Message.ToString());
                return null;
            }
        }

        public IEnumerable<IluminanciaReport> IluminanciaReport(IConfiguration _configuration,long IdLocalMedicao,string dtIni, string dtFim)
        {            
            try
            {
                string sSql = string.Empty;

                sSql = "SELECT CodUsuario,DescLocalMedicao,DescEquip,DtMedicao,DtOcorrencia,FatoOcorrencia,AcoesObservacoes,DescPonto";
                sSql = sSql + ",Valor,EspecificacaoMin,EspecificacaoMax";          
                sSql = sSql + " FROM VW_REPORT_ILUMINANCIA";
                sSql = sSql + " WHERE 1=1";

                if (IdLocalMedicao!=0)
                    sSql = sSql + " AND IdLocalMedicao=" + IdLocalMedicao;

                if(dtIni !=null && dtIni!="" && dtFim!=null && dtFim!="")
                    sSql = sSql + " AND DtMedicao BETWEEN '" + dtIni + "' AND '" + dtFim + "'";


                sSql = sSql + " ORDER BY DtMedicao";

                
                IEnumerable <IluminanciaReport> iluminancias;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                    iluminancias = db.Query<IluminanciaReport>(sSql,commandTimeout:0);
                }                 
                return iluminancias;
            }
            catch (Exception ex)
            {
                log.Error("Erro IluminanciaModel-IluminanciaReport:" + ex.Message.ToString());
                return null;
            }
        }

        public bool InsertIluminancia(IConfiguration _configuration,Iluminancia _iluminancia)
        {
            
            string sSql = string.Empty;
            try
            {
                string dataOcorrencia = (_iluminancia.DtOcorrencia==null) ? "NULL" : _iluminancia.DtOcorrencia.Value.ToString("yyyy-MM-ddTHH:mm:ss");
                
                sSql= "INSERT INTO TB_APONT_ILUMINANCIA (IdUsuario,IdLocalMedicao,IdEquip,DtMedicao,DtOcorrencia,FatoOcorrencia,AcoesObservacoes)";
                sSql = sSql + " VALUES ";
                sSql = sSql + "(" + _iluminancia.IdUsuario;
                sSql = sSql + "," + _iluminancia.IdLocalMedicao;
                sSql = sSql + "," + _iluminancia.IdEquip;
                sSql = sSql + ", GETDATE()";
                sSql = sSql + ((dataOcorrencia=="NULL")? ","+  dataOcorrencia + "" : ",'"+  dataOcorrencia + "'");
                sSql = sSql +  "," + ((_iluminancia.FatoOcorrencia==null) ? "NULL" : ( "'" + _iluminancia.FatoOcorrencia +"'"));
                sSql = sSql +  "," + ((_iluminancia.AcoesObservacoes==null) ? "NULL" : ( "'" + _iluminancia.AcoesObservacoes.ToString() + "'") ) + ")";
                sSql = sSql + " SELECT @@IDENTITY";

                long insertId=0;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                   insertId =db.QueryFirstOrDefault<long>(sSql,commandTimeout:0);
                }
                if(insertId>0)
                {
                    _iluminancia.IdApontIluminancia=insertId;
                    return true;
                }
                return false;
            }
            
            catch(Exception ex)
            {
                log.Error("Erro IluminanciaModel-InsertIluminancia:" + ex.Message.ToString());
                return false;
            }
        }
       public bool Updateluminancia(IConfiguration _configuration,Iluminancia _iluminancia)
        {
            try{
                string sSql = string.Empty;

                string dataOcorrencia = (_iluminancia.DtOcorrencia==null) ? "NULL" : _iluminancia.DtOcorrencia.Value.ToString("yyyy-MM-ddTHH:mm:ss");

                sSql = "UPDATE TB_APONT_ILUMINANCIA SET";                
                sSql=sSql+ ((dataOcorrencia=="NULL")? "[DtOcorrencia]="+  dataOcorrencia + "" : "[DtOcorrencia]='"+  dataOcorrencia + "'");
                sSql=sSql+ ",[FatoOcorrencia]='"+ _iluminancia.FatoOcorrencia + "'";
                sSql=sSql+ ",[AcoesObservacoes]='"+ _iluminancia.AcoesObservacoes + "'";
                sSql =sSql+ " WHERE IdApontIluminancia=" + _iluminancia.IdApontIluminancia;

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
                log.Error("Erro IluminanciaModel-Updateluminancia:" + ex.Message.ToString());
                return false;
            }
        }
        public IEnumerable<IluminanciaMedicoes> SelectMedicaoIluminancia(IConfiguration _configuration, long idApont)
        {
            try
            {
                string sSql = string.Empty;

                sSql = "SELECT IdMedicaoIluminancia,IdApontIluminancia,DescPonto,Valor,IdCadParametroSistema,";
                sSql = sSql + "ControleMin,EspecificacaoMin,EspecificacaoMax,ControleMax";
                sSql = sSql + " FROM TB_MEDICAO_ILUMINANCIA";
                sSql = sSql + " WHERE IdApontIluminancia=" + idApont;  

                IEnumerable <IluminanciaMedicoes> medicoes;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                    medicoes = db.Query<IluminanciaMedicoes>(sSql,commandTimeout:0);
                }                   
                return medicoes;
            }
            catch (Exception ex)
            {
                log.Error("Erro IluminanciaModel-SelectMedicaoIluminancia:" + ex.Message.ToString());
                return null;
            }

        }       
        
        public bool InsertMedicaoIluminancia(IConfiguration _configuration,IluminanciaMedicoes _medicoes)
        {            
            string sSql = string.Empty;
            try
            {
                
                sSql= "INSERT INTO TB_MEDICAO_ILUMINANCIA (IdApontIluminancia,DescPonto,Valor,IdCadParametroSistema,";
                sSql = sSql + "ControleMin,EspecificacaoMin,EspecificacaoMax,ControleMax)";
                sSql = sSql + " VALUES ";
                sSql = sSql + "(" + _medicoes.IdApontIluminancia;
                sSql = sSql + ",'" + _medicoes.DescPonto + "'";
                sSql = sSql + ",'" + _medicoes.Valor.ToString().Replace(",",".") + "'";
                sSql = sSql + "," + _medicoes.IdCadParametroSistema ;
                sSql = sSql + ",'" + _medicoes.ControleMin.ToString().Replace(",",".") + "'";
                sSql = sSql + ",'" + _medicoes.EspecificacaoMin.ToString().Replace(",",".") + "'";
                sSql = sSql + ",'" + _medicoes.EspecificacaoMax.ToString().Replace(",",".") + "'";
                sSql = sSql + ",'" + _medicoes.ControleMax.ToString().Replace(",",".") + "')";
                sSql = sSql + " SELECT @@IDENTITY";

                long insertId=0;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                   insertId =db.QueryFirstOrDefault<long>(sSql,commandTimeout:0);
                }
                if(insertId>0)
                {
                    _medicoes.IdMedicaoIluminancia=insertId;
                    return true;
                }
                return false;
            }
            
            catch(Exception ex)
            {
                log.Error("Erro IluminanciaModel-InsertMedicaoIluminancia:" + ex.Message.ToString());
                return false;
            }
        }
    }
}