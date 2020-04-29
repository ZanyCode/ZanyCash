import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LiquidityDashboardComponent } from './liquidity-dashboard.component';

describe('LiquidityDashboardComponent', () => {
  let component: LiquidityDashboardComponent;
  let fixture: ComponentFixture<LiquidityDashboardComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LiquidityDashboardComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LiquidityDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
