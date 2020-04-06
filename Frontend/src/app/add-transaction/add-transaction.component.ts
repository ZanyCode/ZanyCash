import { Component, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import {formatDate} from '@angular/common';
import { OnetimeTransactionModel } from '../models';
import { HttpClient } from '@angular/common/http';
import { HttpHeaders } from '@angular/common/http';
import { StreamService } from '../services/stream.service';
import { Router } from '@angular/router';


@Component({
  selector: 'app-add-transaction',
  templateUrl: './add-transaction.component.html',
  styleUrls: ['./add-transaction.component.scss']
})
export class AddTransactionComponent implements OnInit {
  addTransactionForm;
  checkoutForm;

  constructor(private formBuilder: FormBuilder, private http: HttpClient, private streamService: StreamService, private router: Router) {
    this.addTransactionForm = this.formBuilder.group({
      date: formatDate(new Date(), 'yyyy-MM-dd', 'en'),
      description: '',
      amount: 0
    });

    this.checkoutForm = this.formBuilder.group({
      name: '',
      address: ''
    });
  }

  ngOnInit(): void {
  }

  async addTransaction(transaction) {
    const connectionId = await this.streamService.connectionId;
    this.http.post<OnetimeTransactionModel>(
      'transaction/onetime-transaction',
       transaction,
       {headers: new HttpHeaders({ConnectionId: '123'})}).subscribe(x => {
         console.log(x);
       });
    this.router.navigateByUrl('/transactions');
  }
}
