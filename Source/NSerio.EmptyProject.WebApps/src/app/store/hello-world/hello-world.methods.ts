import { signalStoreFeature, type, withMethods } from '@ngrx/signals';
import { HelloWorldStateModel } from '../models/hello-world-state.model';
import withHelloWorldMutations from './hello-world.mutations';

export default function withHelloWorldMethods() {
  return signalStoreFeature(
    { state: type<HelloWorldStateModel>() },
    withHelloWorldMutations(),
    withMethods((store) => ({
      // Add methods here
    })),
  );
}
