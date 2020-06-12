module ZanyCash.Core.LiquidityWriteModel
open System
open ZanyCash.Core.Types
open ZanyCash.Core.LiquidityFunctions
open ZanyCash.Core.TransactionFunctions

type State = { CalculationEndDay: DateTime; Transactions: Transaction list; }

let GetInitialState () = 
    { 
        CalculationEndDay = DateTime (2050, 1, 5); 
        Transactions = []
    }

let HandleCommand = function 
    | SetLiquidityCalculationEndDateCommand a -> [LiquidityCalculationEndDateSetEvent a]
    | _ -> []

let private generateLiquidityUpdatedEvent state = 
    let startDate = state.Transactions |> List.map GetStartDate |> List.min
    let dayLiquidities = state.Transactions |> ToDayLiquidities startDate state.CalculationEndDay
    LiquidityUpdatedEvent dayLiquidities

let HandleEvent (state: State) = function
    | LiquidityCalculationEndDateSetEvent e ->   
        let newState = { state with CalculationEndDay=e }       
        newState,Some (generateLiquidityUpdatedEvent newState)
    | TransactionCreatedEvent e ->             
        let newState = { state with Transactions = e.Transaction :: state.Transactions }
        newState,Some (generateLiquidityUpdatedEvent newState)
    | TransactionUpdatedEvent e -> 
        let idToUpdate = e.Transaction |> GetId
        let updatedTransactionList = state.Transactions |> List.map (fun t -> if t |> GetId = idToUpdate then e.Transaction else t)
        let newState = {state with Transactions = updatedTransactionList}
        newState,Some (generateLiquidityUpdatedEvent newState)
    | TransactionDeletedEvent e ->
        let updatedTransactionList = state.Transactions |> List.filter (fun t -> t |> GetId <> e.Id)
        let newState = { state with Transactions = updatedTransactionList }
        newState,Some (generateLiquidityUpdatedEvent newState)
    | _ -> state,None