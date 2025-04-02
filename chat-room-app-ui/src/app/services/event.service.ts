import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DetailedChatEvent, AggregatedChatEvent, CreateEventRequest, Granularity } from '../types/event.types';

@Injectable({
  providedIn: 'root'
})
export class EventService {
  private apiUrl = 'http://localhost:5076';

  constructor(private http: HttpClient) { }

  getDetailedEvents(startDate: string | null, endDate: string | null): Observable<DetailedChatEvent[]> {
    let params = new HttpParams();
    if (startDate) params = params.set('startDate', startDate);
    if (endDate) params = params.set('endDate', endDate);
    
    return this.http.get<DetailedChatEvent[]>(`${this.apiUrl}/events/detailed`, { params });
  }

  getAggregatedEvents(startDate: string | null, endDate: string | null, granularity: Granularity): Observable<AggregatedChatEvent[]> {
    let params = new HttpParams().set('granularity', granularity);
    if (startDate) params = params.set('startDate', startDate);
    if (endDate) params = params.set('endDate', endDate);
    
    return this.http.get<AggregatedChatEvent[]>(`${this.apiUrl}/events/aggregated`, { params });
  }

  createEvent(event: CreateEventRequest): Observable<DetailedChatEvent> {
    return this.http.post<DetailedChatEvent>(`${this.apiUrl}/events`, event);
  }
} 