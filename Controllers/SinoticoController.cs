using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ProjectCleanning_Backend.Models;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.AspNetCore.Cors;
using System.Linq;

namespace ProjectCleanning_Backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [EnableCors("CorsPolicy")]
    public class SinoticoController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(SinoticoController));
         private readonly IConfiguration _configuration;
        
        List<Sinotico> _sl=new List<Sinotico>();
        List<Sinotico> _pt=new List<Sinotico>();
        SinoticoModel _sn= new SinoticoModel();
    
        public SinoticoController(IConfiguration configuration) 
        {            
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult GetSalaLimpa()
        {                       
            _sl.AddRange(_sn.SelectSinotico(_configuration));

            if(_sl.Count()>0)
            {                   
                log.Debug("Sinótico Sala Limpa - Dados retornados com sucesso:" + _sl);  
                return Ok(_sl);
            }
            return StatusCode(500,"Houve um erro, verifique o Log do sistema!");
        }

        [HttpGet]
        public IActionResult GetSalaPintura()
        {
                         
            _pt.AddRange(_sn.SelectSinotico(_configuration,13,14,15,16,2));
            _pt.AddRange(_sn.SelectSinotico(_configuration,9,10,11,12,3));
            _pt.AddRange(_sn.SelectSinotico(_configuration,5,6,7,8,4));
            _pt.AddRange(_sn.SelectSinotico(_configuration,1,2,3,4,5));
            _pt.AddRange(_sn.SelectSinotico(_configuration,17,18,19,20,6));
            _pt.AddRange(_sn.SelectSinotico(_configuration,21,22,23,24,7));
            _pt.AddRange(_sn.SelectSinotico(_configuration,27,28,29,30,8));
            _pt.AddRange(_sn.SelectSinotico(_configuration,31,32,33,34,9));
            _pt.AddRange(_sn.SelectSinotico(_configuration,0,25,26,0,13));


            if(_pt.Count()>0)
            {                   
                log.Debug("Sinótico Sala Pintura - Dados retornados com sucesso:" + _pt);  
                return Ok(_pt);
            }
            return StatusCode(500,"Houve um erro, verifique o Log do sistema!");
        }
            
       
        
    }
}