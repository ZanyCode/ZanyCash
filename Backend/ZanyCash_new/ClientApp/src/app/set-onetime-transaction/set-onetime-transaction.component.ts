import { Component, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import {formatDate} from '@angular/common';
import { OnetimeTransactionModel } from '../models';
import { HttpClient } from '@angular/common/http';
import { HttpHeaders } from '@angular/common/http';
import { StreamService } from '../services/stream.service';
import { Router, ActivatedRoute } from '@angular/router';
import { DataService } from '../services/data.service';
import { switchMap, filter, map } from 'rxjs/operators';
import Routes from '../routes.json';


@Component({
  selector: 'app-set-transaction',
  templateUrl: './set-onetime-transaction.component.html',
  styleUrls: ['./set-onetime-transaction.component.scss']
})
export class SetOnetimeTransactionComponent implements OnInit {
  addTransactionForm;
  checkoutForm;
  action = 'invalid';
  currentTransaction: OnetimeTransactionModel;
  routes: typeof Routes;

  constructor(
    private formBuilder: FormBuilder,
    private http: HttpClient,
    private streamService: StreamService,
    private router: Router,
    private route: ActivatedRoute,
    private data: DataService) {

    this.checkoutForm = this.formBuilder.group({
      name: '',
      address: ''
    });

    this.addTransactionForm = this.formBuilder.group({
        date: formatDate(new Date(), 'yyyy-MM-dd', 'en'),
        description: '',
        amount: 3.50
      });

    this.routes = Routes;
  }

  ngOnInit(): void {
    this.route.paramMap
      .subscribe(paramMap => {
        if (!paramMap.has('action')) {
          return;
        }

        this.action = paramMap.get('action');

        if (this.action === 'add') {
          this.addTransactionForm = this.formBuilder.group({
            date: formatDate(new Date(), 'yyyy-MM-dd', 'en'),
            description: '',
            amount: 3.50
          });
        }

        if (this.action === 'edit' && paramMap.has('id')) {
          const id = paramMap.get('id');
          this.data.onetimeTransactions$.pipe(
            map(tList => tList.find(t => t.id === id),
            filter(t => t !== undefined)))
            .subscribe(t => {
              this.currentTransaction = t;
              this.addTransactionForm = this.formBuilder.group({
                date: formatDate(t.date, 'yyyy-MM-dd', 'en'),
                description: t.description,
                amount: t.amount
              });
            });
        }
      });
  }

  async addTransaction(transaction) {
    const connectionId = await this.streamService.connectionId;
    const url = 'transaction/onetime-transaction';
    const options = {headers: new HttpHeaders({ConnectionId: connectionId})};

    const response$ = this.action === 'add' ?
       this.http.post<OnetimeTransactionModel>(url, transaction, options) :
       this.http.put<OnetimeTransactionModel>(url, {...transaction, id: this.currentTransaction.id }, options);

    response$.subscribe(x => {
         console.log(x);
       });

    this.router.navigateByUrl(this.routes.OnetimeTransactions);
  }
}
