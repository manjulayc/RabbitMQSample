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

namespace RabbitMQPubSubSample
{  
    public class RabbitMQDirectConsumer : BackgroundService
    {


        private readonly IConfiguration _configuration;

        //rabbitmq
        private IConnection _connection;
        private IModel _model;

        private const string ExchangeName = "Direct_Exchange";
        private const string QueueName = "WeatherDirectQueue";


        public RabbitMQDirectConsumer( IConfiguration configuration 
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
            _model.ExchangeDeclare(ExchangeName, type: ExchangeType.Fanout);
            _model.QueueDeclare(QueueName, true, false, false, null);
            _model.QueueBind(QueueName, ExchangeName, "WeatherKey");



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


                _model.BasicAck(ea.DeliveryTag, false);
            };
          
           string s = _model.BasicConsume(QueueName, false, consumer);

            return Task.CompletedTask;
        }


        

       
    }
}
