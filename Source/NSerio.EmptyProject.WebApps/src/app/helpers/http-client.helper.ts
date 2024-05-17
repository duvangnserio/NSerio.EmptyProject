import { HttpErrorResponse } from '@angular/common/http';
import { KEPLER_SERVICE_MODULE } from '@app/app.constants';
import pkg from '@pkg';
import { throwError } from 'rxjs';

/**
 * Handles an HTTP error response.
 * @param response - The HTTP error response.
 * @returns An observable that emits the error message.
 */
export const handleError = (response: HttpErrorResponse) => {
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
        `Server returned code ${response.status}, Error: ${defaultMessage}`,
      );
      break;
  }
  return throwError(() => defaultMessage);
};
