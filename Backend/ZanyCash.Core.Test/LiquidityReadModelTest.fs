namespace ZanyCash.Core.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open ZanyCash.Core.Types
open ZanyCash.Core.LiquidityReadModel

[<TestClass>]
type LiquidityReadModelTest () =           
    [<TestMethod>]
    member this.``LiquidityReadModel should correctly filter days without transactions``() =
        let t1 = {Id="ghj"; Date=new DateTime(2019, 01, 01); Amount=5.0; Description="jjj"}
        let d1 = { Date=DateTime (2000,1,1); DailyMinimum=10.0; StartLiquidity=20.0; EndLiquidity=0.0; Transactions=[t1] }
        let d2 = { Date=DateTime (2000,1,2); DailyMinimum=10.0; StartLiquidity=20.0; EndLiquidity=0.0; Transactions=[] }
        let d3 = { Date=DateTime (2000,1,3); DailyMinimum=10.0; StartLiquidity=20.0; EndLiquidity=0.0; Transactions=[] }
        let d4 = { Date=DateTime (2000,1,4); DailyMinimum=10.0; StartLiquidity=20.0; EndLiquidity=0.0; Transactions=[] }
        let d5 = { Date=DateTime (2000,1,5); DailyMinimum=10.0; StartLiquidity=20.0; EndLiquidity=0.0; Transactions=[t1] }

        let state = {LiquidityPerDay=[d1;d2;d3;d4;d5]}
        let liquidities = QueryLiquidityWithinDaterange (DateTime (2000,1,1)) (DateTime (2000,1,5)) state
        Assert.AreEqual(2, List.length liquidities)

        