import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReportHopComponent } from './report-hop.component';

describe('ReportHopComponent', () => {
  let component: ReportHopComponent;
  let fixture: ComponentFixture<ReportHopComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ReportHopComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ReportHopComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
