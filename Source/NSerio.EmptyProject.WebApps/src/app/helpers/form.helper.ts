import { ValidationErrors } from '@angular/forms';

/**
 * Retrieves the error message for a form control.
 * @param field - The name of the form control.
 * @param errors - The validation errors object.
 * @returns The error message for the form control.
 */
export const getFormControlError = (
  field: string,
  errors: ValidationErrors,
): string => {
  let errorMessage = '';
  const key = Object.keys(errors)[0];
  const requiredLength = errors[key]?.requiredLength;

  switch (key) {
    case 'required':
      errorMessage = `${field} is required`;
      break;
    case 'minlength':
      errorMessage = `${field} must be at least ${requiredLength} characters`;
      break;
    case 'maxlength':
      errorMessage = `${field} must be at most ${requiredLength} characters`;
      break;
    // Add more cases as needed
    default:
      errorMessage = 'Invalid value';
      break;
  }
  return errorMessage;
};
