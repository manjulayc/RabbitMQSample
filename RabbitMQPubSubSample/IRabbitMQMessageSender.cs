
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQPubSubSample
{
    public interface IRabbitMQMessageSender
    {
        void SendMessage(WeatherForecast message,String queueName);
    }
}
