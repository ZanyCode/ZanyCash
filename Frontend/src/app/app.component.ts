import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Routes } from './routes';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  navLinks: any[];

  constructor(private router: Router, private routes: Routes) {
    this.navLinks = Object.values(routes).filter(r => r.display);
  }
}
