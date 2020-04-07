import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SetTransactionComponent } from './set-transaction.component';

describe('AddTransactionComponent', () => {
  let component: SetTransactionComponent;
  let fixture: ComponentFixture<SetTransactionComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SetTransactionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SetTransactionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
