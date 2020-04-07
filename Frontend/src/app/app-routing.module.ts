import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { TransactionsComponent } from './transactions/transactions.component';
import { SetTransactionComponent } from './set-transaction/set-transaction.component';
import { OkCancelDialogComponent } from './ok-cancel-dialog/ok-cancel-dialog.component';
import { Routes, Route } from './routes';

const routes: Routes = {
  Default: new Route({ path: '', redirectTo: '/onetime-transactions', pathMatch: 'full', display: false}),
  OnetimeTransactions: new Route({ path: 'onetime-transactions', component:  TransactionsComponent, display: true, label: 'Single Transactions'}),
  AddOnetimeTransaction: new Route({ path: 'onetime-transactions/:action', component:  SetTransactionComponent, display: true, label: 'Add Onetime'})
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
