import {Recipient} from './recipient';

export class Parcel {
  sender = new Recipient();
  recipient = new Recipient();
  weight = 0.0;
}
