import { Component } from '@angular/core';
import { HelloWorldComponent } from '@app/components/hello-world/hello-world.component';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [HelloWorldComponent, TranslateModule],
  templateUrl: './home.view.html',
  styleUrl: './home.view.scss',
})
export class HomeView {}
