import { HttpHeaders, HttpInterceptorFn } from '@angular/common/http';
import { KEPLER_SERVICE_MODULE } from '@app/app.constants';
import { environment } from '@env/environment';

export const httpRequestInterceptor: HttpInterceptorFn = (req, next) => {
  const baseUrl = `${environment.serviceHost || ''}/Relativity.REST/api/${KEPLER_SERVICE_MODULE}/`;
  const url = req.url.includes('assets') ? req.url : `${baseUrl}${req.url}`;

  const headers: any = {
    'Content-Type': 'application/json',
    'X-CSRF-Header': '-',
  };

  if (environment?.authHeader) {
    headers['Authorization'] = environment?.authHeader;
  }

  const newRequest = req.clone({ headers: new HttpHeaders(headers), url });

  return next(newRequest);
};
