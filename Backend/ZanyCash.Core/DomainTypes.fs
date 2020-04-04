[<AutoOpen>]
module ZanyCash.Core.Types
open System

type PaymentIntervalType = Daily | Weekly | Monthly | Yearly
type PaymentInterval = | PaymentInterval of int * PaymentIntervalType
type PaymentAmount = { Date: DateTime; Amount: float }

type RecurringTransaction = { 
    Id: string;
    EndDate: DateTime;
    Description: string; 
    Interval: PaymentInterval;
    Amounts: PaymentAmount list;
 }

type OnetimeTransaction = { Id: string; Date: DateTime; Description: string; Amount: float }

type Transaction =
    | OnetimeTransaction of OnetimeTransaction
    | RecurringTransaction of RecurringTransaction

type Error =
    | InvalidDate of string
    | MissingId
    | MissingDescription
    | MissingAmounts
    | MissingInterval
    | AmountZero
    | DuplicateId of string
    | Unexpected of Exception
    | AggregateError of Error list

type DayLiquidity = { Date: DateTime; DailyMinimum: float; Transactions: OnetimeTransaction list }
type Balance = { Date: DateTime; Amount: float }

module Actions =
    type CreateTransaction = { Transaction: Transaction }
    type UpdateTransaction = { Transaction: Transaction }
    type DeleteTransaction = { Transaction: Transaction }

type Command = 
    | CreateTransactionCommand of Actions.CreateTransaction
    | UpdateTransactionCommand of Actions.UpdateTransaction
    | DeleteTransactionCommand of Actions.DeleteTransaction
    | SetLiquidityCalculationEndDateCommand of DateTime

type Event = 
    | Nothing
    | TransactionCreatedEvent of Actions.CreateTransaction
    | TransactionUpdatedEvent of Actions.UpdateTransaction
    | TransactionDeletedEvent of Actions.DeleteTransaction
    | LiquidityUpdatedEvent of DayLiquidity list
    | LiquidityCalculationEndDateSetEvent of DateTime