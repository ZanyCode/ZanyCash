import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SetOnetimeTransactionComponent } from './set-recurring-transaction.component';

describe('AddTransactionComponent', () => {
  let component: SetOnetimeTransactionComponent;
  let fixture: ComponentFixture<SetOnetimeTransactionComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SetOnetimeTransactionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SetOnetimeTransactionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
