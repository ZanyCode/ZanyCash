import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OnetimeTransactionsComponent } from './onetime-transactions.component';

describe('TransactionsComponent', () => {
  let component: OnetimeTransactionsComponent;
  let fixture: ComponentFixture<OnetimeTransactionsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OnetimeTransactionsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OnetimeTransactionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
