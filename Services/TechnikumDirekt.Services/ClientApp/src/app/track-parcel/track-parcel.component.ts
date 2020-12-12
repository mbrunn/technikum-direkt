import {Component, OnInit} from '@angular/core';
import {FormControl, Validators} from '@angular/forms';
import {debounceTime, distinctUntilChanged} from 'rxjs/operators';
import {ActivatedRoute, Params, Router} from '@angular/router';
import {TdApiService} from '../services/td-api.service';
import {ParcelState, TrackingInfo} from '../../models/trackingInfo';
import {Hop} from '../../models/hop';

@Component({
  selector: 'app-track-parcel',
  templateUrl: './track-parcel.component.html',
  styleUrls: ['./track-parcel.component.scss']
})
export class TrackParcelComponent implements OnInit {
  private trackingIdParamKey = 'trackingId';

  public searchInput = new FormControl('', Validators.required);
  public trackingInfo: TrackingInfo | undefined;
  public isLoading = false;
  public responseStatus: number | undefined;

  public parcelStates = ParcelState;
  public hops: Hop[] = [];

  constructor(private router: Router, private activatedRoute: ActivatedRoute, private tdApiService: TdApiService) { }

  ngOnInit(): void {
    this.searchInput.valueChanges.pipe(debounceTime(250), distinctUntilChanged())
      .subscribe(query => {
        this.updateSearchQuery(query);
      });

    this.activatedRoute.queryParams.subscribe(params => {
      if (params[this.trackingIdParamKey]) {
        this.search(params[this.trackingIdParamKey]);
        this.searchInput.patchValue(params[this.trackingIdParamKey]);
      }
    });
  }

  updateSearchQuery(query: string): void {
    const queryParams: Params = { trackingId: query };

    this.router.navigate([], {
      relativeTo: this.activatedRoute,
      queryParams
    });
  }

  search(trackingId: string): void {
    this.isLoading = true;
    this.tdApiService.trackParcel(trackingId).subscribe(info => {
      this.trackingInfo = info;
      info.visitedHops.forEach(hop => {
        this.hops.push(hop);
      });
      info.futureHops.forEach(hop => {
        this.hops.push(hop);
      });
      console.log(this.hops);
      this.responseStatus = 200;
    }, error => {
      this.responseStatus = error.status;
      this.isLoading = false;
    }, () => {
      this.isLoading = false;
    });
  }
}
