import { set } from '@app/helpers/mutations.helper';
import { LanguageModel } from '@app/models/language.model';
import { signalStoreFeature, type, withMethods } from '@ngrx/signals';
import { LanguageStateModel } from '../models/language-state.model';

export default function withLanguageMutations() {
  return signalStoreFeature(
    { state: type<LanguageStateModel>() },
    withMethods((store) => ({
      setLanguage: (value: LanguageModel) =>
        set('selectedLanguage')(store, value),
    })),
  );
}
