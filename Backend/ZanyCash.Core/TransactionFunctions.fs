module ZanyCash.Core.TransactionFunctions
open System
open ZanyCash.Core

let ValidateTransaction = function
    | RecurringTransaction t ->         
        Ok (RecurringTransaction t) 
    | OnetimeTransaction t ->  
        Ok (OnetimeTransaction t)

let GetId = function
    | RecurringTransaction r -> r.Id
    | OnetimeTransaction r -> r.Id

let GetStartDate = function
    | RecurringTransaction r -> r.Amounts 
                                |> List.sortBy (fun a -> a.Date) 
                                |> List.map (fun a-> a.Date)
                                |> List.head 

    | OnetimeTransaction t -> t.Date

let GetEndDate = function
    | RecurringTransaction r -> r.EndDate
    | OnetimeTransaction t -> t.Date

let TransactionHasId (id: string) (transaction: Transaction) : bool = 
    (transaction |> GetId) = id

let TransactionsAreEqual (t1: Transaction) (t2: Transaction) : bool = 
    (GetId t1) = (GetId t2)

let private getNextDate (currentDate: DateTime) (paymentInterval: PaymentInterval) = 
    let (PaymentInterval (multiplier, intervalType)) = paymentInterval
    match intervalType with
    | Daily -> currentDate.AddDays (float multiplier)
    | Weekly -> currentDate.AddDays (float (multiplier * 7))
    | Monthly -> currentDate.AddMonths multiplier
    | Yearly -> currentDate.AddYears multiplier

let UnrollRecurringTransaction (startDay: DateTime) (endDay: DateTime) (recTransaction: RecurringTransaction) = 
    let endDay = if endDay < recTransaction.EndDate then endDay else recTransaction.EndDate
    let rec getTransactionDates date =
        match date > endDay with
        | true -> []
        | false -> date::getTransactionDates (getNextDate date recTransaction.Interval)

    let transactionDates = getTransactionDates (GetStartDate (RecurringTransaction recTransaction))
                            |> List.filter (fun d -> d >= startDay && d <= endDay)

    let rec createOnetimeTransactions dates = 
        match dates with
        | [] -> []
        | (current::remaining) ->
            let amount = recTransaction.Amounts 
                            |> List.filter (fun a -> a.Date <= current) 
                            |> List.sortByDescending (fun a -> a.Date) 
                            |> List.map (fun a -> a.Amount)
                            |> List.head
            
            let onetimeTransaction = {Id=recTransaction.Id; Date = current; Description = recTransaction.Description; Amount = amount}
            onetimeTransaction::createOnetimeTransactions remaining
    
    createOnetimeTransactions transactionDates     

let ToOnetimeTransactions startDay endDay transactions = 
    let toOnetimeTransactions = function
        | RecurringTransaction recTransaction -> UnrollRecurringTransaction startDay endDay recTransaction
        | OnetimeTransaction t -> [t]
    
    transactions 
    |> List.collect toOnetimeTransactions 
    |> List.sortBy (fun t -> t.Date.Date,t.Amount)

let rec TransactionWithin (startDay: DateTime) (endDay: DateTime) (t: Transaction) : bool = 
    match t with 
    | RecurringTransaction t ->
        let unrolledTransactions = UnrollRecurringTransaction startDay endDay t
        unrolledTransactions |> List.map OnetimeTransaction |> List.exists (TransactionWithin startDay endDay)
    | OnetimeTransaction t -> t.Date.Date >= startDay.Date && t.Date.Date <= endDay.Date  

let rec GetErrorText = function
| InvalidDate d -> "Specified date is invalid: " + d
| MissingId -> "No id specified"
| MissingDescription -> "No description specified"
| MissingAmounts -> "There were no amounts specified"
| MissingInterval -> "No payment interval specified"
| AmountZero -> "The amount must not be zero"
| DuplicateId id -> "The specified id does already exist: " + id
| Unexpected e -> e.Message
| AggregateError (first::remaining) -> 
    remaining 
    |> List.fold (fun acc elem -> acc + Environment.NewLine + GetErrorText elem) (GetErrorText first)
| AggregateError [] -> "Error: empty list of aggregate errors passed, so not sure what happened here"
