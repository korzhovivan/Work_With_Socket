using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;

namespace Server.Properties
{
    [Table(Name = "Indexe")]
    class Index
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int ID { get; set; }
        [Column]
        public int Code { get; set; }
        [Column]
        public string Street { get; set; } // случайно так назвад, бред полный (Это index)
    }
}
