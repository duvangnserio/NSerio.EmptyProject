import { CommonModule } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  inject,
  model,
} from '@angular/core';
import { HelloWorldStore } from '@app/store';
import { TranslateModule } from '@ngx-translate/core';

import { DividerModule } from 'primeng/divider';

@Component({
  selector: 'app-hello-world',
  standalone: true,
  imports: [CommonModule, TranslateModule, DividerModule],
  templateUrl: './hello-world.component.html',
  styleUrl: './hello-world.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HelloWorldComponent {
  //#region [Injects]

  private readonly helloWorldStore = inject(HelloWorldStore);

  //#endregion [Injects]

  //#region [Properties]

  public title = model.required<string>();
  public subtitle = model.required<string>();
  public readonly version = this.helloWorldStore.version;
  public readonly links = this.helloWorldStore.links;

  //#endregion [Properties]
}
