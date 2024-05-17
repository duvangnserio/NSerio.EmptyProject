import { signalStoreFeature, type, withComputed } from '@ngrx/signals';
import { LanguageStateModel } from '../models/language-state.model';

export default function withLanguageSelectors() {
  return signalStoreFeature(
    { state: type<LanguageStateModel>() },
    withComputed((store) => ({
      // Add computed here
    })),
  );
}
