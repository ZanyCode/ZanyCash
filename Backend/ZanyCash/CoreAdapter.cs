using Microsoft.FSharp.Collections;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ZanyCash.Core;
using static ZanyCash.Core.Types;
using static ZanyCash.Core.Types.Command;

namespace ZanyCash
{
    public class CoreAdapter
    {
        public IObservable<FSharpList<Transaction>> Transactions { get; }

        private Subject<Command> commands = new Subject<Command>();

        public CoreAdapter()
        {
            var sideEffectEvents = new Subject<Event>();
            var events = commands
                .Select(c => TransactionWriteModel.HandleCommand(c).Concat(LiquidityWriteModel.HandleCommand(c)))
                .Select(events => events.ToObservable())
                .Switch()
                .Merge(sideEffectEvents);

            
            var transactionReadState = events.Scan(TransactionReadModel.GetInitialState(), TransactionReadModel.HandleEvent);
            var transactionWriteState = events.Scan(TransactionWriteModel.GetInitialState(), (state, evt) =>
            {
                var (newState, newEvent) = TransactionWriteModel.HandleEvent(state, evt);
                if (newEvent != null)
                    sideEffectEvents.OnNext(newEvent.Value);

                return newState;
            });
            var liquidityReadState = events.Scan(LiquidityReadModel.GetInitialState(), LiquidityReadModel.HandleEvent);
            var liquidityWriteState = events.Scan(LiquidityWriteModel.GetInitialState(), (state, evt) =>
            {
                var (newState, newEvent) = LiquidityWriteModel.HandleEvent(state, evt);
                if (newEvent != null)
                    sideEffectEvents.OnNext(newEvent.Value);

                return newState;
            });

            transactionReadState.Subscribe(s =>
            {
                // Persist State
            });

            liquidityReadState.Subscribe(s =>
            {
                // Persist State
            });

            liquidityWriteState.Subscribe(s =>
            {
                // Persist State
            });

            transactionWriteState.Subscribe(s =>
            {
                // Persist state
            });

            var t = transactionReadState.Select(s => s.Transactions).Replay(1);
            this.Transactions = t;
            t.Connect();
        }

        public void RunCommand(Command c)
        {
            commands.OnNext(c);
        }
    }
}
