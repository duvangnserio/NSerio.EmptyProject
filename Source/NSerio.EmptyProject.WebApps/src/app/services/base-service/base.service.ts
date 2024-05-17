import {
  HttpClient,
  HttpHeaders,
  HttpParams,
  HttpResponse,
} from '@angular/common/http';
import { inject } from '@angular/core';
import { Observable, lastValueFrom, map } from 'rxjs';

/**
 * Base service class for making HTTP requests.
 */
export class BaseService {
  //#region [Injects]

  private readonly httpClient = inject(HttpClient);

  //#endregion [Injects]

  //#region [Get Functions]

  /**
   * Sends an HTTP GET request and returns the response as a Promise.
   * @param url - The URL to send the request to.
   * @param options - The options for the request (observe, params, headers).
   * @returns A Promise that resolves to the response body.
   */
  protected getAsync<T>(
    url: string,
    options?: { observe?: 'body'; params?: HttpParams; headers?: HttpHeaders }
  ): Promise<T>;

  /**
   * Sends an HTTP GET request and returns the full response as a Promise.
   * @param url - The URL to send the request to.
   * @param options - The options for the request (observe, params, headers).
   * @returns A Promise that resolves to the full response or the response body.
   */
  protected getAsync<T>(
    url: string,
    options?: {
      observe?: 'response';
      params?: HttpParams;
      headers?: HttpHeaders;
    }
  ): Promise<HttpResponse<T>>;

  /**
   * Sends an HTTP GET request and returns the response as a Promise.
   * @param url - The URL to send the request to.
   * @param options - The options for the request (observe, params, headers).
   * @returns A Promise that resolves to the full response or the response body.
   */
  protected getAsync<T>(
    url: string,
    options?: {
      observe?: 'response' | 'body';
      params?: HttpParams;
      headers?: HttpHeaders;
    }
  ): Promise<HttpResponse<T> | T> {
    let response$: Observable<HttpResponse<T> | T>;

    switch (options?.observe) {
      case 'response':
        response$ = this.httpClient
          .get<T>(url, {
            params: options?.params,
            headers: options?.headers,
            observe: 'response',
          })
          .pipe(map((result: HttpResponse<T>) => result));
        break;
      default:
        response$ = this.httpClient
          .get<T>(url, { params: options?.params, headers: options?.headers })
          .pipe(map((result: T) => result));
        break;
    }
    return lastValueFrom(response$);
  }

  //#endregion [Get Functions]

  //#region [Post Functions]

  /**
   * Sends an HTTP POST request and returns the response as a Promise.
   * @param url - The URL to send the request to.
   * @param options - The options for the request (observe, params, headers).
   * @returns A Promise that resolves to the response body.
   */
  protected postAsync<T>(
    url: string,
    options?: { observe?: 'body'; params?: HttpParams; headers?: HttpHeaders }
  ): Promise<T>;

  /**
   * Sends an HTTP POST request and returns the full response as a Promise.
   * @param url - The URL to send the request to.
   * @param options - The options for the request (observe, params, headers).
   * @returns A Promise that resolves to the full response or the response body.
   */
  protected postAsync<T>(
    url: string,
    options?: {
      observe?: 'response';
      params?: HttpParams;
      headers?: HttpHeaders;
    }
  ): Promise<HttpResponse<T>>;

  /**
   * Sends an HTTP POST request and returns the response as a Promise.
   * @param url - The URL to send the request to.
   * @param body - The request body.
   * @param options - The options for the request (observe, params, headers).
   * @returns A Promise that resolves to the full response or the response body.
   */
  protected postAsync<T, V>(
    url: string,
    body?: V,
    options?: { observe?: 'body'; params?: HttpParams; headers?: HttpHeaders }
  ): Promise<T>;

  /**
   * Sends an HTTP POST request and returns the full response as a Promise.
   * @param url - The URL to send the request to.
   * @param body - The request body.
   * @param options - The options for the request (observe, params, headers).
   * @returns A Promise that resolves to the full response or the response body.
   */
  protected postAsync<T, V>(
    url: string,
    body?: V,
    options?: {
      observe?: 'response';
      params?: HttpParams;
      headers?: HttpHeaders;
    }
  ): Promise<HttpResponse<T>>;

