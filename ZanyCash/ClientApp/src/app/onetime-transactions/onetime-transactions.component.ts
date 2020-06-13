import { Component, OnInit, Inject } from '@angular/core';
import { DataService } from '../services/data.service';
import { OnetimeTransactionModel, RecurringTransactionModel } from '../models';
import {MatDialog, MatDialogRef, MAT_DIALOG_DATA} from '@angular/material/dialog';
import { OkCancelDialogComponent } from '../ok-cancel-dialog/ok-cancel-dialog.component';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { StreamService } from '../services/stream.service';
import Routes from '../routes.json';
import { AuthorizeService } from 'src/api-authorization/authorize.service';


@Component({
  selector: 'app-transactions',
  templateUrl: './onetime-transactions.component.html',
  styleUrls: ['./onetime-transactions.component.scss']
})
export class OnetimeTransactionsComponent {
  selectedTransaction: OnetimeTransactionModel = {id: '-1'} as any;
  routes: typeof Routes;


  constructor(public data: DataService, public dialog: MatDialog, private http: HttpClient,
              private streamService: StreamService,
              private as: AuthorizeService,
              @Inject('BASE_URL') private baseUrl: string) {
                this.routes = Routes;
              }

  transactionSelected(t: OnetimeTransactionModel) {
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
          this.baseUrl + 'transaction/onetime-transaction/' + this.selectedTransaction.id)
          .subscribe(res => this.selectedTransaction = {id: '-1'} as any);
      }
    });
  }
}
