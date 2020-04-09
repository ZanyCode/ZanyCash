import { Component, Injectable } from '@angular/core';

export class Route
{
    constructor(values: {
        path: string;
        redirectTo?: string;
        pathMatch?: string;
        display?: boolean;
        label?: string;
        component?: any;
    }){
        [this.path, this.redirectTo, this.pathMatch, this.display, this.label, this.component] =
        [values.path, values.redirectTo, values.pathMatch, values.display, values.label, values.component];
    }

    path: string;
    redirectTo?: string;
    pathMatch?: string;
    display?: boolean;
    label?: string;
    component?: any;

    get absPath()
    {
        return '/' + this.path;
    }

    get basePath()
    {
        let endIdx = this.path.indexOf(':') - 1;
        if (endIdx === -2) {
            endIdx = this.path.length - 1;
        }

        return '/' + this.path.substring(0, endIdx);
    }
}

@Injectable({
    providedIn: 'root'
  })
export class Routes
{
    Default: Route;
    OnetimeTransactions: Route;
    RecurringTransactions: Route;
    SetOnetimeTransaction: Route;
    SetRecurringTransaction: Route;
}
