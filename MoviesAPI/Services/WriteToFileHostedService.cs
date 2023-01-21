using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MoviesAPI.Services
{
    public class WriteToFileHostedService : IHostedService
    {
        private readonly IWebHostEnvironment env;
        private readonly string fileName = "File_1.txt";
        private Timer timer;

        public WriteToFileHostedService(IWebHostEnvironment env)
        {
            this.env = env;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(callback: DoWork,
                              state: null,
                              dueTime: TimeSpan.Zero,
                              period: TimeSpan.FromSeconds(5));
            WriteToFile("Process Started");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            WriteToFile("Process Stopped");
            return Task.CompletedTask;
        }

        private void WriteToFile(string message)
        {
            string path = $@"{env.ContentRootPath}\wwwroot\{fileName}";
            using StreamWriter writer = new StreamWriter(path, append: true);
            writer.WriteLine(message);
        }

        private void DoWork(object state)
        {
            WriteToFile($"Process Ongoing:{DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")}");

        }
    }
}
