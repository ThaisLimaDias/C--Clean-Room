using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Embraer_Backend.Models;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.AspNetCore.Cors;
using System.Linq;

namespace Embraer_Backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [EnableCors("CorsPolicy")]
    public class DashboardPinturaController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(DashboardPinturaController));
        private readonly IConfiguration _configuration;
        public  DateDiferenceModel _funcDate = new DateDiferenceModel();
        public DashboardPinturaController(IConfiguration configuration) 
        {            
            _configuration = configuration;
        }

        [HttpGet]      
        public IActionResult  GetTemperatura(long IdLocalColeta)
        {
            //Conversar com o Ale para fazer esse Endpoint
            TemperaturaModel _model= new TemperaturaModel();
            Temperatura _retorno;

            if (IdLocalColeta!=0)
            {                 
                log.Debug("Get Do Dashboard quadro Temperatura Sala Pintura Cabine Pintura de Id "+ IdLocalColeta);          
                _retorno = _model.SelectTemperatura(_configuration,IdLocalColeta).FirstOrDefault();
                if (_retorno!=null)
                {      
                    log.Debug("Retorno não nulo!" + _retorno.Valor);   
                    var difMinutes= _funcDate.Minutos(_configuration,_retorno.DtColeta.Value.TimeOfDay);
                                        

                    if (difMinutes)                    
                        log.Debug("retorna Json!" + _retorno.DtColeta.Value);  
                        return Json(_retorno);
                }    
                log.Debug("Retorno nulo!" );
                return Ok();
                
            }
            else
                return StatusCode(505,"Não foi recebido o parametro IdLocalColeta!");
        }    

        [HttpGet]      
        public IActionResult  GetUmidade(long IdLocalColeta)
        {
            UmidadeModel _model= new UmidadeModel();
            Umidade _retorno = new Umidade();

            if (IdLocalColeta!=0)
            {                 
                log.Debug("Get Do Dashboard quadro umidade Sala Pintura Cabine Pintura de Id "+ IdLocalColeta);            
                _retorno=_model.SelectUmidade(_configuration, IdLocalColeta).FirstOrDefault();
                if (_retorno!=null)
                { 
                    log.Debug("Retorno não nulo!" + _retorno.Valor);                   
                    var difMinutes= _funcDate.Minutos(_configuration,_retorno.DtColeta.Value.TimeOfDay);
                                        

                    if (difMinutes)
                        log.Debug("retorna Json!" + _retorno.DtColeta.Value);  
                        return Ok(_retorno);
                }
                return Ok();
            }
            else
                return StatusCode(505,"Não foi recebido o parametro IdLocalColeta!");
        }       

        [HttpGet]      
        public IActionResult  GetIluminancia(long IdLocalColeta)
        {
            IluminanciaModel _model= new IluminanciaModel();
            List<IluminanciaMedicoes> _retorno = new List<IluminanciaMedicoes>();

            if (IdLocalColeta!=0)
            {                 
                log.Debug("Get Do Dashboard quadro Iluminância Sala Pintura Cabine Pintura de Id "+ IdLocalColeta);    
                var list=_model.SelectIluminancia(_configuration,IdLocalColeta);
                if (list.Count()!=0)
                {
                    _retorno = _model.SelectMedicaoIluminancia(_configuration,list.FirstOrDefault().IdApontIluminancia).ToList();
                    return Ok(_retorno);
                }
                else
                    return Ok();
            }
            else
                return StatusCode(505,"Não foi recebido o parametro IdLocalColeta!");
        }

        [HttpGet]      
        public IActionResult  GetParticulas(long IdLocalColeta)
        {
            ParticulasModel _model= new ParticulasModel();
            List<ParticulasTam> _particulas = new List<ParticulasTam>();

            if (IdLocalColeta!=0)
            {                 
                log.Debug("Get Do Dashboard quadro Iluminância Sala Pintura Cabine Pintura de Id "+ IdLocalColeta);            
                var list=_model.SelectParticulas(_configuration,IdLocalColeta);
                if (list.Count()!=0)
                {
                    var listMed= _model.SelectMedicaoParticulasTam(_configuration,list.FirstOrDefault().IdApontParticulas.Value).ToList();
                    var vlMax1 =listMed.Where(p=>p.TamParticula==">1").Max(p=>p.ValorTamParticula);
                    var vlMax2 =listMed.Where(p=>p.TamParticula==">5").Max(p=>p.ValorTamParticula);
                    var vlMax3 =listMed.Where(p=>p.TamParticula==">10").Max(p=>p.ValorTamParticula);
                    var vlMax4 =listMed.Where(p=>p.TamParticula==">25").Max(p=>p.ValorTamParticula);
                    _particulas.Add(listMed.Where(p=>p.ValorTamParticula==vlMax1).FirstOrDefault());
                    _particulas.Add(listMed.Where(p=>p.ValorTamParticula==vlMax2).FirstOrDefault());
                    _particulas.Add(listMed.Where(p=>p.ValorTamParticula==vlMax3).FirstOrDefault());
                    _particulas.Add(listMed.Where(p=>p.ValorTamParticula==vlMax4).FirstOrDefault());
                    return Ok(_particulas);
                }
                else
                    return Ok();
            }
            else
                return StatusCode(505,"Não foi recebido o parametro IdLocalColeta!");
        }


        [HttpGet]      
        public IActionResult  GetPressao(long IdLocalColeta)
        {
            PressaoModel _model= new PressaoModel();
            Pressao _retorno = new Pressao();

            if (IdLocalColeta!=0)
            {                 
                log.Debug("Get Do Dashboard quadro Pressão Sala Pintura Cabine Pintura de Id "+ IdLocalColeta);            
                _retorno=_model.SelectPressao(_configuration,IdLocalColeta).FirstOrDefault();
                if (_retorno!=null)
                {
                    log.Debug("Retorno não nulo!" + _retorno.Valor);  
                    var difMinutes= _funcDate.Minutos(_configuration,_retorno.DtColeta.Value.TimeOfDay);                                        

                    if (difMinutes)
                        log.Debug("retorna Json!" + _retorno.DtColeta.Value);  
                        return Ok(_retorno);
                }
                return Ok();
                
            }
            else
                return StatusCode(505,"Não foi recebido o parametro IdLocalColeta!");
        }

        [HttpGet]      
        public IActionResult  GetPurezaAr(long IdLocalColeta)
        {
            ArComprimidoModel _model= new ArComprimidoModel();
            ArComprimido _retorno = new ArComprimido();

            if (IdLocalColeta!=0)
            {                 
                log.Debug("Get Do Dashboard quadro Pureza Ar Comprimido Sala Pintura Cabine Pintura de Id "+ IdLocalColeta);            
                _retorno=_model.SelectArComprimido(_configuration,IdLocalColeta).FirstOrDefault();
                if(_retorno!=null)
                {
                     log.Debug("Retorno não nulo!" + _retorno.Valor);
                    var difMinutes= _funcDate.Dias(_configuration,_retorno.DtMedicao.Value.TimeOfDay);
                                        

                    if (difMinutes)
                    {
                         log.Debug("Retorno Json!" + _retorno.Valor);
                        return Ok(_retorno);
                    }
                    return Ok();
                }
                else
                    return Ok();
            }
            else
                return StatusCode(505,"Não foi recebido o parametro IdLocalColeta!");
        }

        [HttpGet]      
        public IActionResult  GetPortas(long IdLocalColeta)
        {
            PortaModel _model= new PortaModel();
            SensoresModel _senModel= new SensoresModel();
            IEnumerable<Sensores> _sensores;
            List<Porta> _retorno = new List<Porta>();

            if (IdLocalColeta!=0)
            {                 
                log.Debug("Get Do Dashboard quadro Portas Sala Pintura!"); 
                _sensores = _senModel.SelectSensor(_configuration,0,IdLocalColeta,"Porta");

                foreach(var item in _sensores)
                {                   
                    var porta = _model.SelectPorta(_configuration,IdLocalColeta,item.IdSensores).FirstOrDefault();
                    log.Debug("Retorno não nulo!" + porta.Valor);
                    var difMinutes= _funcDate.Minutos(_configuration,porta.DtColeta.Value.TimeOfDay);                                        

                    if (difMinutes)
                        log.Debug("retorna Json!" + porta.DescPorta);  
                        _retorno.Add(porta);
                }

                return Ok(_retorno);
                
            }
            else
                return StatusCode(505,"Não foi recebido o parametro IdLocalColeta!");
        }

         [HttpGet]      
        public IActionResult  GetAlarmes(int cabines)
        {
            AlarmesModel _model= new AlarmesModel();
            IEnumerable <Alarmes> _retorno;                       
            log.Debug("Get Do Dashboard quadro Portas Sala Limpa !");            
            _retorno=_model.SelectAlarmesAbertos(_configuration,cabines);
            if(_retorno.Count()>0)
            {
                log.Debug("Retorno não nulo,tem alarmes!" + _retorno.FirstOrDefault().Mensagem);
                return Ok(_retorno);
            }
            else
                return Ok();
        }

    }
}