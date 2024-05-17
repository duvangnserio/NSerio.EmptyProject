import {
  addRecordToList,
  removeRecordFromList,
  replaceRecordInList,
  set,
} from '@app/helpers/mutations.helper';
import { ExampleModel } from '@app/models/example.model';
import { signalStoreFeature, type, withMethods } from '@ngrx/signals';
import { ExampleStateModel } from '../models/example-state.model';

export default function withExampleMutations() {
  return signalStoreFeature(
    { state: type<ExampleStateModel>() },
    withMethods((store) => ({
      setSomeData: (someData: ExampleModel[]) =>
        set<ExampleStateModel>('someData')(store, someData),

      addSomeData: (someData: ExampleModel) =>
        addRecordToList<ExampleStateModel>('someData')(store, someData),

      removeSomeData: (artifactId: number) =>
        removeRecordFromList<ExampleStateModel, ExampleModel>(
          'someData',
          'artifactId',
        )(store, artifactId),

      updateSomeData: (someData: ExampleModel) =>
        replaceRecordInList<ExampleStateModel, ExampleModel>(
          'someData',
          'artifactId',
        )(store, someData),
    })),
  );
}
