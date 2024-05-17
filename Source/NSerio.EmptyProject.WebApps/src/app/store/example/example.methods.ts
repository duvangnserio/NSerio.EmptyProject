import { inject } from '@angular/core';
import { ExampleService } from '@app/services/example-service/example.service';
import { signalStoreFeature, type, withMethods } from '@ngrx/signals';

import { ExampleStateModel } from '../models/example-state.model';
import withExampleMutations from './example.mutations';

export default function withExampleMethods() {
  return signalStoreFeature(
    { state: type<ExampleStateModel>() },
    withExampleMutations(),
    withMethods((store) => {
      const exampleService = inject(ExampleService);

      return {
        async getSomeDataAync() {
          const someData = await exampleService.getSomeDataAync();
          store.setSomeData(someData);
        },

        async createSomeDataAsync(name: string) {
          const artifactId = await exampleService.createSomeDataAsync(name);
          store.addSomeData({ artifactId, name });
        },

        async deleteSomeDataAsync(artifactId: number) {
          await exampleService.deleteSomeDataAsync(artifactId);
          store.removeSomeData(artifactId);
        },

        async updateSomeDataAsync(artifactId: number, name: string) {
          await exampleService.updateSomeDataAsync(artifactId, name);
          store.updateSomeData({ artifactId, name });
        },
      };
    }),
  );
}
