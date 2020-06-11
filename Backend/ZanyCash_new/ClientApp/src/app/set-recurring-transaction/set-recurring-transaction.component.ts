import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import {formatDate} from '@angular/common';
import { OnetimeTransactionModel, RecurringTransactionModel, PaymentIntervalTypeModel } from '../models';
import { HttpClient } from '@angular/common/http';
import { HttpHeaders } from '@angular/common/http';
import { StreamService } from '../services/stream.service';
import { Router, ActivatedRoute } from '@angular/router';
import { DataService } from '../services/data.service';
import { switchMap, filter, map } from 'rxjs/operators';
import { RecurringTransactionsComponent } from '../recurring-transactions/recurring-transactions.component';
import Routes from '../routes.json'

@Component({
  selector: 'app-set-transaction',
  templateUrl: './set-recurring-transaction.component.html',
  styleUrls: ['./set-recurring-transaction.component.scss']
})
export class SetRecurringTransactionComponent implements OnInit {
  addTransactionForm;
  checkoutForm;
  action = 'add';
  currentTransaction: RecurringTransactionModel;
  routes: typeof Routes;

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private streamService: StreamService,
    private router: Router,
    private route: ActivatedRoute,
    private data: DataService) {

    this.routes = Routes;
    this.addTransactionForm = this.fb.group({
      id: undefined,
      endDate: '2040-01-01',
      description: 'dd',
      interval: this.fb.group({ interval: 1, intervalType: PaymentIntervalTypeModel.monthly}),
      // amounts: [{amount: 0, date: formatDate(new Date(), 'yyyy-MM-dd', 'en')}]
      amounts: this.fb.array([
        fb.group({amount: 0, date: formatDate(new Date(), 'yyyy-MM-dd', 'en')}),
        fb.group({amount: 10, date: formatDate(new Date(), 'yyyy-MM-dd', 'en')})
      ])
    });
  }

  ngOnInit(): void {
    this.route.paramMap
      .subscribe(paramMap => {
        if (!paramMap.has('action')) {
          return;
        }

        this.action = paramMap.get('action');

        if (this.action === 'add') {
          this.addTransactionForm = this.fb.group({
            id: undefined,
            endDate: '2040-01-01',
            description: 'dd',
            interval: this.fb.group({ interval: 1, intervalType: PaymentIntervalTypeModel.monthly}),
            // amounts: [{amount: 0, date: formatDate(new Date(), 'yyyy-MM-dd', 'en')}]
            amounts: this.fb.array([
              this.fb.group({amount: 0, date: formatDate(new Date(), 'yyyy-MM-dd', 'en')}),
            ])
          });
        }

        if (this.action === 'edit' && paramMap.has('id')) {
          const id = paramMap.get('id');
          this.data.recurringTransactions$.pipe(
            map(tList => tList.find(t => t.id === id),
            filter(t => t !== undefined)))
            .subscribe(t => {
              this.currentTransaction = t;
              this.addTransactionForm = this.fb.group({
                id: t.id,
                endDate: formatDate(t.endDate, 'yyyy-MM-dd', 'en'),
                description: t.description,
                interval: this.fb.group({ interval: t.interval.interval, intervalType: t.interval.intervalType}),
                amounts: this.fb.array(t.amounts.map(a => this.fb.group({amount: a.amount, date: formatDate(a.date, 'yyyy-MM-dd', 'en')})))
              });
            });
        }
      });
  }

  async addTransaction(transaction: RecurringTransactionModel) {
    transaction.interval.intervalType = parseInt(transaction.interval.intervalType as any);
    const connectionId = await this.streamService.connectionId;
    const url = 'transaction/recurring-transaction';
    const options = {headers: new HttpHeaders({ConnectionId: connectionId})};

    const response$ = this.action === 'add' ?
       this.http.post<OnetimeTransactionModel>(url, transaction, options) :
       this.http.put<OnetimeTransactionModel>(url, {...transaction, id: this.currentTransaction.id }, options);

    response$.subscribe(x => {
         console.log(x);
       });

    this.router.navigateByUrl(this.routes.RecurringTransactions);
  }

  addAmount() {
    this.addTransactionForm.get('amounts').push(this.fb.group({amount: 10, date: formatDate(new Date(), 'yyyy-MM-dd', 'en')}));
  }

  removeAmount(i: number) {
    const newAmounts = this.addTransactionForm.get('amounts').value.splice(i, 1);
    // this.addTransactionForm.amounts = this.fb.array([
    //   this.fb.group({amount: 0, date: formatDate(new Date(), 'yyyy-MM-dd', 'en')}),
    //   this.fb.group({amount: 10, date: formatDate(new Date(), 'yyyy-MM-dd', 'en')})
    // ]);
    this.addTransactionForm.get('amounts').removeAt(i);
  }
}
