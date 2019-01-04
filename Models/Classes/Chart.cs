using System;
using System.Collections.Generic;
using Embraer_Backend.Models;

namespace Embraer_Backend.Models
{
    public class Charts
    {    
        public List<DateTime> categories{get;set;}
        public List<Pena> series{get;set;}
        
        public Charts()
        {
            this.categories = new List<DateTime>();
            this.series = new List<Pena>();
        }
    }
    public class Pena
    {
        public string name {get;set;}
        public List<decimal> data {get;set;}

    
        public Pena()
        {
            this.name = string.Empty;
            this.data = new List<decimal>();
        }
    }
}
