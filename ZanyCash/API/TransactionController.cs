using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ZanyCash.Glue;
using ZanyCash.Models;
using ZanyStreams;
using static ZanyCash.Core.Types;
using static ZanyCash.Core.Types.Command;

namespace ZanyCash.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class TransactionController : ControllerBase
    {
        private IScopedServiceLocator serviceLocator;

        public TransactionController(IScopedServiceLocator serviceLocator)
        {
            this.serviceLocator = serviceLocator;
        }

        [HttpGet("onetime-transaction")]
        public int GetOnetimeTransaction()
        {
            return 42;
        }

        [HttpPost("onetime-transaction")]
        public void AddOnetimeTransaction([FromBody] OnetimeTransactionModel transaction)
        {
            var core = serviceLocator.GetRequiredService<CoreAdapter>(this.GetUserId());
            var command = NewCreateTransactionCommand(new Actions.CreateTransaction(Transaction.NewOnetimeTransaction(transaction.ToDomain())));
            core.RunCommand(command);
        }

        [HttpPost("recurring-transaction")]
        public void AddRecurringTransaction([FromBody] RecurringTransactionModel transaction)
        {
            var core = serviceLocator.GetRequiredService<CoreAdapter>(this.GetUserId());
            var command = NewCreateTransactionCommand(new Actions.CreateTransaction(Transaction.NewRecurringTransaction(transaction.ToDomain())));
            core.RunCommand(command);
        }

        [HttpPut("onetime-transaction")]
        public void UpdateOnetimeTransaction([FromBody] OnetimeTransactionModel transaction)
        {
            var core = serviceLocator.GetRequiredService<CoreAdapter>(this.GetUserId());
            var command = NewUpdateTransactionCommand(new Actions.UpdateTransaction(Transaction.NewOnetimeTransaction(transaction.ToDomain())));
            core.RunCommand(command);
        }

        [HttpPut("recurring-transaction")]
        public void UpdateRecurringTransaction([FromBody] RecurringTransactionModel transaction)
        {
            var core = serviceLocator.GetRequiredService<CoreAdapter>(this.GetUserId());
            var command = NewUpdateTransactionCommand(new Actions.UpdateTransaction(Transaction.NewRecurringTransaction(transaction.ToDomain())));
            core.RunCommand(command);
        }

        [HttpDelete("onetime-transaction/{id}")]
        public void DeleteOnetimeTransaction(string id)
        {
            var core = serviceLocator.GetRequiredService<CoreAdapter>(this.GetUserId());
            var command = NewDeleteTransactionCommand(new Actions.DeleteTransaction(id));
            core.RunCommand(command);
        }

        [HttpDelete("recurring-transaction/{id}")]
        public void DeleteRecurringTransaction(string id)
        {
            var core = serviceLocator.GetRequiredService<CoreAdapter>(this.GetUserId());
            var command = NewDeleteTransactionCommand(new Actions.DeleteTransaction(id));
            core.RunCommand(command);
        }

        [HttpPut("liquidity/date-range")]
        public void UpdateLiquidityDateRange([FromQuery]DateTime startDate, [FromQuery]DateTime endDate)
        {
            var core = serviceLocator.GetRequiredService<CoreAdapter>(this.GetUserId());
            core.SetLiquidityDateRange(startDate, endDate);
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
