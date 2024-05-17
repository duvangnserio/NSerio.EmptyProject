import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';

import { KEPLER_SERVICE_MODULE } from '@app/app.constants';
import pkg from '@pkg';

export const httpErrorInterceptor: HttpInterceptorFn = (req, next) =>
  next(req).pipe(catchError(handleError));

function handleError(response: HttpErrorResponse) {
  let defaultMessage: string;
  switch (response.constructor) {
    case ErrorEvent:
      console.error('An error occurred:', response.error.message);
      defaultMessage = response.error.message;
      break;
    // Add more cases as needed
    default:
      defaultMessage =
        response.error && response.error.Message
          ? response.error.Message
          : `[${pkg.name}].[${KEPLER_SERVICE_MODULE}]. There was an error. Please try again later.`;
      console.error(
        `Server returned code ${response.status}, ` +
          `Error: ${defaultMessage}`,
      );
      break;
  }
  return throwError(() => defaultMessage);
}
