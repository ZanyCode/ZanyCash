import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import {MatSortModule} from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import {MatGridListModule} from '@angular/material/grid-list';
import {MatTabsModule} from '@angular/material/tabs';
import {MatButtonModule} from '@angular/material/button';
import {MatInputModule} from '@angular/material/input';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatIconModule, MatIcon} from '@angular/material/icon';
import {MatDialogModule} from '@angular/material/dialog';
import {MatDividerModule} from '@angular/material/divider';
import { ReactiveFormsModule } from '@angular/forms';
import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { ApiAuthorizationModule } from 'src/api-authorization/api-authorization.module';
import { AuthorizeGuard } from 'src/api-authorization/authorize.guard';
import { AuthorizeInterceptor } from 'src/api-authorization/authorize.interceptor';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { OnetimeTransactionsComponent } from './onetime-transactions/onetime-transactions.component';
import Routes from './routes.json'
import { SetOnetimeTransactionComponent } from './set-onetime-transaction/set-onetime-transaction.component';
import { OkCancelDialogComponent } from './ok-cancel-dialog/ok-cancel-dialog.component';
import { RecurringTransactionsComponent, IntervalPipe, CurrentAmountPipe } from './recurring-transactions/recurring-transactions.component';
import { SetRecurringTransactionComponent } from './set-recurring-transaction/set-recurring-transaction.component';
import { LiquidityDashboardComponent } from './liquidity-dashboard/liquidity-dashboard.component';


@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    OkCancelDialogComponent,
    FetchDataComponent,
    OnetimeTransactionsComponent,
    SetOnetimeTransactionComponent,
    RecurringTransactionsComponent,
    SetRecurringTransactionComponent,
    LiquidityDashboardComponent,
    CurrentAmountPipe,
    IntervalPipe,
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    ApiAuthorizationModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'counter', component: CounterComponent },
      { path: 'fetch-data', component: FetchDataComponent, canActivate: [AuthorizeGuard] },
      { path: Routes.OnetimeTransactions, component: OnetimeTransactionsComponent },
      { path: Routes.SetOnetimeTransaction, component: SetOnetimeTransactionComponent },
      { path: Routes.RecurringTransactions, component: RecurringTransactionsComponent },
      { path: Routes.SetRecurringTransactions, component: SetRecurringTransactionComponent },
      { path: Routes.Liquidity, component: LiquidityDashboardComponent },
    ]),
    BrowserAnimationsModule,
    MatSortModule,
    MatTableModule,
    MatGridListModule,
    MatTabsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    ReactiveFormsModule,
    MatIconModule,
    MatDialogModule,
    MatDividerModule    
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthorizeInterceptor, multi: true }
  ],
  entryComponents: [OkCancelDialogComponent],
  bootstrap: [AppComponent]
})
export class AppModule { }
