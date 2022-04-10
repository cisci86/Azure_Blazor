using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos.Table;
using Exercise17.FuncApi.Models;
using Exercise17.FuncApi.Helpers;
using System.Linq;
using Exercise17.Shared;
using Microsoft.Azure.Cosmos.Table.Queryable;

namespace Exercise17.FuncApi
{
    public static class Api
    {
        [FunctionName("Get")]
        public static async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "machines")] HttpRequest req,
            [Table("machinepark", Connection = "AzureWebJobsStorage")] CloudTable table,
            ILogger log)
        {
            log.LogInformation("Getting all items...");

            var query = new TableQuery<MachineEntity>();
            var res = await table.ExecuteQuerySegmentedAsync(query, null);
            var result = res.Select(Mappers.ToMachine).ToList();

            return new OkObjectResult(result);
        }
        [FunctionName("Get/{id}")]
        public static async Task<IActionResult> GetOne(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "machines/{id}")] HttpRequest req,
            [Table("machinepark", Connection = "AzureWebJobsStorage")] CloudTable table,
            ILogger log)
        {

        }

        [FunctionName("Create")]
        public static async Task<IActionResult> Create(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route ="machines")] HttpRequest req,
            [Table("machinepark", Connection = "AzureWebJobsStorage")] IAsyncCollector<MachineEntity> machines,
            ILogger log)
        {
            log.LogInformation("Creating new machine");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var createMachine = JsonConvert.DeserializeObject<CreateMachineDto>(requestBody);

            if (createMachine == null) return new BadRequestResult();

            var machine = new Machine
            {
                Name = createMachine.Name,
                Online = false
            };

            await machines.AddAsync(machine.ToMachineEntity());

            return new OkObjectResult(machine);
        }

        [FunctionName("Update")]
        public static async Task<IActionResult> UpdateData(
             [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "machines/{id}")] HttpRequest req,
             [Table("IndividualMachines", Connection = "AzureWebJobsStorage")] CloudTable table,
             string id,
             ILogger log)
        {
            log.LogInformation("Updating machine");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var machineToUpdate = JsonConvert.DeserializeObject<Machine>(requestBody);

            if (machineToUpdate is null || machineToUpdate.Id != id) return new BadRequestResult();

            var machineEntity = new MachineEntity
            {
                Name = machineToUpdate.Name,
                Online = machineToUpdate.Online,
                Data = machineToUpdate.Data,
                RowKey = DateTime.Now.ToString("g"),
                PartitionKey = id
            };

            return new NoContentResult();
        }
    }
}
