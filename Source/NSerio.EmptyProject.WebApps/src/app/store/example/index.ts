import { withDevtools } from '@angular-architects/ngrx-toolkit';
import { signalStore, withState } from '@ngrx/signals';
import withExampleMethods from './example.methods';
import withExampleSelectors from './example.selectors';
import exampleState from './example.state';

const ExampleStore = signalStore(
  { providedIn: 'root' },
  withDevtools('Example Store'),
  withState(exampleState),
  withExampleMethods(),
  withExampleSelectors(),
);

export default ExampleStore;
