using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ZanyCash.Glue;
using ZanyStreams;
using static ZanyCash.Core.Types;
using static ZanyCash.Core.Types.Command;

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
            var command = NewCreateTransactionCommand(new Actions.CreateTransaction(Transaction.NewOnetimeTransaction(transaction.ToDomain())));
            core.RunCommand(command);
        }

        [HttpPost("recurring-transaction")]
        public void AddRecurringTransaction([FromBody] RecurringTransactionModel transaction, [FromHeader(Name = "ConnectionId")]string connectionId)
        {
            var core = serviceLocator.GetRequiredService<CoreAdapter>(connectionId);
            var command = NewCreateTransactionCommand(new Actions.CreateTransaction(Transaction.NewRecurringTransaction(transaction.ToDomain())));
            core.RunCommand(command);
        }

        [HttpPut("onetime-transaction")]
        public void UpdateOnetimeTransaction([FromBody] OnetimeTransactionModel transaction, [FromHeader(Name = "ConnectionId")]string connectionId)
        {
            var core = serviceLocator.GetRequiredService<CoreAdapter>(connectionId);
            var command = NewUpdateTransactionCommand(new Actions.UpdateTransaction(Transaction.NewOnetimeTransaction(transaction.ToDomain())));
            core.RunCommand(command);
        }

        [HttpPut("recurring-transaction")]
        public void UpdateRecurringTransaction([FromBody] RecurringTransactionModel transaction, [FromHeader(Name = "ConnectionId")]string connectionId)
        {
            var core = serviceLocator.GetRequiredService<CoreAdapter>(connectionId);
            var command = NewUpdateTransactionCommand(new Actions.UpdateTransaction(Transaction.NewRecurringTransaction(transaction.ToDomain())));
            core.RunCommand(command);
        }

        [HttpDelete("onetime-transaction/{id}")]
        public void DeleteOnetimeTransaction(string id, [FromHeader(Name = "ConnectionId")]string connectionId)
        {
            var core = serviceLocator.GetRequiredService<CoreAdapter>(connectionId);
            var command = NewDeleteTransactionCommand(new Actions.DeleteTransaction(id));
            core.RunCommand(command);
        }

        [HttpDelete("recurring-transaction/{id}")]
        public void DeleteRecurringTransaction(string id, [FromHeader(Name = "ConnectionId")]string connectionId)
        {
            var core = serviceLocator.GetRequiredService<CoreAdapter>(connectionId);
            var command = NewDeleteTransactionCommand(new Actions.DeleteTransaction(id));
            core.RunCommand(command);
        }
    }
}
