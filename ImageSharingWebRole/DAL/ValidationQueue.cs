using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System.Threading;
using System.Net;
using Microsoft.Azure;
using ImageSharingWebRole.Models;

namespace ImageSharingWebRole.DAL
{
    public class ValidationQueue
    {
        
        public const string QueueName = "ValidationQueue";
        static QueueClient Client;

        public static void Initialize()
        {
            try
            {
                // Set the maximum number of concurrent connections 
                ServicePointManager.DefaultConnectionLimit = 12;

                // Create the queue if it does not exist already
                string connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
                var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
                if (!namespaceManager.QueueExists(QueueName))
                {
                    namespaceManager.CreateQueue(QueueName);
                }
                Client = QueueClient.CreateFromConnectionString(connectionString, QueueName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void Send(ValidationRequest Request)
        {
            if (Client == null)
            {
                Initialize();
            }

            try
            {   
                Client.Send(new BrokeredMessage(Request));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Finalize()
        {
            // Close the connection to Service Bus Queue
            Client.Close();
        }
    }
}