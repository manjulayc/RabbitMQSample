
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
namespace RabbitMQPubSubscriber
{
    public class RabbitMQDirectMessageSender : IRabbitMQMessageSender
    {
        private readonly string _hostname;
        private readonly string _password;
        private readonly string _username;
        private IConnection _connection;
        private IConfiguration _configuration;

        private const string ExchangeName = "DirectRouting_Exchange";
        private const string PaymentEmailUpdateQueueName = "WeatherDirectQueue";
        public RabbitMQDirectMessageSender(IConfiguration configuration)
        {

            _hostname = "localhost";
            _password = "guest";
            _username = "guest";
            _configuration = configuration;

        }
        public void SendMessage(BaseMessage message, String queueName)
        {
            
            if (ConnectionExists())
            {
                using var model = _connection.CreateModel();
                model.ExchangeDeclare(ExchangeName, type: ExchangeType.Direct);

                model.QueueDeclare(PaymentEmailUpdateQueueName, true, false, false, null);
                model.QueueBind(PaymentEmailUpdateQueueName, ExchangeName, "WeatherKey");


                var json = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(json);
                model.BasicPublish(ExchangeName, "WeatherKey", basicProperties: null, body: body);
            }
        }
        
        private void CreateConnection()
        {
            try
            {
               string connection = _configuration.GetValue<string>("EventBusSettings:HostAddress");
                var factory = new ConnectionFactory
                {
                    HostName = _hostname,
                    UserName = _username,
                    Password = _password
                };
                _connection = factory.CreateConnection();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not create connection: {ex.Message}");
            }
        }

        private bool ConnectionExists()
        {
            if (_connection != null)
            {
                return true;
            }

            CreateConnection();

            return _connection != null;
        }
    }
}
