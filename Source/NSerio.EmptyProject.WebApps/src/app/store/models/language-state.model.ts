import { LanguageModel } from '@app/models/language.model';

export type LanguageStateModel = {
  selectedLanguage: LanguageModel;
  languages: LanguageModel[];
};
