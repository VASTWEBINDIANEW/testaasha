using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.JsonData
{
    public class PersonModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public List<string> Shoes { get; set; }
        public bool Hungry { get; set; }
        public int Age { get; set; }
    }
}