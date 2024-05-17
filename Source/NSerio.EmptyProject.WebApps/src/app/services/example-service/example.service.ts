import { Injectable } from '@angular/core';

import { ExampleModel } from '@app/models/example.model';

import { getUrlParameter } from '@app/helpers/uri.helper';
import { BaseService } from '../base-service/base.service';

@Injectable({
  providedIn: 'root',
})
export class ExampleService extends BaseService {
  //#region [Properties]

  private readonly workspaceId = Number(getUrlParameter('AppID'));
  private readonly urls = {
    example: `example/${this.workspaceId}/`,
  };

  //#endregion [Properties]

  //#region [Public Functions]

  public async getSomeDataAync(): Promise<ExampleModel[]> {
    return this.getAsync(this.urls.example);
  }

  public async createSomeDataAsync(name: string): Promise<number> {
    return this.postAsync(this.urls.example, name);
  }

  public async updateSomeDataAsync(
    artifactId: number,
    name: string,
  ): Promise<void> {
    const url = `${this.urls.example}${artifactId}`;
    return this.putAsync(url, name);
  }

  public async deleteSomeDataAsync(artifactId: number): Promise<void> {
    const url = `${this.urls.example}${artifactId}`;
    return this.deleteAsync(url);
  }

  //#endregion [Public Functions]
}
