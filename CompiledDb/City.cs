using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiledDb
{
    [Table("City")]
    public class City
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public long Population { get; set; }

        public string Icon => default;

        public string MenuDisplay => "Города";
    }
}
