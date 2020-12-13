import {Hop} from './hop';

export class TrackingInfo {
  state = ParcelState.Pickup;
  visitedHops: Hop[] = [];
  futureHops: Hop[] = [];
}

export enum ParcelState {
  Pickup = 'Pickup',
  InTransport = 'InTransport',
  InTruckDelivery = 'InTruckDelivery',
  Transferred = 'Transferred',
  Delivered = 'Delivered'
}
