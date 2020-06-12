module ZanyCash.Core.TransactionWriteModel
open ZanyCash.Core.TransactionFunctions

type State = { currentId: int }

let GetInitialState() = { currentId = 1337}


let HandleCommand = function
    | CreateTransactionCommand t -> [TransactionCreationRequestedEvent t]
    | UpdateTransactionCommand t -> [TransactionUpdatedEvent t]
    | DeleteTransactionCommand t -> [TransactionDeletedEvent t]
    | _ -> []

let HandleEvent (state: State) = function
| TransactionCreationRequestedEvent e ->   
    let newState = { state with currentId=state.currentId + 1 }      
    let updatedTransaction = SetId e.Transaction (string newState.currentId)
    newState,Some (TransactionCreatedEvent ({ Transaction=updatedTransaction }))

| _ -> state,None