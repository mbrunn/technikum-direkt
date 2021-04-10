import { Injectable } from '@angular/core';
import {Parcel} from '../../models/parcel';
import {Observable} from 'rxjs';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {SubmitParcelResponse} from '../../models/submitParcelResponse';
import {TrackingInfo} from '../../models/trackingInfo';
import {TransferWarehouse} from '../../models/transferwarehouse';

@Injectable({
  providedIn: 'root'
})
export class TdApiService {

  constructor(private http: HttpClient) { }

  public submitParcel(parcel: Parcel): Observable<SubmitParcelResponse> {
    return this.http.post<SubmitParcelResponse>('/api/parcel', parcel);
  }

  public trackParcel(trackingId: string): Observable<TrackingInfo> {
    return this.http.get<TrackingInfo>(`/api/parcel/${trackingId}`);
  }

  public reportDelivery(trackingId: string): Observable<any> {
    return this.http.post<any>(`/api/parcel/${trackingId}/reportDelivery`, {}, { responseType: 'text' as 'json' });
  }

  public reportHop(trackingId: string, code: string): Observable<any> {
    return this.http.post<any>(`/api/parcel/${trackingId}/reportHop/${code}`, {}, { responseType: 'text' as 'json' });
  }

  public getTransferWarehouses(): Observable<TransferWarehouse[]> {
    return this.http.get<TransferWarehouse[]>('/api/warehouse/getTransferWarehouses');
  }
}
