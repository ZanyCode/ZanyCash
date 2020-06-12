namespace ZanyCash.Core.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open ZanyCash.Core.Types
open ZanyCash.Core.LiquidityWriteModel

[<TestClass>]
type LiquidityWriteModelTest () =           
    [<TestMethod>]
    member this.``LiquidityWriteModel should select correct calculation StartDate depending on state transaction list``() =
        let t1 = OnetimeTransaction {Id="ghj"; Date=new DateTime(2019, 01, 01); Amount=5.0; Description="jjj"}
        let t2 = OnetimeTransaction {Id="ghj"; Date=new DateTime(2019, 01, 02); Amount=(-5.0); Description="jjj"}
        let t3 = OnetimeTransaction {Id="ghj"; Date=new DateTime(2019, 01, 03); Amount=10.0; Description="jjj"}

        let state = {CalculationEndDay=DateTime (2019,1,3); Transactions=[t1;t2;t3]}
        let event = LiquidityCalculationEndDateSetEvent (DateTime (2019,1,3))

        let (_,newEvent) = HandleEvent state event

        match newEvent with
        | Some (LiquidityUpdatedEvent e) ->
            Assert.AreEqual(DateTime (2019,1,1), e.Head.Date)
        | _ -> 
            Assert.IsTrue(false)

        