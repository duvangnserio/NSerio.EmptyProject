import { LinkModel } from '@app/models/link.model';

export type HelloWorldStateModel = {
  version: string;
  links: LinkModel[];
};
