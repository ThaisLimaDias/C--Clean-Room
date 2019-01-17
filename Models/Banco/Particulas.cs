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
    public class Particulas
    {
        public long? IdApontParticulas { get; set; }
        public long IdUsuario { get; set; }
        public long  IdLocalMedicao {get;set;}
        public string CodUsuario { get; set; }
        public string DescLocalMedicao { get; set; }
        public string Mes {get;set;}
        public int Ano {get;set;}
        public DateTime? DtMedicao{get;set;}
        public DateTime? DtOcorrencia{get;set;}
        public string FatoOcorrencia{get;set;}
        public string AcoesObservacoes{get;set;}
    }
   public class ParticulasMedicoes
    {
        public long? IdMedicaoParticulas {get;set;}
        public long IdApontParticulas {get;set;}
        public string DescPonto {get;set;}

    }

    public class ParticulasTam
    {
        public long? IdTamParticulas {get;set;}
        public long IdMedicaoParticulas {get;set;}
        public string CodUsuario { get; set; }
        public string DescLocalMedicao { get; set; }
        public string TamParticula{get;set;}
        public decimal ValorTamParticula {get;set;}
        public long IdCadParametroSistema {get;set;}
        public decimal ControleMin {get;set;}
        public decimal EspecificacaoMin {get;set;}
        public decimal EspecificacaoMax {get;set;}
        public decimal ControleMax {get;set;}
    }
    public class ParticulasModel
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ParticulasModel));
        
        public IEnumerable<Particulas> SelectParticulas(IConfiguration _configuration, long id, string dtIni, string dtFim,bool?  Ocorrencia)
        {            
            try
            {
                string sSql = string.Empty;

                sSql = "SELECT IdApontParticulas,CodUsuario,A.IdUsuario,DescLocalMedicao,A.IdLocalMedicao,Mes,Ano,DtMedicao,DtOcorrencia,FatoOcorrencia,AcoesObservacoes";
                sSql += " FROM TB_APONT_PARTICULAS A";
                sSql +=  " INNER JOIN TB_USUARIO U ON A.IdUsuario = U.IdUsuario";
                sSql +=  " INNER JOIN TB_LOCAL_MEDICAO L ON A.IdLocalMedicao = L.IdLocalMedicao";
                sSql +=  " WHERE 1=1";
                
                if (id!=0)
                    sSql = sSql + " AND IdApontParticulas=" + id;

                if(dtIni !=null && dtFim!=null)
                    sSql = sSql + " AND DtMedicao BETWEEN " + dtIni + " AND " + dtFim + "";

                if (Ocorrencia.Value)
                    sSql=sSql + " AND DtOcorrencia IS NULL";
                    
                IEnumerable <Particulas> particulas;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                    particulas = db.Query<Particulas>(sSql,commandTimeout:0);
                }                 
                return particulas;
            }
            catch (Exception ex)
            {
                log.Error("Erro ParticulasModel-SelectParticulas:" + ex.Message.ToString());
                return null;
            }
        }
        public IEnumerable<Particulas> SelectParticulas(IConfiguration _configuration, long IdLocalMedicao)
        {            
            try
            {
                string sSql = string.Empty;

                sSql = "SELECT TOP 1 IdApontParticulas,CodUsuario,A.IdUsuario,DescLocalMedicao,A.IdLocalMedicao,Mes,Ano,DtMedicao,DtOcorrencia,FatoOcorrencia,AcoesObservacoes";
                sSql += " FROM TB_APONT_PARTICULAS A";
                sSql +=  " INNER JOIN TB_USUARIO U ON A.IdUsuario = U.IdUsuario";
                sSql +=  " INNER JOIN TB_LOCAL_MEDICAO L ON A.IdLocalMedicao = L.IdLocalMedicao";
                sSql += " WHERE A.IdLocalMedicao=" + IdLocalMedicao;
                sSql += " ORDER BY DtMedicao DESC";
                
                IEnumerable <Particulas> particulas;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                    particulas = db.Query<Particulas>(sSql,commandTimeout:0);
                }                 
                return particulas;
            }
            catch (Exception ex)
            {
                log.Error("Erro ParticulasModel-SelectIParticulas:" + ex.Message.ToString());
                return null;
            }
        }
        public bool InsertIParticulas(IConfiguration _configuration,Particulas _particulas)
        {
            
            string sSql = string.Empty;
            try
            {
                string dataOcorrencia = (_particulas.DtOcorrencia==null) ? "NULL" : _particulas.DtOcorrencia.Value.ToString("yyyy-MM-ddTHH:mm:ss");
                
                sSql= "INSERT INTO TB_APONT_PARTICULAS (IdUsuario,IdLocalMedicao,Mes,Ano,DtMedicao,DtOcorrencia,FatoOcorrencia,AcoesObservacoes)";
                sSql = sSql + " VALUES ";
                sSql = sSql + "(" + _particulas.IdUsuario;
                sSql = sSql + "," + _particulas.IdLocalMedicao;
                sSql = sSql + ",'" + _particulas.Mes + "'";
                sSql = sSql + "," + _particulas.Ano;
                sSql = sSql + ", GETDATE()";
                sSql = sSql + ((dataOcorrencia=="NULL")? ","+  dataOcorrencia + "" : ",'"+  dataOcorrencia + "'");
                sSql = sSql +  "," + ((_particulas.FatoOcorrencia==null) ? "NULL" : ( "'" + _particulas.FatoOcorrencia +"'"));
                sSql = sSql +  "," + ((_particulas.AcoesObservacoes==null) ? "NULL" : ( "'" + _particulas.AcoesObservacoes.ToString() + "'") ) + ")";
                sSql = sSql + " SELECT @@IDENTITY";

                long insertId=0;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                   insertId =db.QueryFirstOrDefault<long>(sSql,commandTimeout:0);
                }
                if(insertId>0)
                {
                    _particulas.IdApontParticulas=insertId;
                    return true;
                }
                return false;
            }
            
            catch(Exception ex)
            {
                log.Error("Erro ParticulasModel-InsertIParticulas:" + ex.Message.ToString());
                return false;
            }
        }
        public bool UpdateParticulas (IConfiguration _configuration,Particulas _particulas)
        {
            try{
                string sSql = string.Empty;

                string dataOcorrencia = (_particulas.DtOcorrencia==null) ? "NULL" : _particulas.DtOcorrencia.Value.ToString("yyyy-MM-ddTHH:mm:ss");

                sSql = "UPDATE TB_APONT_PARTICULAS SET";                
                sSql=sSql+ ((dataOcorrencia=="NULL")? "[DtOcorrencia]="+  dataOcorrencia + "" : "[DtOcorrencia]='"+  dataOcorrencia + "'");
                sSql=sSql+ ",[FatoOcorrencia]='"+ _particulas.FatoOcorrencia + "'";
                sSql=sSql+ ",[AcoesObservacoes]='"+ _particulas.AcoesObservacoes + "'";
                sSql =sSql+ " WHERE IdApontParticulas=" + _particulas.IdApontParticulas;

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
                log.Error("Erro ParticulasModel-UpdateParticulas:" + ex.Message.ToString());
                return false;
            }
        }
        public IEnumerable<ParticulasMedicoes> SelectMedicaoParticulas(IConfiguration _configuration, long idApont)
        {
            try
            {
                string sSql = string.Empty;

                sSql = "SELECT IdMedicaoParticulas,IdApontParticulas,DescPonto";
                sSql = sSql + " FROM TB_MEDICAO_PARTICULAS";
                sSql = sSql + " WHERE IdApontParticulas=" + idApont;  

                IEnumerable <ParticulasMedicoes> medicoes;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                    medicoes = db.Query<ParticulasMedicoes>(sSql,commandTimeout:0);
                }                   
                return medicoes;
            }
            catch (Exception ex)
            {
                log.Error("Erro ParticulasModel-SelectMedicaoParticulas:" + ex.Message.ToString());
                return null;
            }

        }       
        
        public bool InsertMedicaoParticulas(IConfiguration _configuration,ParticulasMedicoes _medicoes)
        {            
            string sSql = string.Empty;
            try
            {
                
                sSql= "INSERT INTO TB_MEDICAO_PARTICULAS (IdApontParticulas,DescPonto)";
                sSql = sSql + " VALUES ";
                sSql = sSql + "(" + _medicoes.IdApontParticulas;
                sSql = sSql + ",'" + _medicoes.DescPonto + "')";
                sSql = sSql + " SELECT @@IDENTITY";

                long insertId=0;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                   insertId =db.QueryFirstOrDefault<long>(sSql,commandTimeout:0);
                }
                if(insertId>0)
                {
                    _medicoes.IdMedicaoParticulas=insertId;
                    return true;
                }
                return false;
            }
            
            catch(Exception ex)
            {
                log.Error("Erro ParticulasModel-InsertMedicaoParticulas:" + ex.Message.ToString());
                return false;
            }        
        }

        public IEnumerable<ParticulasTam> SelectParticulasTam(IConfiguration _configuration, long IdMedicaoParticulas)
        {
            try
            {
                string sSql = string.Empty;

                sSql = "SELECT IdTamParticulas,IdMedicaoParticulas,TamParticula,ValorTamParticula";
                sSql = sSql + ",IdCadParametroSistema,ControleMin,EspecificacaoMin,EspecificacaoMax,ControleMax";
                sSql = sSql + " FROM TB_MEDICAO_PARTICULAS_TAM_PARTICULAS";
                sSql = sSql + " WHERE IdMedicaoParticulas=" + IdMedicaoParticulas;  

                IEnumerable <ParticulasTam> medicoes;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                    medicoes = db.Query<ParticulasTam>(sSql,commandTimeout:0);
                }                   
                return medicoes;
            }
            catch (Exception ex)
            {
                log.Error("Erro ParticulasModel-SelectParticulasTam:" + ex.Message.ToString());
                return null;
            }

        }   

        public IEnumerable<ParticulasTam> SelectMedicaoParticulasTam(IConfiguration _configuration, long IdApontParticulas)
        {
            try
            {
                string sSql = string.Empty;

                sSql = "SELECT IdTamParticulas,T.IdMedicaoParticulas,TamParticula,ValorTamParticula";
                sSql = sSql + ",IdCadParametroSistema,ControleMin,EspecificacaoMin,EspecificacaoMax,ControleMax";
                sSql = sSql + " FROM TB_MEDICAO_PARTICULAS_TAM_PARTICULAS T";
                sSql = sSql + " INNER JOIN TB_MEDICAO_PARTICULAS M ON M.IdMedicaoParticulas =  T.IdMedicaoParticulas";
                sSql = sSql + " WHERE M.IdApontParticulas=" + IdApontParticulas;  

                IEnumerable <ParticulasTam> medicoes;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                    medicoes = db.Query<ParticulasTam>(sSql,commandTimeout:0);
                }                   
                return medicoes;
            }
            catch (Exception ex)
            {
                log.Error("Erro ParticulasModel-SelectParticulasTam:" + ex.Message.ToString());
                return null;
            }

        }           
        
        public bool InsertParticulasTam(IConfiguration _configuration,ParticulasTam _medicoes)
        {            
            string sSql = string.Empty;
            try
            {
                
                sSql= "INSERT INTO TB_MEDICAO_PARTICULAS_TAM_PARTICULAS (IdMedicaoParticulas,TamParticula,ValorTamParticula";
                sSql = sSql + ",IdCadParametroSistema,ControleMin,EspecificacaoMin,EspecificacaoMax,ControleMax)";
                sSql = sSql + " VALUES ";
                sSql = sSql + "(" + _medicoes.IdMedicaoParticulas;
                sSql = sSql + ",'" + _medicoes.TamParticula + "'";
                sSql = sSql + ",'" + _medicoes.ValorTamParticula.ToString().Replace(",",".") + "'";
                sSql = sSql + "," + _medicoes.IdCadParametroSistema;
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
                    _medicoes.IdMedicaoParticulas=insertId;
                    return true;
                }
                return false;
            }
            
            catch(Exception ex)
            {
                log.Error("Erro ParticulasModel-InsertParticulasTam:" + ex.Message.ToString());
                return false;
            }        
        }

    }
}