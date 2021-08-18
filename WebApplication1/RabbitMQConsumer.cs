using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

using RabbitMQ.Client;
using Microsoft.Extensions.Hosting;
using System.Threading;
using RabbitMQ.Client.Events;

namespace RabbitMQPubSubscriber
{  
    public class RabbitMQConsumer : BackgroundService
    {


        private readonly IConfiguration _configuration;

        //rabbitmq
        private IConnection _connection;
        private IModel _model;

       

        public RabbitMQConsumer( IConfiguration configuration 
           )
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            _connection = factory.CreateConnection();
            _model = _connection.CreateModel();
         //   _model.ExchangeDeclare(ExchangeName, type: ExchangeType.Fanout);

            _model.QueueDeclare(queue: "weatherqueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
         


            _configuration = configuration;
        
       

            

          }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_model);
            consumer.Received += (ch, ea) =>
            {

                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                var updateCustomerFullNameModel = JsonConvert.DeserializeObject<WeatherForecast>(content);

               // HandleMessage(updateCustomerFullNameModel).GetAwaiter().GetResult();

                _model.BasicAck(ea.DeliveryTag, false);
            };
          
           string s= _model.BasicConsume("weatherqueue", false, consumer);

            return Task.CompletedTask;
        }


        

       
    }
}
