import { Component, OnInit } from '@angular/core';
import { DataService } from '../services/data.service';
import { map } from 'rxjs/operators';
import { OnetimeTransactionModel, RecurringTransactionModel } from '../models';

@Component({
  selector: 'app-transactions',
  templateUrl: './transactions.component.html',
  styleUrls: ['./transactions.component.scss']
})
export class TransactionsComponent {
  constructor(public data: DataService) {
  }
}
