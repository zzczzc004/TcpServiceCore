﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using TcpServiceCore.Communication;
using TcpServiceCore.Dispatching;
using TcpServiceCore.Protocol;
using TcpServiceCore.Tools;

namespace TcpServiceCore.Server
{
    class ServerRequestHandler<T> : RequestStreamHandler where T: new()
    {
        IInstanceContextFactory<T> instanceContextFactory;

        Dictionary<string, ChannelConfig> channelConfigs;
        bool isAccepted;

        public ServerRequestHandler(TcpClient client, 
            Dictionary<string, ChannelConfig> channelConfigs,
            IInstanceContextFactory<T> instanceContextFactory)
            : base(client)
        {
            this.instanceContextFactory = instanceContextFactory;
            this.channelConfigs = channelConfigs;
        }

        protected override async Task OnRequestReceived(Request request)
        {
            if (isAccepted)
            {
                await DoHandleRequest(request);
            }
            else
            {
                var contract = request.Contract;
                if (string.IsNullOrEmpty(contract))
                    throw new Exception($"Wrong socket initialization, Request.Contract should not be null or empty");
                var channelConfig = this.channelConfigs.FirstOrDefault(x => x.Key == contract);
                if (channelConfig.Value == null)
                    throw new Exception($"Wrong socket initialization, contract {contract} is missing");

                this.Client.Configure(channelConfig.Value);

                await DoHandleRequest(request);

                isAccepted = true;
            }
        }

        async Task DoHandleRequest(Request request)
        {
            var context = this.instanceContextFactory.Create(this.Client);
            await context.HandleRequest(request, this);
        }
    }
}