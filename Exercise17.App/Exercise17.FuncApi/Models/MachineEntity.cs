using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exercise17.FuncApi.Models
{
    public class MachineEntity : TableEntity
    {
        public string Name { get; set; }
        public bool Online { get; set; }
    }
}
