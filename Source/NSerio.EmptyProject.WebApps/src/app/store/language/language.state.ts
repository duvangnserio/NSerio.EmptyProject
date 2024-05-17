import { LanguageStateModel } from '../models/language-state.model';

const languageState: LanguageStateModel = {
  languages: [
    { name: 'English', code: 'en', flag: 'flag flag-us' },
    { name: 'Español', code: 'es', flag: 'flag flag-es' },
  ],
  selectedLanguage: { name: 'English', code: 'en', flag: 'flag flag-us' },
};

export default languageState;
