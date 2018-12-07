using System.Collections.Generic;
using Embraer_Backend.Models;

namespace Embraer_Backend.Models
{
    public class IluminanciaIndex
    {
       public IEnumerable<Equipamentos> AtivoFixo { get; set; }
       public IEnumerable<LocalMedicao> Local { get; set; }

        public IluminanciaIndex()
        {
            this.AtivoFixo = new List<Equipamentos>();
            this.Local = new List<LocalMedicao>();
        }

    }
}