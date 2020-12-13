import { Component, OnInit } from '@angular/core';
import {FormControl, FormGroup, Validators} from '@angular/forms';
import {Recipient} from '../../models/recipient';
import {Parcel} from '../../models/parcel';
import {TdApiService} from '../services/td-api.service';

@Component({
  selector: 'app-submit-parcel',
  templateUrl: './submit-parcel.component.html',
  styleUrls: ['./submit-parcel.component.scss']
})
export class SubmitParcelComponent implements OnInit {

  public submitParcelForm = new FormGroup({
    senderName: new FormControl('', Validators.required),
    senderStreet: new FormControl('', Validators.required),
    senderZip: new FormControl('', Validators.required),
    senderCity: new FormControl('', Validators.required),
    senderCountry: new FormControl('', Validators.required),

    recipientName: new FormControl('', Validators.required),
    recipientStreet: new FormControl('', Validators.required),
    recipientZip: new FormControl('', Validators.required),
    recipientCity: new FormControl('', Validators.required),
    recipientCountry: new FormControl('', Validators.required),

    weight: new FormControl('', [Validators.required, Validators.min(0)])
  });

  public trackingId: string | undefined;
  public isLoading = false;

  constructor(private tdApiService: TdApiService) { }

  ngOnInit(): void {
  }

  public submitParcel(): void {
    if (!this.submitParcelForm.valid) {
      return;
    }

    const sender = new Recipient();
    sender.name = this.submitParcelForm.get('senderName')?.value;
    sender.street = this.submitParcelForm.get('senderStreet')?.value;
    sender.postalCode = this.submitParcelForm.get('senderZip')?.value;
    sender.city = this.submitParcelForm.get('senderCity')?.value;
    sender.country = this.submitParcelForm.get('senderCountry')?.value;

    const recipient = new Recipient();
    recipient.name = this.submitParcelForm.get('recipientName')?.value;
    recipient.street = this.submitParcelForm.get('recipientStreet')?.value;
    recipient.postalCode = this.submitParcelForm.get('recipientZip')?.value;
    recipient.city = this.submitParcelForm.get('recipientCity')?.value;
    recipient.country = this.submitParcelForm.get('recipientCountry')?.value;

    const parcel = new Parcel();
    parcel.sender = sender;
    parcel.recipient = recipient;
    parcel.weight = this.submitParcelForm.get('weight')?.value;

    this.submitParcelForm.disable();
    this.isLoading = true;
    this.tdApiService.submitParcel(parcel).subscribe(response => {
      this.trackingId = response.trackingId;
    }, error => {
      console.error(error);
    }, () => {
      this.isLoading = false;
    });
  }

  public resetForm(): void {
    this.submitParcelForm.reset();
    this.submitParcelForm.enable();
    this.trackingId = undefined;
  }
}
