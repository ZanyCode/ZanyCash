import { Component, OnInit, Pipe, PipeTransform, Inject } from '@angular/core';
import { DataService } from '../services/data.service';
import { OnetimeTransactionModel, RecurringTransactionModel, PaymentAmountModel, PaymentIntervalModel, PaymentIntervalTypeModel } from '../models';
import {MatDialog, MatDialogRef, MAT_DIALOG_DATA} from '@angular/material/dialog';
import { OkCancelDialogComponent } from '../ok-cancel-dialog/ok-cancel-dialog.component';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { StreamService } from '../services/stream.service';
import Routes from '../routes.json';

@Pipe({name: 'currentAmount'})
export class CurrentAmountPipe implements PipeTransform {
  transform(amounts: PaymentAmountModel[]): number {
    if (amounts.length === 0) {
      return 0;
    }

    const today = new Date();
    if (amounts[0].date > today) {
      return amounts[0].amount;
    }

    const relevantAmountIdx = amounts.findIndex(a => a.date > today) - 1;
    return relevantAmountIdx < 0 ? amounts[amounts.length - 1].amount : amounts[relevantAmountIdx].amount;
  }
}


@Pipe({name: 'interval'})
export class IntervalPipe implements PipeTransform {
  transform(interval: PaymentIntervalModel): string {
    let intervalTypeString = '';
    switch (interval.intervalType){
      case PaymentIntervalTypeModel.daily:
        intervalTypeString = 'Days';
        break;
      case PaymentIntervalTypeModel.weekly:
        intervalTypeString = 'Weeks';
        break;
      case PaymentIntervalTypeModel.monthly:
        intervalTypeString = 'Months';
        break;
      case PaymentIntervalTypeModel.yearly:
        intervalTypeString = 'Years';
        break;
    }

    return `Every ${interval.interval} ${intervalTypeString}`;
  }
}

@Component({
  selector: 'app-transactions',
  templateUrl: './recurring-transactions.component.html',
  styleUrls: ['./recurring-transactions.component.scss']
})
export class RecurringTransactionsComponent {
  routes: typeof Routes;
  selectedTransaction: RecurringTransactionModel = {id: '-1'} as any;

  constructor(public data: DataService, public dialog: MatDialog, private http: HttpClient,
              private streamService: StreamService,
              @Inject('BASE_URL') private baseUrl: string) {
                this.routes = Routes;
               }

  transactionSelected(t: RecurringTransactionModel) {
    if (this.selectedTransaction.id === t.id) {
      this.selectedTransaction = {id: '-1'} as any;
    } else {
      this.selectedTransaction = t;
    }
  }

  deleteSelectedTransaction() {
    const dialogRef = this.dialog.open(OkCancelDialogComponent, {
      data: {caption: 'Delete Transaction', text: `Do you really want to delete Transaction #${this.selectedTransaction.id}? This can not be undone.`}
    });

    dialogRef.afterClosed().subscribe((shouldDelete: boolean) => {
      if (shouldDelete) {
        this.http.delete<OnetimeTransactionModel>(
          this.baseUrl + 'transaction/recurring-transaction/' + this.selectedTransaction.id)
          .subscribe(res => this.selectedTransaction = {id: '-1'} as any);
      }
    });
  }
}
