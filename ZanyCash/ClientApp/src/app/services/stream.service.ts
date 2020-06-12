import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { publishReplay, shareReplay } from 'rxjs/operators';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';

export class Error {
  message: string;
  code: number;
}

export class Result<T> {
  hasError: boolean;
  error: Error;
  value: T;
}

@Injectable({
  providedIn: 'root'
})
export class StreamService {
  private connection: Promise<HubConnection>;
  private streams: Map<string, Observable<any>> = new Map<string, Observable<any>>();
  public connectionId: Promise<string>;

  private async getConnection() {
    if (!this.connection) {
      this.connection = new Promise(async res => {
        const connection = new HubConnectionBuilder()
        .withUrl('/streams')
        .build();

        await connection.start();
        this.connectionId = connection.invoke('GetConnectionId');
        res(connection);
      });
    }

    return this.connection;
  }

  public getStream<T>(name: string): Observable<Result<T>> {
    if (!this.streams.get(name)) {
      const stream$ = new Observable<any>(observer => {
        this.getConnection().then(connection => {
          connection.on(name, value => {
            observer.next(value);
          });

          connection.send('ConnectToStream', name).catch(e => observer.next(e));
        });
      });

      this.streams.set(name, stream$);
    }

    return this.streams.get(name);
  }
}