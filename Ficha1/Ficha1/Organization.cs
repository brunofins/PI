using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ficha1
{
    public class Organization
    {
        public String name { set; get; }
        public String local { set; get; }




        public Organization(string name, string local)
        {
            this.name = name;
            this.local = local;
        }
    }
}
