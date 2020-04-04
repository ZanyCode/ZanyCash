module ZanyCash.Core.TransactionWriteModel

let HandleCommand = function
    | CreateTransactionCommand t -> [TransactionCreatedEvent t]
    | UpdateTransactionCommand t -> [TransactionUpdatedEvent t]
    | DeleteTransactionCommand t -> [TransactionDeletedEvent t]
    | _ -> []