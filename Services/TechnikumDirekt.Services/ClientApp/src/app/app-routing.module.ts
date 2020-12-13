import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {SubmitParcelComponent} from './submit-parcel/submit-parcel.component';
import {TrackParcelComponent} from './track-parcel/track-parcel.component';
import {ReportHopComponent} from './report-hop/report-hop.component';
import {HomeComponent} from './home/home.component';

const routes: Routes = [
  {
    path: '',
    component: HomeComponent
  },
  {
    path: 'submit',
    component: SubmitParcelComponent
  },
  {
    path: 'track',
    component: TrackParcelComponent
  },
  {
    path: 'report-hop',
    component: ReportHopComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
