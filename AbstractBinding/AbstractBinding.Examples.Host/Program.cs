﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;

namespace AbstractBinding.Examples.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialize recipient
            var serializer = new Serializer();
            var recipient = new Recipient(serializer);
            recipient.Register<IExampleObject>("obj1", new ExampleObject());

            // Start server
            var recipientService = new RecipientService(recipient);
            var server = new Server
            {
                Services = { Protos.RecipientService.BindService(recipientService) },
                Ports = { new ServerPort("127.0.0.1", 6789, ServerCredentials.Insecure) }
            };
            server.Start();

            Console.WriteLine("Server started...");
            Console.ReadLine();
        }
    }
}
