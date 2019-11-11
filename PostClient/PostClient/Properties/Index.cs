using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Server.Properties
{
    [Serializable]
    class Index
    {
        public int ID { get; set; }
        public int Code { get; set; }
        public string Street { get; set; } // случайно так назвад, бред полный (Это index)
    }
}
