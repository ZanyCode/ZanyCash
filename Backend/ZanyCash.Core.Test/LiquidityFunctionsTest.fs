namespace ZanyCash.Core.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open ZanyCash.Core.Types
open ZanyCash.Core.LiquidityFunctions

[<TestClass>]
type LiquidityFunctionsTests () =           
    [<TestMethod>]
    member this.``Liquidity steps should be correct for recurring transaction with singular Amount``() =
        let rTransaction = { 
            Id="kj";       
            EndDate = new DateTime(2019, 01, 03);
            Description = "Recurring Test"; 
            Interval = PaymentInterval (1,Daily);
            Amounts = [
                        {Date=new DateTime(2019, 01, 01);Amount=5.0}
            ]
        }

        let t1 = RecurringTransaction rTransaction
        let liquiditySteps = [t1] |> ToDayLiquidities (new DateTime(2019, 01, 01)) (new DateTime(2019, 01, 04))
        Assert.AreEqual(4, liquiditySteps.Length);

        Assert.AreEqual(
            [new DateTime(2019, 01, 01);new DateTime(2019, 01, 02); new DateTime(2019, 01, 03); new DateTime(2019, 01, 04)], 
            liquiditySteps |> List.map (fun t -> t.Date))

        Assert.AreEqual([0.0;5.0;10.0;15.0], liquiditySteps |> List.map (fun t -> t.DailyMinimum))    
        
    [<TestMethod>]
    member this.``Liquidity steps should be correct for recurring transaction with multiple Amounts``() =        
        let rTransaction = { 
            Id="kj";       
            EndDate = new DateTime(2019, 01, 03);
            Description = "Recurring Test"; 
            Interval = PaymentInterval (1,Daily);
            Amounts = [
                        {Date=new DateTime(2019, 01, 01);Amount=5.0};
                        {Date=new DateTime(2019, 01, 03);Amount=10.0}
            ]
        }

        let t1 = RecurringTransaction rTransaction
        let liquiditySteps = [t1] |> ToDayLiquidities (new DateTime(2019, 01, 01)) (new DateTime(2019, 01, 04))
        Assert.AreEqual(4, liquiditySteps.Length);

        Assert.AreEqual(
            [new DateTime(2019, 01, 01);new DateTime(2019, 01, 02); new DateTime(2019, 01, 03); new DateTime(2019, 01, 04)], 
            liquiditySteps |> List.map (fun t -> t.Date))

        Assert.AreEqual([0.0;5.0;10.0;20.0], liquiditySteps |> List.map (fun t -> t.DailyMinimum))    

    [<TestMethod>]
    member this.``Liquidity steps should be correct for multiple OnetimeTransactions``() =
        let t1 = OnetimeTransaction {Id="ghj"; Date=new DateTime(2019, 01, 01); Amount=5.0; Description="jjj"}
        let t2 = OnetimeTransaction {Id="ghj"; Date=new DateTime(2019, 01, 02); Amount=5.0; Description="jjj"}
        let t3 = OnetimeTransaction {Id="ghj"; Date=new DateTime(2019, 01, 03); Amount=10.0; Description="jjj"}

        let liquiditySteps = [t1;t2;t3] |> ToDayLiquidities (new DateTime(2019, 01, 01)) (new DateTime(2019, 01, 04))
        Assert.AreEqual(4, liquiditySteps.Length);

        Assert.AreEqual(
            [new DateTime(2019, 01, 01);new DateTime(2019, 01, 02); new DateTime(2019, 01, 03); new DateTime(2019, 01, 04)], 
            liquiditySteps |> List.map (fun t -> t.Date))

        Assert.AreEqual([0.0;5.0;10.0;20.0], liquiditySteps |> List.map (fun t -> t.DailyMinimum))    
    
    [<TestMethod>]
    member this.``Liquidity steps should be correct for mixed recurring and onetime transaction``() =
        let t1 = RecurringTransaction { 
            Id="kj";       
            EndDate = new DateTime(2019, 01, 03);
            Description = "Recurring Test"; 
            Interval = PaymentInterval (1,Daily);
            Amounts = [
                        {Date=new DateTime(2019, 01, 01); Amount=5.0}
            ]
        }

        let t2 = OnetimeTransaction {Id="ghj"; Date=new DateTime(2019, 01, 02); Amount=7.0; Description="jjj"}

        let liquiditySteps = [t1;t2] |> ToDayLiquidities (new DateTime(2019, 01, 01)) (new DateTime(2019, 01, 04))
        Assert.AreEqual(4, liquiditySteps.Length);

        Assert.AreEqual(
            [new DateTime(2019, 01, 01);new DateTime(2019, 01, 02); new DateTime(2019, 01, 03); new DateTime(2019, 01, 04)], 
            liquiditySteps |> List.map (fun t -> t.Date))

        Assert.AreEqual([0.0;5.0;17.0;22.0], liquiditySteps |> List.map (fun t -> t.DailyMinimum))    

    [<TestMethod>]
    member this.``Liquidity steps should ignore Transactions not within DateRange``() =
        let t1 = OnetimeTransaction {Id="ghj"; Date=new DateTime(2019, 01, 01); Amount=5.0; Description="jjj"}
        let t2 = OnetimeTransaction {Id="ghj"; Date=new DateTime(2019, 01, 02); Amount=(5.0); Description="jjj"}
        let t3 = OnetimeTransaction {Id="ghj"; Date=new DateTime(2019, 01, 03); Amount=10.0; Description="jjj"}

        let liquiditySteps = [t1;t2;t3] |> ToDayLiquidities (new DateTime(2019, 01, 02)) (new DateTime(2019, 01, 02))
        Assert.AreEqual(1, liquiditySteps.Length);

        Assert.AreEqual(
            [new DateTime(2019, 01, 02)],
            liquiditySteps |> List.map (fun t -> t.Date))

        Assert.AreEqual([0.], liquiditySteps |> List.map (fun t -> t.DailyMinimum))    

    [<TestMethod>]
    member this.``Liquidity should include negative transactions in daily minimum calculation``() =
        let t1 = OnetimeTransaction {Id="ghj"; Date=new DateTime(2019, 01, 01); Amount=5.0; Description="jjj"}
        let t2 = OnetimeTransaction {Id="ghj"; Date=new DateTime(2019, 01, 02); Amount=(-5.0); Description="jjj"}
        let t3 = OnetimeTransaction {Id="ghj"; Date=new DateTime(2019, 01, 03); Amount=10.0; Description="jjj"}

        let liquiditySteps = [t1;t2;t3] |> ToDayLiquidities (new DateTime(2019, 01, 01)) (new DateTime(2019, 01, 04))
        Assert.AreEqual(4, liquiditySteps.Length);

        Assert.AreEqual(
            [new DateTime(2019, 01, 01);new DateTime(2019, 01, 02); new DateTime(2019, 01, 03); new DateTime(2019, 01, 04)], 
            liquiditySteps |> List.map (fun t -> t.Date))

        Assert.AreEqual([0.0;0.0;0.0;10.0], liquiditySteps |> List.map (fun t -> t.DailyMinimum))    