using System;
using System.Collections.Generic;
using Embraer_Backend.Models;
using Microsoft.Extensions.Configuration;

namespace Embraer_Backend.Models
{
    public class DateDiferenceModel
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(SecagensModel));
        
        public bool Minutos(IConfiguration _configuration,DateTime dateColeta)
        {
            // Transformando a data recebida para uma data compativel com o SQL
            System.Globalization.CultureInfo brCulture = new System.Globalization.CultureInfo("pt-br");
            log.Debug(brCulture.ThreeLetterISOLanguageName.ToString());
            DateTime coleta  = Convert.ToDateTime(dateColeta,brCulture);          
            DateTime agora = Convert.ToDateTime(DateTime.Now,brCulture); 
                 
            TimeSpan dateNow = DateTime.Now.TimeOfDay;                   
            TimeSpan difMin =(agora - coleta);
            long minutosDif = difMin.Minutes;
            long ultColetaMinutos = Convert.ToInt64(_configuration.GetSection("Settings:MinutosUltimaColeta").Value);
            log.Debug(ultColetaMinutos);
            if (minutosDif<ultColetaMinutos)            
            {
                log.Debug("true");       
                return true;
            }
            else
                log.Debug("false");  
                return false;
        }
        public bool Dias(IConfiguration _configuration,DateTime dateColeta)
        {   
            // Transformando a data recebida para uma data compativel com o SQL
            System.Globalization.CultureInfo brCulture = new System.Globalization.CultureInfo("pt-br");
            log.Debug(brCulture.ThreeLetterISOLanguageName.ToString());
            DateTime coleta  = Convert.ToDateTime(dateColeta,brCulture);          
            DateTime agora = Convert.ToDateTime(DateTime.Now,brCulture); 

            TimeSpan dateNow = DateTime.Now.TimeOfDay;                                       
            TimeSpan difMin =(agora - coleta);
            long diasDiff = difMin.Days;
            long ultColetaDias= Convert.ToInt64(_configuration.GetSection("Settings:DiasUltimoApontamento").Value);
            log.Debug(ultColetaDias);
            if (diasDiff<ultColetaDias)     
            {
                log.Debug("true");       
                return true;
            }
            else
                log.Debug("false"); 
                return false;
        }
    }
}