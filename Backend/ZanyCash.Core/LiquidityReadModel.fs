module ZanyCash.Core.LiquidityReadModel

open System
open ZanyCash.Core

type State = { LiquidityPerDay: DayLiquidity list }

let GetInitialState () = 
    { LiquidityPerDay = [] }

let HandleEvent (state: State) = function
    | LiquidityUpdatedEvent liquidityDays ->
        { state with LiquidityPerDay = liquidityDays }
    | _ -> state

let QueryLiquidityWithinDaterange (startDay: DateTime) (endDay: DateTime) (state: State) : DayLiquidity list = 
    state.LiquidityPerDay 
    |> List.filter (fun ld -> ld.Date >= startDay && ld.Date <= endDay)
    |> List.fold 
        (fun acc v ->
            match acc with
            | head::rest when (List.length v.Transactions) > 0 -> v::acc
            | head::rest -> acc
            | [] -> [v]) []     
     |> List.rev