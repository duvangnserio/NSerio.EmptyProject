import { signalStoreFeature, type, withMethods } from '@ngrx/signals';
import { HelloWorldStateModel } from '../models/hello-world-state.model';

export default function withHelloWorldMutations() {
  return signalStoreFeature(
    { state: type<HelloWorldStateModel>() },
    withMethods((store) => ({
      // Add mutations here
    })),
  );
}
