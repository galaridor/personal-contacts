import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { Contact, ContactPayload, ContactsPageRequest, PagedResult } from '../models';

@Injectable({ providedIn: 'root' })
export class ContactsApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/contacts`;

  getPage(request: ContactsPageRequest): Observable<PagedResult<Contact>> {
    let params = new HttpParams().set('page', request.page).set('pageSize', request.pageSize);

    if (request.search) {
      params = params.set('search', request.search);
    }

    return this.http.get<PagedResult<Contact>>(this.baseUrl, { params });
  }

  getById(id: string): Observable<Contact> {
    return this.http.get<Contact>(`${this.baseUrl}/${id}`);
  }

  create(payload: ContactPayload): Observable<Contact> {
    return this.http.post<Contact>(this.baseUrl, payload);
  }

  update(id: string, payload: ContactPayload): Observable<Contact> {
    return this.http.put<Contact>(`${this.baseUrl}/${id}`, payload);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
