using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using FunctionAppAndRestApi.Models;
using System.Collections.Generic;
using System.Linq;

namespace FunctionAppAndRestApi
{
    public static class Function1
    {
        public static readonly List<Tasks> Items = new List<Tasks>();

        [FunctionName("CreateTask")]
        public static async Task<IActionResult> CreateTask(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "task")] HttpRequest req, ILogger log)
        {
            log.LogInformation("Creating a new Task list item");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<TaskCreateModel>(requestBody);

            var task = new Tasks() { TaskDescription = input.TaskDescription };
            Items.Add(task);

            return new OkObjectResult(task);
        }

        [FunctionName("GetAllTasks")]
        public static IActionResult GetAllTask(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "task")] HttpRequest req, ILogger log)
        {
            log.LogInformation("Getting Task list items");
            return new OkObjectResult(Items);
        }

        [FunctionName("GetTaskById")]
        public static IActionResult GetTaskById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "task/{id}")] HttpRequest req, ILogger log, string id
            )
        {
            var task = Items.FirstOrDefault(x => x.Id == id);

            if (task == null)
                return new NotFoundResult();

            return new OkObjectResult(task);
        }

        [FunctionName("UpdateTask")]
        public static async Task<IActionResult> UpdateTask(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route ="task/{id}")] HttpRequest req, ILogger log, string id
            )
        {
            var task = Items.FirstOrDefault(x => x.Id == id);

            if (task == null)
                return new NotFoundResult();

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var updated = JsonConvert.DeserializeObject<TaskUpdateModel>(requestBody);

            task.IsCompleted = updated.IsCompleted;

            if (!string.IsNullOrEmpty(updated.TaskDescription))
                task.TaskDescription = updated.TaskDescription;

            return new OkObjectResult(task);
        }

        [FunctionName("DeleteTask")]
        public static IActionResult DeleteTask(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route ="task/{id}")] HttpRequest req, ILogger log, string id
            )
        {
            var task = Items.FirstOrDefault(x => x.Id == id);

            if (task == null)
                return new NotFoundResult();

            Items.Remove(task);
            return new OkResult();
        }
    }
}
