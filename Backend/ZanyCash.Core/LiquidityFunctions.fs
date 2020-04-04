module ZanyCash.Core.LiquidityFunctions
open ZanyCash.Core.TransactionFunctions
open System

let GetDayLiquidityDate dayLiquidiy = dayLiquidiy.Date

let ToDayLiquidities startDay endDay transactions =             
    let transactionsByDay =
        transactions 
        |> ToOnetimeTransactions startDay endDay
        |> List.filter (fun f -> f.Date >= startDay)
        |> List.sortBy (fun f -> (f.Date.Date,f.Amount))
        |> List.groupBy (fun f -> f.Date.Date)
    
    let rec getDayLiquidities initialValue currentDay (transactionDays:(DateTime*OnetimeTransaction list) list) currentLiquidities = 
        match currentDay > endDay with
        | true -> 
            currentLiquidities |> List.rev
        | false ->            
            let nextDay = currentDay.AddDays (1.)
            match transactionDays with
            | (day, dayTransactions)::remaining when day = currentDay ->
                let minLiquidity = initialValue + 
                                    (dayTransactions |> List.filter (fun f -> f.Amount < 0.) |> List.sumBy (fun f->f.Amount))
                                
                let endOfDayLiquidity = initialValue + (dayTransactions |> List.sumBy (fun f->f.Amount))
                let dayLiquidity = {Date=currentDay; DailyMinimum=minLiquidity; Transactions=dayTransactions}
                let newLiquidities = dayLiquidity::currentLiquidities
                getDayLiquidities endOfDayLiquidity nextDay remaining newLiquidities
            | _ ->
                let dayLiquidity = {Date=currentDay; DailyMinimum=initialValue; Transactions=[]}
                let newLiquidities = dayLiquidity::currentLiquidities
                getDayLiquidities initialValue nextDay transactionDays newLiquidities                        

    let x = getDayLiquidities 0. startDay transactionsByDay []
    x