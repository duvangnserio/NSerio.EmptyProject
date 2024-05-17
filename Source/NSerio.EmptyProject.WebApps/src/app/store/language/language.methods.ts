import { inject } from '@angular/core';
import { LanguageModel } from '@app/models/language.model';
import { signalStoreFeature, type, withMethods } from '@ngrx/signals';
import { TranslateService } from '@ngx-translate/core';
import { LanguageStateModel } from '../models/language-state.model';
import withLanguageMutations from './language.mutations';

export default function withLanguageMethods() {
  return signalStoreFeature(
    { state: type<LanguageStateModel>() },
    withLanguageMutations(),
    withMethods((store) => {
      const translateService = inject(TranslateService);

      return {
        setDefaultLanguage() {
          const browserLangCode = translateService.getBrowserLang();
          const browserLangInLanguagesList = store
            .languages()
            .find((lang) => lang.code === browserLangCode);

          const defaultLanguageCode =
            browserLangInLanguagesList ?? store.selectedLanguage();
          translateService.setDefaultLang(defaultLanguageCode.code);
          this.changeAppLanguage(defaultLanguageCode!);
        },

        changeAppLanguage(selectedLanguage: LanguageModel) {
          translateService.use(selectedLanguage.code);
          store.setLanguage(selectedLanguage);
        },
      };
    }),
  );
}
