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
    state.LiquidityPerDay |> List.filter (fun ld -> ld.Date >= startDay && ld.Date <= endDay)