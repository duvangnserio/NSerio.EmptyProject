import { signalStoreFeature, type, withComputed } from '@ngrx/signals';
import { HelloWorldStateModel } from '../models/hello-world-state.model';

export default function withHelloWorldSelectors() {
  return signalStoreFeature(
    { state: type<HelloWorldStateModel>() },
    withComputed((store) => ({
      // Add computed here
    })),
  );
}
