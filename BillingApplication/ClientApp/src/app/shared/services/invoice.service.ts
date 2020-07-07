import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { Invoice } from '../interfaces/i-Invoice';
import { tap, map, catchError } from 'rxjs/operators';
import { IStats } from '../interfaces/i-stats';
import { IUpload } from '../interfaces/i-upload';

@Injectable({
  providedIn: 'root'
})
export class InvoiceService {

  constructor(private httpClient: HttpClient) { }

  //Get Invoice by Id
  GetInvoice(id: number): Observable<Invoice> {
    return this.httpClient.get<Invoice>(`/document/${id}`).pipe(
      tap(response => console.log(JSON.stringify(response))
      ), catchError(this.handleError)
    );
  }

  //Get statistics data
  GetStats(): Observable<IStats[]>{
    return this.httpClient.get<IStats[]>('/stats').pipe(
      tap(response => console.log(JSON.stringify(response))
      ), catchError(this.handleError)
    );
  }

  //Upload invoice
  PostInvoice(requestBody: IUpload): Observable<boolean>{
    return this.httpClient.post<boolean>('/upload', requestBody).pipe(
      map((response: any) => {return response;
      }), catchError(this.handleError)
    );
  }

  //Error Handler
  private handleError(err: HttpErrorResponse) {
    let errorMessage = '';
    if (err.error instanceof ErrorEvent) {
      errorMessage = `An error Occurred: ${err.error.message}`;
    }
    else {
      errorMessage = `Server returned code: ${err.status}, error message is: ${err.message}`;
    }
    console.error(errorMessage);
    return throwError(err);
  }
}
