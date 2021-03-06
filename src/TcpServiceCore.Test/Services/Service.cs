﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TcpServiceCore.Attributes;
using TcpServiceCore.Dispatching;

namespace TcpServiceCore.Test.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Service : IService
    {
        public Task<string> Echo(string msg)
        {
            //Console.WriteLine(OperationContext.Current == null);
            return Task.FromResult(msg.ToUpper());
        }

        public Task<Msg> EchoMany(Msg msg1, Msg msg2, string str, int x)
        {
            var msg = new Msg
            {
                Body = $"msg1: {msg1.Body}, msg2: {msg2.Body}, Str: {str}, X: { x }"
            };

            return Task.FromResult(msg);
        }

        public Task<Msg> EchoMsg(Msg msg)
        {
            msg.Id = 100;
            msg.Body = "From Server";
            return Task.FromResult(msg);
        }

        public Task<string> EchoNoParam()
        {
            return Task.FromResult("No Parameters Received");
        }

        public Task<string> EchoServerError()
        {
            throw new NotImplementedException();
        }

        public Task FireMsg(string msg)
        {
            Console.WriteLine($"received {msg}");
            return Task.CompletedTask;
        }
    }
}