  /**
   * Sends an HTTP POST request and returns the response as a Promise.
   * @param url - The URL to send the request to.
   * @param body - The request body.
   * @param options - The options for the request (observe, params, headers).
   * @returns A Promise that resolves to the full response or the response body.
   */
  protected postAsync<T, V>(
    url: string,
    body?: V,
    options?: {
      observe?: 'response' | 'body';
      params?: HttpParams;
      headers?: HttpHeaders;
    }
  ): Promise<HttpResponse<T> | T> {
    let response$: Observable<HttpResponse<T> | T>;

    switch (options?.observe) {
      case 'response':
        response$ = this.httpClient
          .post<T>(url, body, {
            params: options?.params,
            headers: options?.headers,
            observe: 'response',
          })
          .pipe(map((result: HttpResponse<T>) => result));
        break;
      default:
        response$ = this.httpClient
          .post<T>(url, body, {
            params: options?.params,
            headers: options?.headers,
          })
          .pipe(map((result: T) => result));
        break;
    }
    return lastValueFrom(response$);
  }

  //#endregion [Post Functions]

  //#region [Put Functions]

  /**
   * Sends an HTTP PUT request and returns the response as a Promise.
   * @param url - The URL to send the request to.
   * @param body - The request body.
   * @param options - The options for the request (observe, params, headers).
   * @returns A Promise that resolves to the response body.
   */
  protected putAsync<T, V>(
    url: string,
    body: V,
    options?: { observe?: 'body'; params?: HttpParams; headers?: HttpHeaders }
  ): Promise<T>;

  /**
   * Sends an HTTP PUT request and returns the full response as a Promise.
   * @param url - The URL to send the request to.
   * @param body - The request body.
   * @param options - The options for the request (observe, params, headers).
   * @returns A Promise that resolves to the full response or the response body.
   */
  protected putAsync<T, V>(
    url: string,
    body: V,
    options?: {
      observe?: 'response';
      params?: HttpParams;
      headers?: HttpHeaders;
    }
  ): Promise<HttpResponse<T>>;

  /**
   * Sends an HTTP PUT request and returns the response as a Promise.
   * @param url - The URL to send the request to.
   * @param body - The request body.
   * @param options - The options for the request (observe, params, headers).
   * @returns A Promise that resolves to the full response or the response body.
   */
  protected putAsync<T, V>(
    url: string,
    body: V,
    options?: {
      observe?: 'response' | 'body';
      params?: HttpParams;
      headers?: HttpHeaders;
    }
  ): Promise<HttpResponse<T> | T> {
    let response$: Observable<HttpResponse<T> | T>;

    switch (options?.observe) {
      case 'response':
        response$ = this.httpClient
          .put<T>(url, body, {
            params: options?.params,
            headers: options?.headers,
            observe: 'response',
          })
          .pipe(map((result: HttpResponse<T>) => result));
        break;
      default:
        response$ = this.httpClient
          .put<T>(url, body, {
            params: options?.params,
            headers: options?.headers,
          })
          .pipe(map((result: T) => result));
        break;
    }
    return lastValueFrom(response$);
  }

  //#endregion [Put Functions]

  //#region [Delete Functions]

  /**
   * Sends an HTTP DELETE request and returns the response as a Promise.
   * @param url - The URL to send the request to.
   * @param options - The options for the request (observe, params, headers).
   * @returns A Promise that resolves to the response body.
   */
  protected deleteAsync<T>(
    url: string,
    options?: { observe?: 'body'; params?: HttpParams; headers?: HttpHeaders }
  ): Promise<T>;

  /**
   * Sends an HTTP DELETE request and returns the full response as a Promise.
   * @param url - The URL to send the request to.
   * @param options - The options for the request (observe, params, headers).
   * @returns A Promise that resolves to the full response or the response body.
   */
  protected deleteAsync<T>(
    url: string,
    options?: {
      observe?: 'response';
      params?: HttpParams;
      headers?: HttpHeaders;
    }
  ): Promise<HttpResponse<T>>;

  /**
   * Sends an HTTP DELETE request and returns the response as a Promise.
   * @param url - The URL to send the request to.
   * @param options - The options for the request (observe, params, headers).
   * @returns A Promise that resolves to the full response or the response body.
   */
  protected deleteAsync<T>(
    url: string,
    options?: {
      observe?: 'response' | 'body';
      params?: HttpParams;
      headers?: HttpHeaders;
    }
  ): Promise<HttpResponse<T> | T> {
    let response$: Observable<HttpResponse<T> | T>;

    switch (options?.observe) {
      case 'response':
        response$ = this.httpClient
          .delete<T>(url, {
            params: options?.params,
            headers: options?.headers,
            observe: 'response',
          })
          .pipe(map((result: HttpResponse<T>) => result));
        break;
      default:
        response$ = this.httpClient
          .delete<T>(url, {
            params: options?.params,
            headers: options?.headers,
          })
          .pipe(map((result: T) => result));
        break;
    }
    return lastValueFrom(response$);
  }

  //#endregion [Delete Functions]
}
