import { Component, OnInit } from '@angular/core';
import {FormControl, Validators} from '@angular/forms';
import {Params, Router} from '@angular/router';
import {HttpParams} from '@angular/common/http';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

  public searchControl = new FormControl('', Validators.required);

  constructor(private router: Router) { }

  ngOnInit(): void {
  }

  search(): void {
    if (this.searchControl.value) {
      const queryParams: Params = { trackingId: this.searchControl.value};
      this.router.navigate(['/track'], {queryParams, queryParamsHandling: 'merge'});
    }
  }
}
