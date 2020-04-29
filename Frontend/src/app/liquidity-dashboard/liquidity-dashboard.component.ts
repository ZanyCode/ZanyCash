import { Component, OnInit, ViewChild } from '@angular/core';
import { DataService } from '../services/data.service';
import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { formatDate } from '@angular/common';
import { StreamService } from '../services/stream.service';
import { fromEvent, Observable, combineLatest } from 'rxjs';
import { map } from 'rxjs/operators';
import { DayLiquidityModel, OnetimeTransactionModel } from '../models';
declare var Plotly: any;

@Component({
  selector: 'app-liquidity-dashboard',
  templateUrl: './liquidity-dashboard.component.html',
  styleUrls: ['./liquidity-dashboard.component.scss']
})
export class LiquidityDashboardComponent implements OnInit {

  constructor(public data: DataService, private http: HttpClient, private streamService: StreamService) { }

  @ViewChild('liquidityPlot', { static: true })
  plot: any;

  startDate: Date = new Date('2000-01-01');
  endDate: Date = new Date('2030-01-01');

  transactions$: Observable<OnetimeTransactionModel[]>;

  private chartLayout = {
    margin: {t: 0, b: 0, l: 0, r: 0, pad: 0, autoexpand: true},
    legend: {xanchor: 'auto', x: 1, y: 1},
    yaxis: {
      title: 'Liquidity',
      autorange: true,
      showticklabels: true,
      automargin: true
    },
    xaxis: {
      title: 'Date',
      type: 'date',
      autorange: true,
      showticklabels: true,
      automargin: true
    },
  };

  ngOnInit(): void {

    Plotly.newPlot(this.plot.nativeElement, {}, this.chartLayout, {displayModeBar: false});

    const selectedDate$ = new Observable<Date>(observer => {
      this.plot.nativeElement.on('plotly_click', data => {
        const selectedDate = data.points[data.points.length - 1].x;
        observer.next(selectedDate);
      });
    });

    this.transactions$ = combineLatest([this.data.liquidity$, selectedDate$]).pipe(
      map(([liquidities, date]) => {
        return liquidities.find(l => new Date(l.date).getTime() === new Date(date + 'T00:00:00').getTime()).transactions;
      })
    );

    this.data.liquidity$.subscribe(dayLiquidities => {
      const data = [{
        x: dayLiquidities.map(l => new Date(l.date)),
        y: dayLiquidities.map(l => l.dailyMinimum),
        type: 'scattergl',
        name: 'Daily Minimum',
        mode: 'lines+markers'
      }];
      Plotly.react(this.plot.nativeElement, data, this.chartLayout, {displayModeBar: false})
    });
  }

  startDateChanged(e) {
    this.startDate = e.target.value;
    this.updateLiquidityDates();
  }

  endDateChanged(e) {
    this.endDate = e.target.value;
    this.updateLiquidityDates();
  }

  updateLiquidityDates() {
    this.streamService.connectionId.then(connectionId => {
        const params =
          new HttpParams()
          .append('startDate', formatDate(this.startDate, 'yyyy-MM-dd', 'en'))
          .append('endDate', formatDate(this.endDate, 'yyyy-MM-dd', 'en'));

        this.http.put<any>(
          'transaction/liquidity/date-range',
          {},
          {headers: new HttpHeaders({ConnectionId: connectionId}), params})
          .subscribe(x => console.log(x));
    });

    // const params = new HttpParams();
    // params.append('startDate', formatDate(this.startDate, 'yyyy-MM-dd', 'en'));
    // params.append('endDate', formatDate(this.endDate, 'yyyy-MM-dd', 'en'));

    // this.http.put<any>('liquidity/date-range', {}, {params}).subscribe(x => console.log(x));
  }
}
