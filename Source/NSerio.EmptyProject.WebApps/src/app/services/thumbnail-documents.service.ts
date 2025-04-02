import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { BaseService } from './base-service/base.service';

@Injectable({
  providedIn: 'root',
})
export class ThumbnailDocumentsService extends BaseService {
  constructor() {
    super();
  }

  getThumbnailDocuments(page: number, pageSize: number) {
    const params: HttpParams = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    return this.getAsync<ThumbnailDocument[]>('thumbnail-documents', {
      params,
    });
  }
}

export class ThumbnailDocument {
  artifactId?: number;
  name?: string;
  thumbnailSrc?: string;
}
