using Exercise17.FuncApi.Models;
using Exercise17.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exercise17.FuncApi.Helpers
{
    public static class Mappers
    {
        public static Machine ToMachine(this MachineEntity machineEntity)
        {
            return new Machine
            {
                Id = machineEntity.RowKey,
                Name = machineEntity.Name,
                Online = machineEntity.Online,
            };
        }

        public static MachineEntity ToMachineEntity(this Machine machine)
        {
            return new MachineEntity
            {
                RowKey = machine.Id,
                Name = machine.Name,
                Online = machine.Online,
                PartitionKey = "Machines"

            };
        }
        
    }
}
