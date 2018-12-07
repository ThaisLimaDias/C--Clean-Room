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
    public class Equipamentos
    {
        public long IdEquip { get; set; }
        public string CodEquip { get; set; }
        public string  DescEquip {get;set;}
        public DateTime? DataUltCalibracao{get;set;}  
    }

   
    public class EquipamentosModel
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(EquipamentosModel));
        
        public IEnumerable<Equipamentos> SelectEquipamentos(IConfiguration _configuration)
        {            
            try
            {
                string sSql = string.Empty;

                sSql = "SELECT IdEquip,CodEquip,DescEquip,DataUltCalibracao";
                sSql = sSql + " FROM TB_EQUIPAMENTOS";
                sSql = sSql + " WHERE 1=1";
                                

                IEnumerable <Equipamentos> equipamentos;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                    equipamentos = db.Query<Equipamentos>(sSql,commandTimeout:0);
                }
                return  equipamentos;
            }
            catch (Exception ex)
            {
                log.Error("Erro EquipamentosModel-SelectEquipamentos:" + ex.Message.ToString());
                return null;
            }
        }

         public bool UpdateEquipamentos(IConfiguration _configuration,Equipamentos _equip)
        {
            try{
                string sSql = string.Empty;

                
                sSql = "UPDATE TB_EQUIPAMENTOS SET";    
                sSql=sSql+ " DataUltCalibracao='"+ _equip.DataUltCalibracao.Value.ToString("yyyy-MM-ddTHH:mm:ss") + "'";      
                sSql =sSql+ " WHERE IdEquip=" + _equip.IdEquip;

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
                log.Error("Erro EquipamentosModel-UpdateEquipamentos:" + ex.Message.ToString());
                return false;
            }
        }

        public bool InsertEquipamentos(IConfiguration _configuration,Equipamentos _equip)
        {
            
            string sSql = string.Empty;
            try
            {
             
                sSql= "INSERT INTO TB_EQUIPAMENTOS (CodEquip,DescEquip,DataUltCalibracao)";
                sSql = sSql + " VALUES ";
                sSql = sSql + "('" + _equip.CodEquip + "'";
                sSql = sSql + ",'" + _equip.DescEquip + "'";
                sSql = sSql + ", GETDATE())";
                sSql = sSql + " SELECT @@IDENTITY";

                long insertId=0;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DB_Embraer_Sala_Limpa")))
                {
                   insertId =db.QueryFirstOrDefault<long>(sSql,commandTimeout:0);
                }
                if(insertId>0)
                {
                    _equip.IdEquip=insertId;
                    return true;
                }
                return false;
            }
            
            catch(Exception ex)
            {
                log.Error("Erro EquipamentosModel-InsertEquipamentos:" + ex.Message.ToString());
                return false;
            }        
        }
    }
}