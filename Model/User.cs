using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToMi.Model
{
    public class User
    {
        public int id { get; set; } 
        public string name { get; set; }
        public string firstname { get; set; }
        public string? pantromimic { get; set; }
        public string address { get; set; }
        public string? phone_number { get; set; }
        public string password { get; set; }
    }
}
