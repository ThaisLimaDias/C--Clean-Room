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

        AlarmesModel _alm = new AlarmesModel();
        ControleApontamentoModel _ctrlApt = new ControleApontamentoModel();

        public  DateDiferenceModel _funcDate = new DateDiferenceModel();
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
                maxTemp.Valor=Convert.ToDecimal(maxTemp.Valor.ToString().Substring(0,4));
                _retorno.Add(maxTemp);

                var valmin = list.Min( p=> p.Valor);
                var minTemp = list.Where(p=>p.Valor==valmin).FirstOrDefault();
                minTemp.Valor=Convert.ToDecimal(minTemp.Valor.ToString().Substring(0,4));
                _retorno.Add(minTemp);
                
                var difMinMinutes= _funcDate.Minutos(_configuration,maxTemp.DtColeta.Value);
                var difMaxMinutes= _funcDate.Minutos(_configuration,minTemp.DtColeta.Value); 

                //Definindo a cor de fundo do quadrante de temperatura
                var alarmes = _alm.SelectAlarmesAbertos(_configuration,IdLocalColeta, "Temperatura",0);
                if (alarmes.Count()>0)                
                    _retorno.FirstOrDefault().CorDashboard = true;
                else              
                    _retorno.FirstOrDefault().CorDashboard = false;
                    /////////////     


                if (difMinMinutes==false || difMaxMinutes==false)
                    return StatusCode(204);
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
                    _retorno.Valor=Convert.ToDecimal(_retorno.Valor.ToString().Substring(0,2));
                    var difMinMinutes= _funcDate.Minutos(_configuration,_retorno.DtColeta.Value);                                

                    //Definindo a cor de fundo do quadrante de temperatura
                    var alarmes = _alm.SelectAlarmesAbertos(_configuration,IdLocalColeta, "Umidade",0);
                    if (alarmes.Count()>0)                
                        _retorno.CorDashboard = true;
                    else              
                        _retorno.CorDashboard = false;
                        /////////////     

                    if (difMinMinutes)
                        return Ok(_retorno);
                        
                }
                    return StatusCode(204);
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
                    _retorno.Valor=Convert.ToDecimal(_retorno.Valor.ToString().Substring(0,4));
                    var difMinMinutes= _funcDate.Minutos(_configuration,_retorno.DtColeta.Value); 

                    //Definindo a cor de fundo do quadrante de temperatura
                    var alarmes = _alm.SelectAlarmesAbertos(_configuration,IdLocalColeta, "Temperatura",0);
                    if (alarmes.Count()>0)                
                        _retorno.CorDashboard = true;
                    else              
                        _retorno.CorDashboard = false;
                        /////////////  


                    if (difMinMinutes)
                        return Ok(_retorno);          
                }
                return StatusCode(204);
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
                    var vldiario = list.Where(p=>p.TipoControle=="DIARIO" && p.Status=="Finalizado").Max(p=>p.DtMedicao); 
                    var dtAptDiario = _ctrlApt.SelectControleApontamento(_configuration, "Limpeza - Diário").FirstOrDefault();
                    if(vldiario!=null)
                    {
                        var dia = list.Where(p=> p.DtMedicao==vldiario.Value).FirstOrDefault();
                        //Verifica se o APontamento está vencido
                        TimeSpan date =  Convert.ToDateTime(dtAptDiario.ProxApont.Value) - Convert.ToDateTime(dia.DtMedicao.Value);
                        int totalDias = date.Days;
                        if(totalDias>dtAptDiario.DiasProximaMed)
                            dia.CorDashboard=true;
                        
                        _retorno.Add(dia);   
                    }        


                    //Pegando o Ultima limpeza SEMANAL
                    var vlsema = list.Where(p=>p.TipoControle=="SEMANAL" && p.Status=="Finalizado").Max(p=>p.DtMedicao);
                    var dtAptSemanal = _ctrlApt.SelectControleApontamento(_configuration, "Limpeza - Semanal").FirstOrDefault(); 
                    if(vlsema!=null)
                    {
                        var semana= list.Where(p=> p.DtMedicao==vlsema.Value).FirstOrDefault();
                        //Verifica se o APontamento está vencido
                        TimeSpan date =  Convert.ToDateTime(dtAptSemanal.ProxApont.Value) - Convert.ToDateTime(semana.DtMedicao.Value);
                        int totalDias = date.Days;
                        if(totalDias>dtAptSemanal.DiasProximaMed)
                            semana.CorDashboard=true;
                        
                        _retorno.Add(semana);
                    }

                    
                    //Pegando o Ultima limpeza QUINZENAL
                    var vlQz = list.Where(p=>p.TipoControle=="QUINZENAL" && p.Status=="Finalizado").Max(p=>p.DtMedicao); 
                    var dtAptQuinzenal = _ctrlApt.SelectControleApontamento(_configuration, "Limpeza - Quinzenal").FirstOrDefault(); 
                    if(vlQz!=null)
                    { 
                        var quinzena = list.Where(p=> p.DtMedicao==vlQz.Value).FirstOrDefault();
                        //Verifica se o APontamento está vencido
                        TimeSpan date =  Convert.ToDateTime(dtAptQuinzenal.ProxApont.Value) - Convert.ToDateTime(quinzena.DtMedicao.Value);
                        int totalDias = date.Days;
                        if(totalDias>dtAptQuinzenal.DiasProximaMed)
                            quinzena.CorDashboard=true;
                                            
                        _retorno.Add(quinzena);
                    }

                    //Pegando o Ultima limpeza MENSAL
                    var vlMen = list.Where(p=>p.TipoControle=="MENSAL" && p.Status=="Finalizado").Max(p=>p.DtMedicao); 
                    var dtAptMensal = _ctrlApt.SelectControleApontamento(_configuration, "Limpeza - Mensal").FirstOrDefault(); 
                    if(vlMen!=null)
                    { 
                        var mes= list.Where(p=> p.DtMedicao==vlMen.Value).FirstOrDefault();                    
                        //Verifica se o APontamento está vencido
                        TimeSpan date =  Convert.ToDateTime(dtAptMensal.ProxApont.Value) - Convert.ToDateTime(mes.DtMedicao.Value);
                        int totalDias = date.Days;
                        if(totalDias>dtAptMensal.DiasProximaMed)
                            mes.CorDashboard=true;
                            
                        _retorno.Add(mes);
                        
                    }


                    //Pegando o Ultima limpeza SEMESTRAL
                    var vlSem = list.Where(p=>p.TipoControle=="SEMESTRAL" && p.Status=="Finalizado").Max(p=>p.DtMedicao);
                    var dtAptSemestral = _ctrlApt.SelectControleApontamento(_configuration, "Limpeza - Semestral").FirstOrDefault();
                    if(vlSem!=null)
                    { 
                        var semestre =list.Where(p=> p.DtMedicao==vlSem.Value).FirstOrDefault();                     
                        //Verifica se o APontamento está vencido

                        TimeSpan date =  Convert.ToDateTime(dtAptSemestral.ProxApont.Value) - Convert.ToDateTime(semestre.DtMedicao.Value);
                        int totalDias = date.Days;
                        if(totalDias>dtAptSemestral.DiasProximaMed)
                            semestre.CorDashboard=true;

                        _retorno.Add(semestre);
                    }

                    return Ok(_retorno);
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
                log.Debug("Get Do Dashboard quadro Iluminância Sala Limpa !");            
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
                    var vlMax1 =listMed.Where(p=>p.TamParticula==">0,5").Max(p=>p.ValorTamParticula);
                    var vlMax2 =listMed.Where(p=>p.TamParticula==">1").Max(p=>p.ValorTamParticula);
                    var vlMax3 =listMed.Where(p=>p.TamParticula==">5").Max(p=>p.ValorTamParticula);
                    var vlMax4 =listMed.Where(p=>p.TamParticula==">10").Max(p=>p.ValorTamParticula);
                    _particulasTam.Add(listMed.Where(p=>p.ValorTamParticula==vlMax1 && p.TamParticula==">0,5").FirstOrDefault());
                    _particulasTam.Add(listMed.Where(p=>p.ValorTamParticula==vlMax2 && p.TamParticula==">1").FirstOrDefault());
                    _particulasTam.Add(listMed.Where(p=>p.ValorTamParticula==vlMax3 && p.TamParticula==">5").FirstOrDefault());
                    _particulasTam.Add(listMed.Where(p=>p.ValorTamParticula==vlMax4 && p.TamParticula==">10").FirstOrDefault());          

                                        
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
                log.Debug("Get Do Dashboard quadro Pressão Sala Limpa !");            
                _retorno=_model.SelectPressao(_configuration,IdLocalColeta).FirstOrDefault();
                if (_retorno!=null)
                {         
                    _retorno.Valor=Convert.ToDecimal(_retorno.Valor.ToString().Substring(0,4));
                    var difMinMinutes= _funcDate.Minutos(_configuration,_retorno.DtColeta.Value);                                

                    //Definindo a cor de fundo do quadrante de pressão
                    var alarmes = _alm.SelectAlarmesAbertos(_configuration,IdLocalColeta, "Pressão",0);
                    if (alarmes.Count()>0)                
                        _retorno.CorDashboard = true;
                    else              
                        _retorno.CorDashboard = false;
                        /////////////  

                    if (difMinMinutes)        
                        return Ok(_retorno);
                    
                }                                
                    return StatusCode(204);                                
            }
            else
                return StatusCode(505,"Não foi recebido o parametro IdLocalColeta!");
        }

        [HttpGet]      
        public IActionResult  GetPortas()
        {
            PortaModel _model= new PortaModel();
            SensoresModel _senModel= new SensoresModel();
            IEnumerable<Sensores> _sensores;
            List<Porta> _retorno = new List<Porta>();
              
            log.Debug("Get Do Dashboard quadro Portas Sala Limpa!"); 
            _sensores = _senModel.SelectSensor(_configuration,0,0,"Porta");
            foreach(var item in _sensores)
            {
                var porta = _model.SelectPorta(_configuration,0,item.IdSensores).FirstOrDefault();
                if (porta!=null)
                {                   
                    var difMinMinutes= _funcDate.Minutos(_configuration,porta.DtColeta.Value);                                

                    //Definindo a cor de fundo do quadrante de porta
                    var alarmes = _alm.SelectAlarmesAbertos(_configuration,0, "Porta",item.IdSensores);
                    if (alarmes.Count()>0)                
                        porta.CorDashboard = true;
                    else              
                        porta.CorDashboard = false;
                    /////////////

                    if (difMinMinutes) 
                            _retorno.Add(porta);
                }                    
            } 
            if (_retorno.Count()==0)
                return StatusCode(204);                    
            else
                return Ok(_retorno);     
        }

        [HttpGet]      
        public IActionResult  GetAlarmes()
        {
            AlarmesModel _model= new AlarmesModel();
            IEnumerable <Alarmes> _retorno;
                
                log.Debug("Get Do Dashboard quadro Alarmes Sala Limpa !");            
                _retorno=_model.SelectAlarmesAbertos(_configuration,99,null,0);
                if(_retorno!=null)
                    return Ok(_retorno);
                else
                    return StatusCode(204);
        }
    }
}