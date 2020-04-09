import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { OnetimeTransactionsComponent } from './onetime-transactions/onetime-transactions.component';
import { SetOnetimeTransactionComponent } from './set-onetime-transaction/set-onetime-transaction.component';
import { OkCancelDialogComponent } from './ok-cancel-dialog/ok-cancel-dialog.component';
import { Routes, Route } from './routes';
import { SetRecurringTransactionComponent } from './set-recurring-transaction/set-recurring-transaction.component';
import { RecurringTransactionsComponent } from './recurring-transactions/recurring-transactions.component';

const routes: Routes = {
  Default: new Route({ path: '', redirectTo: '/onetime-transactions', pathMatch: 'full', display: false}),
  OnetimeTransactions: new Route({ path: 'onetime-transactions', component:  OnetimeTransactionsComponent, display: true, label: 'Single Transactions'}),
  RecurringTransactions: new Route({ path: 'recurring-transactions', component:  RecurringTransactionsComponent, display: true, label: 'Recurring Transactions'}),
  SetOnetimeTransaction: new Route({ path: 'onetime-transactions/:action', component:  SetOnetimeTransactionComponent, display: false}),
  SetRecurringTransaction: new Route({ path: 'recurring-transactions/:action', component: SetRecurringTransactionComponent, display: false})
};

export const appRouting = RouterModule.forRoot(Object.values(routes));

@NgModule({
  imports: [
    RouterModule.forRoot(Object.values(routes)),
    CommonModule
  ],
  exports: [ RouterModule ],
  declarations: [],
  providers: [
    { provide: Routes, useValue: routes}
  ]
})
export class AppRoutingModule { }
