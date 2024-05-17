import pkg from '@pkg';
import { HelloWorldStateModel } from '../models/hello-world-state.model';

const helloWorldState: HelloWorldStateModel = {
  version: `v${pkg.version}`,
  links: [
    { text: 'Angular IO', url: 'https://angular.io/docs' },
    { text: 'Angular Dev', url: 'https://angular.dev/overview' },
    { text: 'Prime Angular', url: 'https://primeng.org/' },
    { text: 'Prime Flex', url: 'https://primeflex.org/installation' },
    {
      text: 'ngx-translate',
      url: 'https://github.com/ngx-translate/core',
    },
    { text: 'ngrx signals', url: 'https://ngrx.io/guide/signals' },
  ],
};

export default helloWorldState;
