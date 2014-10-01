using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ficha1
{
    public class LanguageOccurrences
    {
        public String name { set; get; }
        public int occurrences { set; get; }

        public LanguageOccurrences(string name, int occ)
        {
            this.name = name;
            this.occurrences = occ;
        }

    }
}
