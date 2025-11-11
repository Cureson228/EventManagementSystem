import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class EventService {

  constructor(private http: HttpClient) {
    
  }

  createEvent(eventData: any): Observable<any>{
    return this.http.post(environment.apiBaseUrl + '/events/create', eventData);
  }

  getAllTags() : Observable<any>{
    return this.http.get(environment.apiBaseUrl + '/events/tags');
  }

  getPublicEvents(tags? : string[] | null) : Observable<any> {
    let params = new HttpParams();

    if (tags && tags.length > 0){
      tags.forEach(tag => {
        params = params.append('tags', tag);
      })
    }

    return this.http.get<Event[]>(environment.apiBaseUrl + '/events', {params});
  }

  getEventTags(id :number) : Observable<any>{
    return this.http.get<string[]>(environment.apiBaseUrl + '/events/tags/' + id);
  }
  getEventDetails(id : number) : Observable<any> {
    return this.http.get<Event>(environment.apiBaseUrl + '/events/' + id)
  }
  updateEvent(id : number, updatedEvent : any) : Observable<any> {
    return this.http.patch(environment.apiBaseUrl + '/events/' + id,updatedEvent);
  }
  getUserEvents() : Observable<any> {
    return this.http.get<Event[]>(environment.apiBaseUrl + '/users/me/events');
  }
  deleteEvent(id : number) : Observable<any> {
    return this.http.delete(environment.apiBaseUrl + '/events/' + id);
  }
  joinEvent(id : number) : Observable<any> {
    return this.http.post(environment.apiBaseUrl + '/events/' + id + '/join', {});
  }
  leaveEvent(id: number) : Observable<any> {
    return this.http.post(environment.apiBaseUrl + '/events/' + id + '/leave', {});
  }
}
