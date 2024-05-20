import { Component, signal } from '@angular/core';
import { HelloWorldComponent } from '@app/components/hello-world/hello-world.component';
import { FullCalendarModule } from '@fullcalendar/angular';
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

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [HelloWorldComponent, TranslateModule, FullCalendarModule],
  templateUrl: './home.view.html',
  styleUrl: './home.view.scss',
})
export class HomeView {
  calendarOptions = signal<CalendarOptions> ({
    initialView: 'dayGridMonth', // aka defaultView in fullcalendar vue
    plugins: [dayGridPlugin, timeGridPlugin, interactionPlugin],
    selectable: true,
    weekends: true,
    headerToolbar: {
      left: '',
      center: '',
      right: '',
    },//empty properties to remove the default elements
    // datesRender: angular doesn't have this
    //eventLimit: angular doesn't have this
    events: [
      { title: 'event 1', date: '2024-05-17' },
      { title: 'event 2', date: '2024-05-18' },
    ],
    eventTimeFormat: {
      hour: '2-digit',
      minute: '2-digit',
      meridiem: 'short',
    },
    // eventRender: angular doesn't have this
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
    // slotLabelFormat:{
    //   hour: 'numeric',
    //             minute: '2-digit',
    //             omitZeroMinute: true,
    //             meridiem: 'long',
    // } // angular doesn't have this, the object doesn't match with fullcalendar vue
    eventDragStart: this.eventDragStart,
    eventResizeStart: this.eventDragStart,
    windowResize: this.setAspectRatio,
    eventClick: this.fullCalendarEventClick,
    select: this.fullCalendarSelect,
    eventDrop: this.fullCalendarDragDropResize,
    eventResize: this.fullCalendarDragResize,
    // eventPositioned: angular doesn't have this,
  });

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
