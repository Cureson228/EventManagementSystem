import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class AiAssistanceService {

  constructor(private http : HttpClient){

  }

  getAnswer(question : string) : Observable<any>{
    console.log(question);
    
    return this.http.post(environment.apiBaseUrl + '/assistance/ask', {question});
  }
}
