import {
  Component,
  HostListener,
  effect,
  signal,
  viewChild,
} from '@angular/core';
import { HelloWorldComponent } from '@app/components/hello-world/hello-world.component';
import {
  FullCalendarComponent,
  FullCalendarModule,
} from '@fullcalendar/angular';
import {
  CalendarApi,
  CalendarOptions,
  DateSelectArg,
  DateSpanApi,
  EventClickArg,
  EventDropArg,
  ViewApi,
} from '@fullcalendar/core';
import { TranslateModule } from '@ngx-translate/core';
import dayGridPlugin from '@fullcalendar/daygrid';
import timeGridPlugin from '@fullcalendar/timeGrid';
import interactionPlugin, {
  EventDragStartArg,
  EventResizeDoneArg,
} from '@fullcalendar/interaction';
import { EventImpl } from '@fullcalendar/core/internal';
import { ButtonModule } from 'primeng/button';
import { SelectButtonModule } from 'primeng/selectbutton';
import { CalendarToolbarComponent } from '../../components/calendar/calendar-toolbar/calendar-toolbar.component';
import { CalendarComponent } from '@app/components/calendar/calendar/calendar.component';
import { FullCalendarViewEnum } from '@app/models/full-calendar-view.model';

@Component({
  selector: 'app-home',
  standalone: true,
  templateUrl: './home.view.html',
  styleUrl: './home.view.scss',
  imports: [
    HelloWorldComponent,
    TranslateModule,
    FullCalendarModule,
    ButtonModule,
    SelectButtonModule,
    CalendarToolbarComponent,
    CalendarComponent,
  ],
})
export class HomeView {
  public view = signal<FullCalendarViewEnum>(FullCalendarViewEnum.DayGridMonth);
  public title = signal<string>('');
  public next = signal<boolean>(false);
  public prev = signal<boolean>(false);
  public setToday = signal<boolean>(false);
}
