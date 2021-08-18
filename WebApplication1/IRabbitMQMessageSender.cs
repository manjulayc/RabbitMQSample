﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rabbitmqsample
{
    public interface IRabbitMQMessageSender
    {
        void SendMessage(BaseMessage message,String queueName);
    }
}
