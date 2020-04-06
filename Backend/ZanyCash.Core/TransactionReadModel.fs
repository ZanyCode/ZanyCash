module ZanyCash.Core.TransactionReadModel

open System
open ZanyCash.Core
open ZanyCash.Core.TransactionFunctions

type State = { Transactions: Transaction list }

let GetInitialState () = 
    { Transactions = [] }

let HandleEvent (state: State) = function
    | TransactionCreatedEvent e ->
        { state with Transactions = e.Transaction :: state.Transactions }
    | TransactionUpdatedEvent e -> 
        let idToUpdate = e.Transaction |> GetId
        let updatedTransactionList = state.Transactions |> List.map (fun t -> if t |> GetId = idToUpdate then e.Transaction else t)
        {state with Transactions = updatedTransactionList}
    | TransactionDeletedEvent e ->
        let updatedTransactionList = state.Transactions |> List.filter (fun t -> t |> GetId <> e.Id)
        { state with Transactions = updatedTransactionList }
    | _ -> state

let QueryTransactionById (id: string) (state: State): Transaction =    
    state.Transactions |> List.filter (fun t -> t |> GetId = id) |> List.head

let QueryTransactionsWithinDaterange (startDay: DateTime) (endDay: DateTime) (state: State): Transaction list =        
    state.Transactions |> List.filter (TransactionWithin startDay endDay)