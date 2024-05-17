import { computed } from '@angular/core';
import { mapByKey } from '@app/helpers/computed.helper';
import { ExampleModel } from '@app/models/example.model';
import { signalStoreFeature, type, withComputed } from '@ngrx/signals';
import { ExampleStateModel } from '../models/example-state.model';

export default function withExampleSelectors() {
  return signalStoreFeature(
    { state: type<ExampleStateModel>() },
    withComputed((state) => ({
      someDataNames: mapByKey<ExampleModel>('name')(state.someData),
      someDataLength: computed(() => state.someData().length),
    })),
  );
}
