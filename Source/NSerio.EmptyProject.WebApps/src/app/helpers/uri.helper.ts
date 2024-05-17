/**
 * Retrieves the value of a URL parameter.
 * @param paramName - The name of the parameter.
 * @returns The value of the parameter if found, null otherwise.
 */
export const getUrlParameter = (paramName: string) => {
  paramName = (paramName + '').toLowerCase().replace(/[[\]]/g, '\\$&');

  let url = window.location.href.toLowerCase(),
    regex = new RegExp('[?&]' + paramName + '(=([^&#]*)|&|#|$)'),
    results = regex.exec(url);

  let paramResult: string = !results ? '' : results[2] ? results[2] : '';

  return decodeURIComponent(paramResult.replace(/\+/g, ' '));
};
