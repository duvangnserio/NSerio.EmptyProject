import {
  HttpClient,
  provideHttpClient,
  withInterceptors,
} from '@angular/common/http';
import { ApplicationConfig, importProvidersFrom } from '@angular/core';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideRouter, withHashLocation } from '@angular/router';

import { TranslateLoader, TranslateModule } from '@ngx-translate/core';

import { routes } from './app.routes';
import { TranslateHttpLoaderFactory } from './factories/translate-http-loader.factory';
import { httpErrorInterceptor } from './interceptors/http-error.interceptor';
import { httpRequestInterceptor } from './interceptors/http-request.interceptor';
import { MessageService } from 'primeng/api';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes, withHashLocation()),
    provideAnimationsAsync(),
    provideHttpClient(
      withInterceptors([httpRequestInterceptor, httpErrorInterceptor]),
    ),
    importProvidersFrom(
      TranslateModule.forRoot({
        loader: {
          provide: TranslateLoader,
          useFactory: TranslateHttpLoaderFactory,
          deps: [HttpClient],
        },
      }),
    ),
   // MessageService, provideAnimationsAsync()
  ],
};
