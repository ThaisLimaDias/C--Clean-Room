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
    public class DashboardSLController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(DashboardSLController));
        private readonly IConfiguration _configuration;

        public DashboardSLController(IConfiguration configuration) 
        {            
            _configuration = configuration;
        }

        [HttpGet]      
        public IActionResult  GetTemperatura(long IdLocalColeta)
        {
            TemperaturaModel _model= new TemperaturaModel();
            List <Temperatura> _retorno = new List<Temperatura>();

            if (IdLocalColeta!=0)
            {                 
                log.Debug("Get Do Dashboard quadro Temperatura Sala Limpa !");            
                var list=_model.SelectTemperatura(_configuration, IdLocalColeta,null,null);
                if (list.Count()==0)
                    return StatusCode(505,"Não foi encontrado nenhuma coleta de temperatura");

                var valmax = list.Max( p=> p.Valor);
                var maxTemp = list.Where(p=>p.Valor==valmax).FirstOrDefault();
                _retorno.Add(maxTemp);

                var valmin = list.Min( p=> p.Valor);
                var minTemp = list.Where(p=>p.Valor==valmin).FirstOrDefault();
                _retorno.Add(minTemp);
                
                DateTime dateMin = maxTemp.DtColeta.Value;
                double difMin = (DateTime.Now.Subtract(dateMin)).TotalMinutes;
                double ultColetaMinutos = Convert.ToInt16(_configuration.GetSection("Settings:MinutosUltimaColeta").Value);

                DateTime dateMax = maxTemp.DtColeta.Value;
                double difMax = (DateTime.Now.Subtract(dateMin)).TotalMinutes;
                

                if (difMin<ultColetaMinutos ||difMax<ultColetaMinutos)
                    return Ok();
                else
                    return Ok(_retorno);
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
                log.Debug("Get Do Dashboard quadro umidade Sala Limpa !");            
                _retorno=_model.SelectUmidade(_configuration, IdLocalColeta).FirstOrDefault();
                if (_retorno!=null)
                {
                    DateTime date = _retorno.DtColeta.Value;
                    double difMin = (DateTime.Now.Subtract(date)).TotalMinutes;
                    double ultColetaMinutos = Convert.ToInt16(_configuration.GetSection("Settings:MinutosUltimaColeta").Value);

                    if (difMin<ultColetaMinutos)              
                        return Ok(_retorno);
                }
                    return Ok();
            }
            else
                return StatusCode(505,"Não foi recebido o parametro IdLocalColeta!");
        }

        [HttpGet]      
        public IActionResult  GetFrezzer(long IdLocalColeta)
        {
            TemperaturaModel _model= new TemperaturaModel();
            Temperatura _retorno = new Temperatura();

            if (IdLocalColeta!=0)
            {                 
                log.Debug("Get Do Dashboard quadro Freezer Sala Limpa !");            
                _retorno=_model.SelectTemperatura(_configuration, IdLocalColeta).FirstOrDefault();
                if (_retorno!=null)
                {
                    DateTime date = _retorno.DtColeta.Value;
                    double difMin = (DateTime.Now.Subtract(date)).TotalMinutes;
                    double ultColetaMinutos = Convert.ToInt16(_configuration.GetSection("Settings:MinutosUltimaColeta").Value);

                    if (difMin<ultColetaMinutos)
                        return Ok(_retorno);            
                }
                    return Ok();
            }
            else
                return StatusCode(505,"Não foi recebido o parametro IdLocalColeta!");
        }

        [HttpGet]      
        public IActionResult  GetLimpeza(long IdLocalColeta)
        {
            LimpezaModel _model= new LimpezaModel();
            List<Limpeza> _retorno = new List<Limpeza>();

            if (IdLocalColeta!=0)
            {                 
                log.Debug("Get Do Dashboard quadro Limpeza Sala Limpa !");            
                var list=_model.SelectLimpezaDashboard(_configuration,IdLocalColeta);
                if (list.Count()!=0)
                {
                    //Pegando o Ultima limpeza DIARIO
                    var vldiario = list.Where(p=>p.TipoControle=="DIARIO").Max(p=>p.DtMedicao); 
                    _retorno.Add(list.Where(p=> p.DtMedicao==vldiario.Value).FirstOrDefault());

                    //Pegando o Ultima limpeza SEMANAL
                    var vlsema = list.Where(p=>p.TipoControle=="SEMANAL").Max(p=>p.DtMedicao); 
                    _retorno.Add(list.Where(p=> p.DtMedicao==vlsema.Value).FirstOrDefault());

                    //Pegando o Ultima limpeza QUINZENAL
                    var vlQz = list.Where(p=>p.TipoControle=="QUINZENAL").Max(p=>p.DtMedicao);  
                    _retorno.Add(list.Where(p=> p.DtMedicao==vlQz.Value).FirstOrDefault());

                    //Pegando o Ultima limpeza MENSAL
                    var vlMen = list.Where(p=>p.TipoControle=="MENSAL").Max(p=>p.DtMedicao); 
                    _retorno.Add(list.Where(p=> p.DtMedicao==vlMen.Value).FirstOrDefault());

                    //Pegando o Ultima limpeza SEMESTRAL
                    var vlSem = list.Where(p=>p.TipoControle=="SEMESTRAL").Max(p=>p.DtMedicao);
                    _retorno.Add(list.Where(p=> p.DtMedicao==vlSem.Value).FirstOrDefault());
                }
                return Ok(_retorno);
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
                log.Debug("Get Do Dashboard quadro Iluminância Sala Limpa !");            
                var list=_model.SelectIluminancia(_configuration,IdLocalColeta);
                if (list.Count()!=0)
                {
                    _retorno = _model.SelectMedicaoIluminancia(_configuration,list.FirstOrDefault().IdApontIluminancia).ToList();
                    return Ok(_retorno);
                }
                else
                    return StatusCode(505,"Não foi encontrado nenhum apontamento para o IdLocalColeta!");
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
                log.Debug("Get Do Dashboard quadro Particulas Sala Limpa !");            
                var list=_model.SelectParticulas(_configuration,IdLocalColeta);
                if (list.Count()!=0)
                {
                    var listMed= _model.SelectMedicaoParticulasTam(_configuration,list.FirstOrDefault().IdApontParticulas.Value).ToList();
                    var vlMax1 =listMed.Where(p=>p.TamParticula==">0,5").Max(p=>p.ValorTamParticula);
                    var vlMax2 =listMed.Where(p=>p.TamParticula==">1").Max(p=>p.ValorTamParticula);
                    var vlMax3 =listMed.Where(p=>p.TamParticula==">5").Max(p=>p.ValorTamParticula);
                    var vlMax4 =listMed.Where(p=>p.TamParticula==">10").Max(p=>p.ValorTamParticula);
                    _particulas.Add(listMed.Where(p=>p.ValorTamParticula==vlMax1).FirstOrDefault());
                    _particulas.Add(listMed.Where(p=>p.ValorTamParticula==vlMax2).FirstOrDefault());
                    _particulas.Add(listMed.Where(p=>p.ValorTamParticula==vlMax3).FirstOrDefault());
                    _particulas.Add(listMed.Where(p=>p.ValorTamParticula==vlMax4).FirstOrDefault());
                    return Ok(_particulas);
                }
                else
                    return StatusCode(505,"Não foi encontrado nenhum apontamento para o IdLocalColeta!");
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
                log.Debug("Get Do Dashboard quadro Pressão Sala Limpa !");            
                _retorno=_model.SelectPressao(_configuration,IdLocalColeta).FirstOrDefault();
                if (_retorno!=null)
                {         
                    DateTime date = _retorno.DtColeta.Value;
                    double difMin = (DateTime.Now.Subtract(date)).TotalMinutes;
                    double ultColetaMinutos = Convert.ToInt16(_configuration.GetSection("Settings:MinutosUltimaColeta").Value);

                    if (difMin<ultColetaMinutos)              
                    return Ok(_retorno);
                }                                
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
                log.Debug("Get Do Dashboard quadro Portas Sala Limpa!"); 
                _sensores = _senModel.SelectSensor(_configuration,0,IdLocalColeta,"Porta");

                foreach(var item in _sensores)
                {
                    var porta = _model.SelectPorta(_configuration,IdLocalColeta,item.IdSensores).FirstOrDefault();
                    if (porta!=null)
                    { 
                        DateTime date = porta.DtColeta.Value;
                        double difMin = (DateTime.Now.Subtract(date)).TotalMinutes;
                        double ultColetaMinutos = Convert.ToInt16(_configuration.GetSection("Settings:MinutosUltimaColeta").Value);

                        if (difMin<ultColetaMinutos)   
                            _retorno.Add(porta);
                    }
                }                           
                return Ok(_retorno);
                
            }
            else
                return StatusCode(505,"Não foi recebido o parametro IdLocalColeta!");
        }

         [HttpGet]      
        public IActionResult  GetAlarmes(long IdLocalColeta)
        {
            AlarmesModel _model= new AlarmesModel();
            IEnumerable <Alarmes> _retorno;

            if (IdLocalColeta!=0)
            {                 
                log.Debug("Get Do Dashboard quadro Alarmes Sala Limpa !");            
                _retorno=_model.SelectAlarmesAbertos(_configuration,IdLocalColeta);
                if(_retorno!=null)
                    return Ok(_retorno);
                else
                    return Ok();
                
            }
            else
                return StatusCode(505,"Não foi recebido o parametro IdLocalColeta!");
        }


    }
}