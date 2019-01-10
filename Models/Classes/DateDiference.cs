using System;
using System.Collections.Generic;
using Embraer_Backend.Models;
using Microsoft.Extensions.Configuration;

namespace Embraer_Backend.Models
{
    public class DateDiferenceModel
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(SecagensModel));
        
        public bool Minutos(IConfiguration _configuration,TimeSpan dateColeta)
        {
                 
            TimeSpan dateNow = DateTime.Now.TimeOfDay;                   
            TimeSpan difMin =(dateNow - dateColeta);
            long minutosDif = difMin.Minutes;
            long ultColetaMinutos = Convert.ToInt64(_configuration.GetSection("Settings:MinutosUltimaColeta").Value);
            log.Debug(ultColetaMinutos);
            if (minutosDif<ultColetaMinutos)            
                return true;
            else
                return false;
        }
        public bool Dias(IConfiguration _configuration,TimeSpan dateColeta)
        {   
            TimeSpan dateNow = DateTime.Now.TimeOfDay;                                       
            TimeSpan difMin =(dateNow - dateColeta);
            long diasDiff = difMin.Days;
            long ultColetaDias= Convert.ToInt64(_configuration.GetSection("Settings:DiasUltimoApontamento").Value);
            log.Debug(ultColetaDias);
            if (diasDiff<ultColetaDias)            
                return true;
            else
                return false;
        }
    }
}