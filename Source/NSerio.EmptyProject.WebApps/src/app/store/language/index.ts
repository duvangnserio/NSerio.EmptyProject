import { withDevtools } from '@angular-architects/ngrx-toolkit';
import { signalStore, withState } from '@ngrx/signals';
import withLanguageMethods from './language.methods';
import withLanguageSelectors from './language.selectors';
import languageState from './language.state';

const LanguageStore = signalStore(
  { providedIn: 'root' },
  withDevtools('Language Store'),
  withState(languageState),
  withLanguageMethods(),
  withLanguageSelectors()
);

export default LanguageStore;
