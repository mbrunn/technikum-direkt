import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SubmitParcelComponent } from './submit-parcel.component';

describe('SubmitParcelComponent', () => {
  let component: SubmitParcelComponent;
  let fixture: ComponentFixture<SubmitParcelComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SubmitParcelComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SubmitParcelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
