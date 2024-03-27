import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AppService {

  constructor(private http: HttpClient) { }

  convertInput(input: string): Observable<any> {

    const headers = new HttpHeaders({
      'Content-Type': 'application/json'
    });
    const body = { input };

    return this.http.post<ConvertorResponse>('/api/convertor', body, { headers });
  }
}

export class ConvertorResponse {
  output: string;
  message: string;

  constructor(output: string, message: string) {
    this.output = output;
    this.message = message;
  }
}
