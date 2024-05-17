import { withDevtools } from '@angular-architects/ngrx-toolkit';
import { signalStore, withState } from '@ngrx/signals';
import withHelloWorldMethods from './hello-world.methods';
import withHelloWorldSelectors from './hello-world.selectors';
import helloWorldState from './hello-world.state';

const HelloWorldStore = signalStore(
  { providedIn: 'root' },
  withDevtools('Hello World Store'),
  withState(helloWorldState),
  withHelloWorldMethods(),
  withHelloWorldSelectors()
);

export default HelloWorldStore;
