using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ZanyCash.Core.Types;

namespace ZanyCash.Glue
{
    public enum PaymentIntervalTypeModel
    {
        daily,
        weekly,
        monthly,
        yearly
    }

    public class PaymentIntervalModel 
    {
        public int interval { get; set; }

        public PaymentIntervalTypeModel intervalType { get; set; }
    }

    public class PaymentAmountModel
    {
        public DateTime date { get; set; }

        public double amount { get; set; }
    }

    public class TransactionModel
    {
        public bool isOnetimeTransaction { get; set; }

        public OnetimeTransactionModel onetimeTransaction { get; set; }

        public RecurringTransactionModel recurringTransaction { get; set; }
    }

    public class OnetimeTransactionModel
    {
        public string id { get; set; }

        public DateTime date { get; set; }

        public string description { get; set; }

        public double amount { get; set; }
    }

    public class RecurringTransactionModel
    {
        public string id { get; set; }

        public DateTime endDate { get; set; }

        public string description { get; set; }

        public PaymentIntervalModel interval { get; set; }

        public IEnumerable<PaymentAmountModel> amounts { get; set; }
    }

    public static class MappingExtensions
    {
        public static PaymentIntervalTypeModel ToModel(this PaymentIntervalType @this)
        {
            var enumValues = new []{
                PaymentIntervalTypeModel.daily,
                PaymentIntervalTypeModel.weekly ,
                PaymentIntervalTypeModel.monthly ,
                PaymentIntervalTypeModel.yearly };

            int selectedIdx = 
                @this.IsWeekly ? 0 : 1 + 
                (@this.IsMonthly ? 0 : 2) + 
                (@this.IsYearly ? 0 : 3);

            return enumValues[selectedIdx];
        }

        public static PaymentIntervalModel ToModel(this PaymentInterval @this)
        {
            return new PaymentIntervalModel
            {
                intervalType = @this.Item2.ToModel(),
                interval = @this.Item1
            };
        }

        public static PaymentAmountModel ToModel(this PaymentAmount @this)
        {
            return new PaymentAmountModel
            {
                amount = @this.Amount,
                date = @this.Date
            };
        }

        public static TransactionModel ToModel(this Transaction @this)
        {
            switch (@this)
            {
                case Transaction.OnetimeTransaction ot:
                    var otModel = new OnetimeTransactionModel
                    {
                        id = ot.Item.Id,
                        amount = ot.Item.Amount,
                        date = ot.Item.Date,
                        description = ot.Item.Description
                    };

                    return new TransactionModel { isOnetimeTransaction = true, onetimeTransaction = otModel };
                case Transaction.RecurringTransaction rt:
                    var rtModel = new RecurringTransactionModel
                    {
                        id = rt.Item.Id,
                        endDate = rt.Item.EndDate,
                        description = rt.Item.Description,
                        interval = rt.Item.Interval.ToModel(),
                        amounts = rt.Item.Amounts.Select(a => a.ToModel())
                    };

                    return new TransactionModel { isOnetimeTransaction = false, recurringTransaction = rtModel };
            }

            throw new Exception("Error while mapping transaction: Invalid case");
        }
    }
}
