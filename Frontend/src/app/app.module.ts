import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { SetOnetimeTransactionComponent } from './set-onetime-transaction/set-onetime-transaction.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {MatSortModule} from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import {MatGridListModule} from '@angular/material/grid-list';
import {NgbModule} from '@ng-bootstrap/ng-bootstrap';
import { OnetimeTransactionsComponent } from './onetime-transactions/onetime-transactions.component';
import {MatTabsModule} from '@angular/material/tabs';
import {MatButtonModule} from '@angular/material/button';
import {MatInputModule} from '@angular/material/input';
import {MatFormFieldModule} from '@angular/material/form-field';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import {HttpClientModule} from '@angular/common/http';
import {MatIconModule, MatIcon} from '@angular/material/icon';
import { OkCancelDialogComponent } from './ok-cancel-dialog/ok-cancel-dialog.component';
import {MatDialogModule} from '@angular/material/dialog';
import { SetRecurringTransactionComponent } from './set-recurring-transaction/set-recurring-transaction.component';
import { RecurringTransactionsComponent, CurrentAmountPipe, IntervalPipe } from './recurring-transactions/recurring-transactions.component';
import {MatDividerModule} from '@angular/material/divider';
import { LiquidityDashboardComponent } from './liquidity-dashboard/liquidity-dashboard.component';


@NgModule({
  declarations: [
    AppComponent,
    SetOnetimeTransactionComponent,
    SetRecurringTransactionComponent,
    OnetimeTransactionsComponent,
    RecurringTransactionsComponent,
    OkCancelDialogComponent,
    CurrentAmountPipe,
    IntervalPipe,
    LiquidityDashboardComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    MatSortModule,
    MatTableModule,
    MatGridListModule,
    NgbModule,
    MatTabsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    MatIconModule,
    MatDialogModule,
    MatDividerModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
