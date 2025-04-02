import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ThumbnailDocument, ThumbnailDocumentsService } from '@app/services/thumbnail-documents.service';
import { CardModule } from 'primeng/card';
import { ImageModule } from 'primeng/image';
import { SelectButtonModule } from 'primeng/selectbutton';
import { TooltipModule } from 'primeng/tooltip';

@Component({
  selector: 'app-thumbnail-visualizer.view',
  standalone: true,
  imports: [CardModule, ImageModule, TooltipModule, SelectButtonModule, CommonModule, FormsModule,],
  templateUrl: './thumbnail-visualizer.view.html',
  styleUrl: './thumbnail-visualizer.view.scss',
})
export class ThumbnailVisualizerViewComponent {
  public thumbnailDocuments = signal<ThumbnailDocument[]>([]);
  private readonly thumbnailDocumentsService = inject(
    ThumbnailDocumentsService,
  );

  sizes = [
    { label: 'S', value: '50%' },
    { label: 'M', value: '75%' },
    { label: 'L', value: '100%' }
  ];

  selectedSize = '50%'; // Default to 'S'
  async ngOnInit() {
    const thumbnailDocuments = await this.thumbnailDocumentsService
      .getThumbnailDocuments(1, 10);
    this.thumbnailDocuments.set(thumbnailDocuments);
  }
}
