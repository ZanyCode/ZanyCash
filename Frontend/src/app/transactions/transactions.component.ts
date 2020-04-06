import { Component, OnInit } from '@angular/core';
import { DataService } from '../services/data.service';
import { OnetimeTransactionModel, RecurringTransactionModel } from '../models';
import {MatDialog, MatDialogRef, MAT_DIALOG_DATA} from '@angular/material/dialog';
import { OkCancelDialogComponent } from '../ok-cancel-dialog/ok-cancel-dialog.component';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { StreamService } from '../services/stream.service';


@Component({
  selector: 'app-transactions',
  templateUrl: './transactions.component.html',
  styleUrls: ['./transactions.component.scss']
})
export class TransactionsComponent {
  selectedTransaction: OnetimeTransactionModel = {id: '-1'} as any;

  constructor(public data: DataService, public dialog: MatDialog, private http: HttpClient, private streamService: StreamService) { }

  transactionSelected(t: OnetimeTransactionModel) {
    if (this.selectedTransaction.id === t.id) {
      this.selectedTransaction = {id: '-1'} as any;
    }
    else {
      this.selectedTransaction = t;
    }
  }

  deleteSelectedTransaction()
  {
    const dialogRef = this.dialog.open(OkCancelDialogComponent, {
      data: {caption: 'Delete Transaction', text: `Do you really want to delete Transaction #${this.selectedTransaction.id}? This can not be undone.`}
    });

    dialogRef.afterClosed().subscribe((shouldDelete: boolean) => {
      if (shouldDelete) {
        this.streamService.connectionId.then(connectionId => {
          this.http.delete<OnetimeTransactionModel>(
            'transaction/onetime-transaction/' + this.selectedTransaction.id,
            {headers: new HttpHeaders({ConnectionId: '123'})})
            .subscribe(res => this.selectedTransaction = {id: '-1'} as any);
        });
      }
    });
  }
}
