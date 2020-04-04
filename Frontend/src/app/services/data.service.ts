import { Injectable } from '@angular/core';
import { StreamService, Error } from './stream.service';
import { TransactionModel, OnetimeTransactionModel, RecurringTransactionModel } from '../models';
import { Observable, Subject } from 'rxjs';
import { filter, map, tap, scan, shareReplay } from 'rxjs/operators';
import StreamNames from '../../../../Backend/ZanyCash/streamNames.json';

class ErrorUpdate
{
  isResolved: boolean;
  key: string;
  error: Error;
}

@Injectable({
  providedIn: 'root'
})
export class DataService {
  get onetimeTransactions$() {
    return this.getStream<OnetimeTransactionModel[]>(StreamNames.onetimeTransactions);
  }

  get recurringTransactions$() {
    return this.getStream<RecurringTransactionModel[]>(StreamNames.recurringTransactions);
  }

  public errorSubject: Subject<ErrorUpdate> = new Subject<ErrorUpdate>();
  public errors$: Observable<Map<string, Error>>;
  private streams: Map<string, Observable<any>> = new Map<string, Observable<any>>();


  private getStream<T>(name: string): Observable<T> {
    if (!this.streams.get(name)) {
      const stream$ =
        this.streamService.getStream<any>(name).pipe(
              tap(x => {
                const errorUpdate = {isResolved: !x.hasError, key: name, error: x.error};
                this.errorSubject.next(errorUpdate);
              }),
              map(x => x.value),
              shareReplay({refCount: false, bufferSize: 1}));

      this.streams.set(name, stream$);
    }

    // Return raw value without errors
    return this.streams.get(name);
  }

  constructor(private streamService: StreamService) {
    this.errors$ = this.errorSubject.pipe(
      scan((acc, val) => {
        if (val.isResolved && acc.get(val.key)) {
          acc.delete(val.key);
        }
        else if (!val.isResolved) {
          acc.set(val.key, val.error);
        }

        return acc;
      }, new Map<string, Error>()));
  }
}
