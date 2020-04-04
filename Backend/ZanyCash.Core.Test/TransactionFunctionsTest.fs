namespace ZanyCash.Core.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open ZanyCash.Core.Types
open ZanyCash.Core.TransactionFunctions
open ZanyCash.Core.LiquidityFunctions

[<TestClass>]
type TransactionFunctionsTest () =
    [<TestMethod>]
    member this.``Unrolling should work if within specified DateRange and singular Amount``() =
        let rTransaction = { 
            Id="kj";
            EndDate = DateTime(2019, 01, 03);
            Description = "Recurring Test"; 
            Interval = PaymentInterval (1,Daily);
            Amounts = [
                { Date=DateTime (2019, 1, 1); Amount=5.0 }
            ]
        }

        let unrolledTransactions = rTransaction |> UnrollRecurringTransaction (DateTime (2019, 01, 01)) (DateTime (2019, 01, 03))

        Assert.AreEqual(3, unrolledTransactions.Length);
        Assert.AreEqual([DateTime(2019, 01, 01);DateTime(2019, 01, 02);DateTime(2019, 01, 03)], unrolledTransactions |> List.map (fun x -> x.Date))
        Assert.AreEqual([5.0;5.0;5.0], unrolledTransactions |> List.map (fun x -> x.Amount))

    [<TestMethod>]
    member this.``Unrolling should work if within specified DateRange and multiple Amounts``() =
        let rTransaction = { 
            Id="kj";
            EndDate = DateTime(2019, 01, 03);
            Description = "Recurring Test"; 
            Interval = PaymentInterval (1,Daily);
            Amounts = [
                { Date = DateTime (2019, 1, 2); Amount = 6.0 };
                { Date = DateTime (2019, 1, 1); Amount = 5.0 }                        
            ]
        }

        let unrolledTransactions = rTransaction |> UnrollRecurringTransaction (DateTime(2019, 01, 01)) (DateTime(2019, 01, 03))

        Assert.AreEqual(3, unrolledTransactions.Length);
        Assert.AreEqual([DateTime(2019, 01, 01);DateTime(2019, 01, 02);DateTime(2019, 01, 03)], unrolledTransactions |> List.map (fun x -> x.Date))
        Assert.AreEqual([5.0;6.0;6.0], unrolledTransactions |> List.map (fun x -> x.Amount))

    [<TestMethod>]
    member this.``Unrolling should return no values if before start date``() =
        let rTransaction = { 
            Id="kj";
            EndDate = DateTime(2019, 01, 03);
            Description = "Recurring Test"; 
            Interval = PaymentInterval (1,Daily);
            Amounts = [
                        {Date = DateTime(2019, 01, 01); Amount=5.0}
            ]
        }

        let unrolledTransactions = rTransaction |> UnrollRecurringTransaction (DateTime(2018, 01, 01)) (DateTime(2018, 01, 03))
        Assert.AreEqual(0, unrolledTransactions.Length);   

    [<TestMethod>]
    member this.``Unrolling should return no values if after end date``() =
        let rTransaction = { 
            Id="kj";       
            EndDate = DateTime(2019, 01, 03);
            Description = "Recurring Test"; 
            Interval = PaymentInterval (1,Daily);
            Amounts = [
                        {Date=DateTime(2019, 01, 01); Amount=5.0}
            ]
        }

        let unrolledTransactions = rTransaction |> UnrollRecurringTransaction (DateTime(2020, 01, 01)) (DateTime(2020, 01, 03))
        Assert.AreEqual(0, unrolledTransactions.Length);   

    [<TestMethod>]
    member this.``Unrolling should only return Transactions within DateRange if partially before specified DateRange``() =
        let rTransaction = { 
                Id="kj";
                EndDate = DateTime(2019, 01, 03);
                Description = "Recurring Test"; 
                Interval = PaymentInterval (1,Daily);
                Amounts = [
                    {Date=DateTime(2019, 01, 01);Amount=5.0}
                ]
        }

        let unrolledTransactions = rTransaction |> UnrollRecurringTransaction (DateTime(2019, 01, 02)) (DateTime(2019, 01, 03))
        Assert.AreEqual(2, unrolledTransactions.Length);
        Assert.AreEqual([DateTime(2019, 01, 02);DateTime(2019, 01, 03)], unrolledTransactions |> List.map (fun x -> x.Date))
        Assert.AreEqual([5.0;5.0], unrolledTransactions |> List.map (fun x -> x.Amount))      

    [<TestMethod>]
    member this.``Unrolling should only return Transactions within DateRange if partially after specified DateRange``() =
        let rTransaction = { 
            Id="kj";       
            EndDate = DateTime(2019, 01, 03);
            Description = "Recurring Test"; 
            Interval = PaymentInterval (1,Daily);
            Amounts = [
                        {Date=DateTime(2019, 01, 01);Amount=5.0}
            ]
        }

        let unrolledTransactions = rTransaction |> UnrollRecurringTransaction (DateTime(2019, 01, 01)) (DateTime(2019, 01, 02))
        Assert.AreEqual(2, unrolledTransactions.Length);
        Assert.AreEqual([DateTime(2019, 01, 01);DateTime(2019, 01, 02)], unrolledTransactions |> List.map (fun x -> x.Date))
        Assert.AreEqual([5.0;5.0], unrolledTransactions |> List.map (fun x -> x.Amount))              

    [<TestMethod>]
    member this.``ToOnetimeTransactions should correctly convert recurring transaction``() =
        let rTransaction = { 
            Id="kj";       
            EndDate = DateTime(2019, 01, 03);
            Description = "Recurring Test"; 
            Interval = PaymentInterval (1,Daily);
            Amounts = [
                        {Date=DateTime(2019, 01, 01);Amount=5.0}
            ]
        }

        let onetimeTransactions = [RecurringTransaction rTransaction] 
                                    |> ToOnetimeTransactions (DateTime (2019, 1, 1)) (DateTime (2019,1,30))

        Assert.AreEqual(3, onetimeTransactions.Length);
        Assert.AreEqual(
            [DateTime(2019, 01, 01);DateTime(2019, 01, 02);DateTime(2019, 01, 03)],
            onetimeTransactions |> List.map (fun x -> x.Date))

        Assert.AreEqual([5.0;5.0;5.0], onetimeTransactions |> List.map (fun x -> x.Amount)) 
        
    [<TestMethod>]
    member this.``ToOnetimeTransactions should correctly convert mixed recurring and onetime transaction``() =
        let rTransaction = RecurringTransaction { 
            Id="kj";       
            EndDate = DateTime(2019, 01, 03);
            Description = "Recurring Test"; 
            Interval = PaymentInterval (1,Daily);
            Amounts = [
                        {Date=DateTime(2019, 01, 02);Amount=5.0}
            ]
        }
        
        let oTransaction = OnetimeTransaction { Date=DateTime (2019, 1, 1); Description="dfghjk"; Amount=7.0; Id="445"}

        let onetimeTransactions = [rTransaction; oTransaction] 
                                    |> ToOnetimeTransactions (DateTime (2019, 1, 1)) (DateTime (2019,1,3))

        Assert.AreEqual(3, onetimeTransactions.Length);
        Assert.AreEqual(
            [DateTime(2019, 01, 01);DateTime(2019, 01, 02);DateTime(2019, 01, 03)],
            onetimeTransactions |> List.map (fun x -> x.Date))

        Assert.AreEqual([7.0;5.0;5.0], onetimeTransactions |> List.map (fun x -> x.Amount))          
    
    [<TestMethod>]
    member this.``GetStartDate should correctly find StartDate for recurring transaction``() =        
        let t = RecurringTransaction { 
            Id="kj";       
            EndDate = DateTime(2019, 12, 31);
            Description = "Recurring Test"; 
            Interval = PaymentInterval (1,Monthly);
            Amounts = [
                        {Date=DateTime(2019, 01, 15);Amount=5.0}
                        {Date=DateTime(2019, 01, 20);Amount=5.0}
                        {Date=DateTime(2019, 03, 19);Amount=5.0}
                        {Date=DateTime(2019, 04, 15);Amount=5.0}

            ]
        }

        let startDate = GetStartDate t
        Assert.AreEqual (DateTime (2019,1,15), startDate)
        
    [<TestMethod>]
    member this.``TransactionWithin should correctly evaluate OnetimeTransaction``() =        
        let t = OnetimeTransaction { Date=DateTime (2019, 1, 1); Description="dfghjk"; Amount=7.0; Id="445"}

        Assert.IsTrue (TransactionWithin (DateTime (2018, 1, 1)) (DateTime (2020, 1, 1)) t)        
        Assert.IsTrue (TransactionWithin (DateTime (2019, 1, 1)) (DateTime (2020, 1, 1)) t) 
        Assert.IsTrue (TransactionWithin (DateTime (2018, 1, 1)) (DateTime (2019, 1, 1)) t) 
        Assert.IsTrue (TransactionWithin (DateTime (2019, 1, 1)) (DateTime (2019, 1, 1)) t) 

        Assert.IsFalse (TransactionWithin (DateTime (2018, 1, 1)) (DateTime (2018, 12, 31)) t) 
        Assert.IsFalse (TransactionWithin (DateTime (2019, 1, 2)) (DateTime (2020, 12, 31)) t) 

    [<TestMethod>]
    member this.``TransactionWithin should correctly evaluate RecurringTransaction``() =        
        let t = RecurringTransaction { 
            Id="kj";       
            EndDate = DateTime(2019, 12, 31);
            Description = "Recurring Test"; 
            Interval = PaymentInterval (1,Monthly);
            Amounts = [
                        {Date=DateTime(2019, 01, 15);Amount=5.0}
            ]
        }

        Assert.IsTrue (TransactionWithin (DateTime (2019, 1, 1)) (DateTime (2019, 1, 30)) t)        
        Assert.IsTrue (TransactionWithin (DateTime (2019, 1, 1)) (DateTime (2019, 1, 15)) t) 
        Assert.IsTrue (TransactionWithin (DateTime (2019, 1, 15)) (DateTime (2019, 1, 30)) t) 
        Assert.IsTrue (TransactionWithin (DateTime (2019, 1, 15)) (DateTime (2019, 1, 15)) t) 

        Assert.IsFalse (TransactionWithin (DateTime (2018, 1, 1)) (DateTime (2019, 1, 14)) t) 
        Assert.IsFalse (TransactionWithin (DateTime (2019, 1, 16)) (DateTime (2019, 2, 14)) t)    

    [<TestMethod>]
    member this.``GetErrorText for nested AggregateError with multiple errors should work correctly``() =
        let e1 = InvalidDate "Test"
        let e2 = MissingId
        let e3 = MissingAmounts
        let e4 = MissingInterval
        let e5 = AmountZero

        let aggregateError1 = AggregateError [e1;e2]
        let aggregateError = AggregateError [aggregateError1; e3; e4; e5]
        
        let aggregateErrorText = GetErrorText aggregateError
        let expectedErrorText = 
            [e1;e2;e3;e4;e5] 
            |> List.map GetErrorText
            |> List.reduce (fun s1 s2-> (s1 + Environment.NewLine + s2))

        Assert.AreEqual (expectedErrorText, aggregateErrorText)
  