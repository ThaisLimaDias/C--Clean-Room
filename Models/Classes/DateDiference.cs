using System;
using System.Collections.Generic;
using ProjectCleanning_Backend.Models;
using Microsoft.Extensions.Configuration;

namespace ProjectCleanning_Backend.Models
{
    public class DateDiferenceModel
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(SecagensModel));
        
        public bool Seconds(IConfiguration _configuration,DateTime dateColeta)
        {
            // Transformando a data recebida para uma data compativel com o SQL
            System.Globalization.CultureInfo brCulture = new System.Globalization.CultureInfo("pt-br");
            log.Debug(brCulture.ThreeLetterISOLanguageName.ToString());
            DateTime coleta  = Convert.ToDateTime(dateColeta,brCulture);          
            DateTime agora = Convert.ToDateTime(DateTime.Now,brCulture);    
                      
            TimeSpan difSec =agora.Subtract(coleta);
            long ultColetaSec = 30;
            if (difSec.Seconds<ultColetaSec  && difSec.Minutes==0 && difSec.Hours==0)            
            {
                log.Debug("true");       
                return true;
            }
            else
                log.Debug("false");  
                return false;
        }
        public bool Dias(IConfiguration _configuration,DateTime dateColeta,int Dias)
        {   
            // Transformando a data recebida para uma data compativel com o SQL
            System.Globalization.CultureInfo brCulture = new System.Globalization.CultureInfo("pt-br");
            log.Debug(brCulture.ThreeLetterISOLanguageName.ToString());
            DateTime coleta  = Convert.ToDateTime(dateColeta,brCulture);          
            DateTime agora = Convert.ToDateTime(DateTime.Now,brCulture); 

            TimeSpan dateNow = DateTime.Now.TimeOfDay;                                       
            TimeSpan difMin =(agora - coleta);
            long diasDiff = difMin.Days;  
            if (diasDiff<Dias)     
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