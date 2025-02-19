using System.Collections.Generic;

namespace ProjectCleanning_Backend.Models
{
    public class LimpezaIndex
    {
       public List<string> TipoControle { get; set; }
       public IEnumerable<LocalMedicao> Local { get; set; }

        public LimpezaIndex()
        {
            this.TipoControle = new List<string>();
            this.Local = new List<LocalMedicao>();
        }

    }
}