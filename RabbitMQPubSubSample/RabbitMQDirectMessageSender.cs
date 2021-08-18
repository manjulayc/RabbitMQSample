
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
namespace RabbitMQPubSubSample
{
    public class RabbitMQDirectMessageSender : IRabbitMQMessageSender
    {
        private readonly string _hostname;
        private readonly string _password;
        private readonly string _username;
        private IConnection _connection;
        private IConfiguration _configuration;

        private const string ExchangeName = "Direct_Exchange";
        private const string QueueName = "WeatherDirectQueue";
        public RabbitMQDirectMessageSender(IConfiguration configuration)
        {

            _hostname = "localhost";
            _password = "guest";
            _username = "guest";
            _configuration = configuration;

        }
        public void SendMessage(WeatherForecast message, String queueName)
        {
            var factory = new ConnectionFactory
            {
                HostName = _hostname,
                UserName = _username,
                Password = _password
            };
            _connection = factory.CreateConnection();
            using var model = _connection.CreateModel();
            model.ExchangeDeclare(ExchangeName, type: ExchangeType.Direct);

            model.QueueDeclare(QueueName, true, false, false, null);
            model.QueueBind(QueueName, ExchangeName, "WeatherKey");


            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);
            model.BasicPublish(ExchangeName, "WeatherKey", basicProperties: null, body: body);
        }
    }
}
