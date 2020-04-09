import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RecurringTransactionsComponent } from './recurring-transactions.component';

describe('TransactionsComponent', () => {
  let component: RecurringTransactionsComponent;
  let fixture: ComponentFixture<RecurringTransactionsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RecurringTransactionsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RecurringTransactionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
