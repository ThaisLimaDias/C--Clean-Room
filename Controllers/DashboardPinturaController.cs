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
        AlarmesModel _alm = new AlarmesModel();
        ControleApontamentoModel _ctrlApt = new ControleApontamentoModel();
        StatusSensorModel _status = new StatusSensorModel();
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
                    _retorno.Valor=Convert.ToDecimal(_retorno.Valor.ToString().Substring(0,4));
                    log.Debug("Retorno não nulo!" + _retorno.Valor);  
                    StatusSensor dataUltimaColetaSensor =  _status.SelectStatus(_configuration,_retorno.IdSensores).FirstOrDefault();
                    var dif= _funcDate.Seconds(_configuration,dataUltimaColetaSensor.Dt_Status.Value);                                        
                  
                    //Definindo a cor de fundo do quadrante de temperatura
                    var alarmes = _alm.SelectAlarmesAbertos(_configuration,IdLocalColeta, "Temperatura",0);
                    if (alarmes.Count()>0)                
                        _retorno.CorDashboard = true;
                    else              
                        _retorno.CorDashboard = false;
                    /////////////  

                    if (dif==true)  
                    {                  
                        log.Debug("retorna Json!" + _retorno.DtColeta.Value);  
                        return Json(_retorno);
                    }
                }    
                log.Debug("Retorno nulo!" );
                return StatusCode(204);
                
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
                    _retorno.Valor=Convert.ToDecimal(_retorno.Valor.ToString().Substring(0,2));
                    log.Debug("Retorno não nulo!" + _retorno.Valor);   
                    StatusSensor dataUltimaColetaSensor =  _status.SelectStatus(_configuration,_retorno.IdSensores).FirstOrDefault();
                    var dif= _funcDate.Seconds(_configuration,dataUltimaColetaSensor.Dt_Status.Value); 
                    //Definindo a cor de fundo do quadrante de temperatura
                    var alarmes = _alm.SelectAlarmesAbertos(_configuration,IdLocalColeta, "Umidade",0);
                    if (alarmes.Count()>0)                
                        _retorno.CorDashboard = true;
                    else              
                        _retorno.CorDashboard = false;
                    /////////////                

                    if (dif==true)  
                    {
                        log.Debug("retorna Json!" + _retorno.DtColeta.Value);  
                        return Ok(_retorno);
                    }
                }
                return StatusCode(204);
            }
            else
                return StatusCode(505,"Não foi recebido o parametro IdLocalColeta!");
        }       

        [HttpGet]      
        public IActionResult  GetIluminancia(long IdLocalColeta)
        {
            IluminanciaModel _model= new IluminanciaModel();
            Iluminancia _retorno = new Iluminancia();

            if (IdLocalColeta!=0)
            {                 
                log.Debug("Get Do Dashboard quadro Iluminância Sala de Pintura !");            
                var list=_model.SelectIluminancia(_configuration,IdLocalColeta);
                if (list.Count()!=0)
                {
                    _retorno.medicoes=_model.SelectMedicaoIluminancia(_configuration,list.FirstOrDefault().IdApontIluminancia);
                    
                    foreach(var item in _retorno.medicoes)
                    {
                        if (item.Valor<item.EspecificacaoMin || item.Valor>item.EspecificacaoMax)
                            _retorno.CorDashboard =true;                        
                    }

                    var proxApt = _ctrlApt.SelectControleApontamento(_configuration,"Iluminância");
                    TimeSpan date =  Convert.ToDateTime(DateTime.Now) - Convert.ToDateTime(list.FirstOrDefault().DtMedicao.Value);
                    int totalDias = date.Days;
                    if(totalDias>proxApt.FirstOrDefault().DiasProximaMed)
                        _retorno.CorDashboard = true;

                    return Ok(_retorno);
                }
                else
                    return StatusCode(204);
            }
            else
                return StatusCode(505,"Não foi recebido o parametro IdLocalColeta!");
        }

        [HttpGet]      
        public IActionResult  GetParticulas(long IdLocalColeta)
        {
             ParticulasModel _model= new ParticulasModel();
            Particulas _particulas = new Particulas();
            List<ParticulasTam> _particulasTam = new List<ParticulasTam>();

            if (IdLocalColeta!=0)
            {                 
                log.Debug("Get Do Dashboard quadro Particulas Sala Limpa !");            
                _particulas=_model.SelectParticulas(_configuration,IdLocalColeta).FirstOrDefault();
                if (_particulas!=null)
                {
                    var listMed= _model.SelectMedicaoParticulasTam(_configuration,_particulas.IdApontParticulas.Value).ToList();                    
                    var vlMax1 =listMed.Where(p=>p.TamParticula==">1").Max(p=>p.ValorTamParticula);
                    var vlMax2 =listMed.Where(p=>p.TamParticula==">5").Max(p=>p.ValorTamParticula);
                    var vlMax3 =listMed.Where(p=>p.TamParticula==">10").Max(p=>p.ValorTamParticula);
                    var vlMax4 =listMed.Where(p=>p.TamParticula==">25").Max(p=>p.ValorTamParticula);
                    _particulasTam.Add(listMed.Where(p=>p.ValorTamParticula==vlMax1 && p.TamParticula==">1").FirstOrDefault());
                    _particulasTam.Add(listMed.Where(p=>p.ValorTamParticula==vlMax2 && p.TamParticula==">5").FirstOrDefault());
                    _particulasTam.Add(listMed.Where(p=>p.ValorTamParticula==vlMax3 && p.TamParticula==">10").FirstOrDefault());
                    _particulasTam.Add(listMed.Where(p=>p.ValorTamParticula==vlMax4 && p.TamParticula==">25").FirstOrDefault());          

                                        
                    foreach(var item in _particulasTam)
                    {
                        if (item.ValorTamParticula<item.EspecificacaoMin || item.ValorTamParticula>item.EspecificacaoMax)
                            item.CorDashboard =true;                        
                    }

                    var proxApt = _ctrlApt.SelectControleApontamento(_configuration,"Particulas");
                    TimeSpan date =  Convert.ToDateTime(DateTime.Now) - Convert.ToDateTime(_particulas.DtMedicao.Value);
                    int totalDias = date.Days;
                    if(totalDias>proxApt.FirstOrDefault().DiasProximaMed)
                        _particulasTam.FirstOrDefault().CorDashboard = true;

                    return Ok(_particulasTam);                
                }
                else
                    return StatusCode(204);
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
                    _retorno.Valor=Convert.ToDecimal(_retorno.Valor.ToString().Substring(0,4));  
                    StatusSensor dataUltimaColetaSensor =  _status.SelectStatus(_configuration,_retorno.IdSensores).FirstOrDefault();
                    var dif= _funcDate.Seconds(_configuration,dataUltimaColetaSensor.Dt_Status.Value);                                        

                    //Definindo a cor de fundo do quadrante de temperatura
                    var alarmes = _alm.SelectAlarmesAbertos(_configuration,IdLocalColeta, "Pressão",0);
                    if (alarmes.Count()>0)                
                        _retorno.CorDashboard = true;
                    else              
                        _retorno.CorDashboard = false;
                    /////////////


                    if (dif==true)  
                    {
                        log.Debug("retorna Json!" + _retorno.DtColeta.Value);  
                        return Ok(_retorno);
                    }
                }
                return StatusCode(204);
                
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
                    var ParametroApont=_ctrlApt.SelectControleApontamento(_configuration,"Pureza Ar Comprimido").FirstOrDefault();
                    var difMinutes= _funcDate.Dias(_configuration,_retorno.DtMedicao.Value,ParametroApont.DiasProximaMed);                                        

                    log.Debug("difMinutes" + difMinutes);
                    if (difMinutes)
                    {
                        log.Debug("Retorno Json!" + _retorno.Valor);
                        return Ok(_retorno);
                    }
                }                
                return StatusCode(204);
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
                    StatusSensor dataUltimaColetaSensor =  _status.SelectStatus(_configuration,item.IdSensores).FirstOrDefault();
                    var dif= _funcDate.Seconds(_configuration,dataUltimaColetaSensor.Dt_Status.Value);                                        
                    System.Globalization.CultureInfo brCulture = new System.Globalization.CultureInfo("pt-br");
                    DateTime coleta  = Convert.ToDateTime(porta.DtColeta,brCulture);          
                    DateTime agora = Convert.ToDateTime(DateTime.Now,brCulture);  
                    TimeSpan diff =agora.Subtract(coleta);

                    porta.horaTv = Convert.ToInt32(diff.TotalHours).ToString() +"h"+(diff.Minutes.ToString().Length<2 
                    ?"0"+diff.Minutes.ToString() : diff.Minutes.ToString()) +"m";
                    

                    //Definindo a cor de fundo do quadrante de porta
                        var alarmes = _alm.SelectAlarmesAbertos(_configuration,0, "Porta",item.IdSensores);
                        if (alarmes.Count()>0)                
                            porta.CorDashboard = true;
                        else              
                            porta.CorDashboard = false;
                    /////////////
                    if (dif==true)  
                        log.Debug("retorna Json!" + porta.DescPorta);  
                        _retorno.Add(porta);
                }

                if (_retorno.Count()==0)
                    return StatusCode(204);                    
                else
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
            _retorno=_model.SelectAlarmesAbertos(_configuration,cabines,null,0);
            if(_retorno.Count()>0)
            {
                log.Debug("Retorno não nulo,tem alarmes!" + _retorno.FirstOrDefault().Mensagem);
                return Ok(_retorno);
            }
            else
                return StatusCode(204);
        }

    }
}