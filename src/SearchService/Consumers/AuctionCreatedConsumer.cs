using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Contracts;

namespace SearchService.Consumers
{
    public class AuctionCreatedConsumer : IConsumer<AuctionCreated>
    {
        public Task Consume(ConsumeContext<AuctionCreated> context)
        {
            throw new NotImplementedException();
        }
    }
}