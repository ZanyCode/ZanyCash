import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SetRecurringTransactionComponent } from './set-recurring-transaction.component';

describe('AddTransactionComponent', () => {
  let component: SetRecurringTransactionComponent;
  let fixture: ComponentFixture<SetRecurringTransactionComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SetRecurringTransactionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SetRecurringTransactionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
