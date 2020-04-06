using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ZanyCash.Glue;
using ZanyStreams;
using static ZanyCash.Core.Types;

namespace ZanyCash.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransactionController : ControllerBase
    {
        private IScopedServiceLocator serviceLocator;

        public TransactionController( IScopedServiceLocator serviceLocator)
        {
            this.serviceLocator = serviceLocator;
        }

        [HttpPost("onetime-transaction")]
        public void AddOnetimeTransaction([FromBody] OnetimeTransactionModel transaction, [FromHeader(Name ="ConnectionId")]string connectionId)
        {
            var core = serviceLocator.GetRequiredService<CoreAdapter>(connectionId);
            core.AddOnetimeTransaction(transaction.ToDomain());
        }

        [HttpDelete("onetime-transaction/{id}")]
        public void DeleteOnetimeTransaction(string id, [FromHeader(Name = "ConnectionId")]string connectionId)
        {
            var core = serviceLocator.GetRequiredService<CoreAdapter>(connectionId);
            core.DeleteOnetimeTransaction(id);
        }
    }
}
