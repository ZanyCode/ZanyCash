using Microsoft.FSharp.Collections;
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
                @this.IsWeekly ? 1 : 0 + 
                (@this.IsMonthly ? 2 : 0) + 
                (@this.IsYearly ? 3 : 0);

            return enumValues[selectedIdx];
        }

        public static PaymentIntervalType ToDomain(this PaymentIntervalTypeModel @this)
        {
            switch(@this)
            {
                case PaymentIntervalTypeModel.daily:
                    return PaymentIntervalType.Daily;
                case PaymentIntervalTypeModel.weekly:
                    return PaymentIntervalType.Weekly;
                case PaymentIntervalTypeModel.monthly:
                    return PaymentIntervalType.Monthly;
                case PaymentIntervalTypeModel.yearly:
                    return PaymentIntervalType.Yearly;
            }

            throw new Exception("Invalid enum value when mapping to domain: " + @this);
        }

        public static PaymentIntervalModel ToModel(this PaymentInterval @this)
        {
            return new PaymentIntervalModel
            {
                intervalType = @this.Item2.ToModel(),
                interval = @this.Item1
            };
        }

        public static PaymentInterval ToDomain(this PaymentIntervalModel @this)
        {
            return PaymentInterval.NewPaymentInterval(@this.interval, @this.intervalType.ToDomain());
        }

        public static PaymentAmountModel ToModel(this PaymentAmount @this)
        {
            return new PaymentAmountModel
            {
                amount = @this.Amount,
                date = @this.Date
            };
        }

        public static PaymentAmount ToDomain(this PaymentAmountModel @this)
        {
            return new PaymentAmount(@this.date, @this.amount);
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
    
        public static OnetimeTransaction ToDomain(this OnetimeTransactionModel @this)
        {
            return new OnetimeTransaction(@this.id, @this.date, @this.description, @this.amount);
        }

        public static RecurringTransaction ToDomain(this RecurringTransactionModel @this)
        {
            var amounts = ListModule.OfSeq(@this.amounts.Select(x => x.ToDomain()));
            return new RecurringTransaction(@this.id, @this.endDate, @this.description, @this.interval.ToDomain(), amounts);
        }

        public static Transaction ToDomain(this TransactionModel @this)
        {
            return @this.isOnetimeTransaction ?
                Transaction.NewOnetimeTransaction(@this.onetimeTransaction.ToDomain()) :
                Transaction.NewRecurringTransaction(@this.recurringTransaction.ToDomain());

        }
    }
}
