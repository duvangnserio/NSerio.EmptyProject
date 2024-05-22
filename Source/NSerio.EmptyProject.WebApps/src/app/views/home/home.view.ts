import {
  Component,
  HostListener,
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

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    HelloWorldComponent,
    TranslateModule,
    FullCalendarModule,
    ButtonModule,
    SelectButtonModule,
  ],
  templateUrl: './home.view.html',
  styleUrl: './home.view.scss',
})
export class HomeView {
  view = signal<string>('dayGridMonth');
  title = signal<string>('');

  calendarOptions = signal<CalendarOptions>({
    initialView: this.view(), // aka defaultView in fullcalendar vue
    plugins: [dayGridPlugin, timeGridPlugin, interactionPlugin],
    selectable: true,
    height: '90%',
    weekends: true,
    headerToolbar: {
      left: '',
      center: '',
      right: '',
    }, //empty properties to remove the default elements
    datesSet: (arg) => {
      console.log('datesSet', arg);
      this.title.set(arg.view.title);
    }, // aka datesRender in fullcalendar vue
    //eventLimit: angular doesn't have this
    events: [
      { title: 'event 1', date: '2024-05-17', allDay: true },
      {
        title: 'event 2',
        start: '2024-05-03T14:33:00',
        end: '2024-05-03T15:33:00',
      },
    ],
    eventTimeFormat: {
      hour: '2-digit',
      minute: '2-digit',
      meridiem: 'short',
    },
    eventDidMount: (arg) => {
      // arg.el.style.borderColor = 'red';
      console.log('eventDidMount', arg);
    }, //aka eventRender in fullcalendar vue
    fixedWeekCount: false,
    nowIndicator: true,
    editable: true,
    allDayMaintainDuration: true,
    eventAllow: this.fullCalendarEventAllow, //TODO: check what is intended here
    snapDuration: '00:15:00',
    slotEventOverlap: false,
    eventResizableFromStart: true,
    navLinks: true,
    navLinkDayClick: this.navLinkDayClick,
    forceEventDuration: true,
    timeZone: 'local',
    // columnHeader: angular doesn't have this
    slotLabelFormat: {
      hour: 'numeric',
      minute: '2-digit',
      omitZeroMinute: true,
      meridiem: true,
    },
    eventDragStart: this.eventDragStart,
    eventResizeStart: this.eventDragStart,
    windowResize: this.setAspectRatio,
    eventClick: this.fullCalendarEventClick,
    select: this.fullCalendarSelect,
    eventDrop: this.fullCalendarDragDropResize,
    eventResize: this.fullCalendarDragResize,
    eventDragStop(arg) {
      console.log('eventDragStop', arg);
    }, //could be used as event positioned in angular
    // eventPositioned: angular doesn't have this,
  });

  fullCalendar = viewChild(FullCalendarComponent);
  viewOptions = [
    { label: 'Day', value: 'dayGridDay' },
    { label: 'Week', value: 'timeGridWeek' },
    { label: 'Month', value: 'dayGridMonth' },
  ];

  private readonly dayGridMonthView = 'dayGridMonth';
  private readonly timeGridDayView = 'timeGridDay';
  private readonly timeGridWeekView = 'timeGridWeek';

  fullCalendarDragResize(arg: EventResizeDoneArg): void {
    console.log('fullCalendarDragResize', arg);
  }

  fullCalendarDragDropResize(arg: EventDropArg): void {
    console.log('fullCalendarDragDropResize', arg);
  }

  fullCalendarSelect(arg: DateSelectArg): void {
    console.log('fullCalendarSelect', arg);
  }

  fullCalendarEventClick(arg: EventClickArg): void {
    console.log('fullCalendarEventClick', arg);
  }

  fullCalendarEventAllow(
    span: DateSpanApi,
    movingEvent: EventImpl | null,
  ): boolean {
    console.log('fullCalendarEventAllow', span, movingEvent);
    return true;
  }

  navLinkDayClick(this: CalendarApi, date: Date, jsEvent: UIEvent): void {
    console.log('navLinkDayClick', date, jsEvent);
  }

  eventDragStart(arg: EventDragStartArg): void {
    console.log('eventDragStart', arg);
  }

  setAspectRatio(arg: { view: ViewApi }): void {
    console.log('setAspectRatio', arg);
  }

  //#region FullCalendarComponent methods

  @HostListener('document:keydown.shift.d')
  setViewToDay() {
    this.changeView(this.timeGridDayView);
  }

  @HostListener('document:keydown.shift.w')
  setViewToWeek() {
    this.changeView(this.timeGridWeekView);
  }

  @HostListener('document:keydown.shift.m')
  setViewToMonth() {
    this.changeView(this.dayGridMonthView);
  }

  changeView(view: string) {
    this.view.set(view);
    this.fullCalendar()?.getApi().changeView(view);
  }

  @HostListener('document:keydown.shift.arrowright')
  next() {
    this.fullCalendar()?.getApi().next();
  }

  @HostListener('document:keydown.shift.arrowleft')
  prev() {
    this.fullCalendar()?.getApi().prev();
  }

  @HostListener('document:keydown.shift.t')
  setToday() {
    this.fullCalendar()?.getApi().today();
  }
  //#endregion
}
