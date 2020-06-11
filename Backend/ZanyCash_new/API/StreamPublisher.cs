using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using ZanyCash.Glue;
using ZanyStreams;

namespace ZanyCash.API
{
    public class StreamPublisher : IStreamPublisher
    {
        private readonly CoreAdapter core;
        private readonly StreamNames streamNames;

        public StreamPublisher(CoreAdapter core, StreamNames streamNames)
        {
            this.core = core;
            this.streamNames = streamNames;
        }

        public IEnumerable<Stream> GetStreams()
        {    
            return new List<Stream>()
            {                
                core.Transactions.Select(
                    t => t
                    .Select(MappingExtensions.ToModel)
                    .Where(t=>t.isOnetimeTransaction)
                    .Select(t=>t.onetimeTransaction))
                .ToStream(streamNames.onetimeTransactions),

                core.Transactions.Select(
                    t => t
                    .Select(MappingExtensions.ToModel)
                    .Where(t=>!t.isOnetimeTransaction)
                    .Select(t=>t.recurringTransaction))
                .ToStream(streamNames.recurringTransactions),

                core.Liquidity
                    .Select(dayLiquidities => dayLiquidities.Select(MappingExtensions.ToModel))
                    .ToStream(streamNames.liquidity)
            };
        }
    }
}
