import {
  Component,
  OnChanges,
  SimpleChanges,
  computed,
  effect,
  input,
  model,
  signal,
  viewChild,
} from '@angular/core';
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
import { EventImpl } from '@fullcalendar/core/internal';
import {
  EventDragStartArg,
  EventResizeDoneArg,
} from '@fullcalendar/interaction';
import dayGridPlugin from '@fullcalendar/daygrid';
import timeGridPlugin from '@fullcalendar/timeGrid';
import interactionPlugin from '@fullcalendar/interaction';

@Component({
  selector: 'app-calendar',
  standalone: true,
  imports: [FullCalendarModule],
  templateUrl: './calendar.component.html',
  styleUrl: './calendar.component.scss',
})
export class CalendarComponent implements OnChanges {
  public view = model<string>('');
  public title = model<string>('17 Jun 2024');

  public next = input<boolean>(false);
  public prev = input<boolean>(false);
  public today = input<boolean>(false);

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
      console.log('datesSet:title', arg.view.title);
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

  private fullCalendarApi = computed(() => this.fullCalendar()?.getApi());

  constructor() {
    effect(() => {
      if (this.view()) {
        this.fullCalendar()?.getApi()?.changeView(this.view());
      }
    },
    {
      allowSignalWrites: true
    }
  );

  }
  ngOnChanges(changes: SimpleChanges): void {
    if (changes) {
      if (changes['next']) {
        this.fullCalendar()?.getApi()?.next();
      }
      if (changes['prev']) {
        this.fullCalendar()?.getApi()?.prev();
      }
      if (changes['today']) {
        this.fullCalendar()?.getApi()?.today();
      }
    }
  }

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
}
