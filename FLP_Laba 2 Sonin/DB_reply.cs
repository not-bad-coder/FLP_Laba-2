using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLP_Laba_2_Sonin
{
    internal class DB_reply
    {
        public int id { get; set; }
        private string one;
        private string two;

        public string One {
            get { return one; }
            set { one = value; }
        }
        public string Two
        {
            get { return two; }
            set { two = value; }
        }

        public DB_reply() { }
        public override string ToString()
        {
            return one + " " + two;
        }

    }
}
