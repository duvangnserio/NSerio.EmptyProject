import { Component, HostListener, input, model } from '@angular/core';
import { FullCalendarViewEnum } from '@app/models/full-calendar-view.model';
import { ButtonModule } from 'primeng/button';
import { SelectButtonModule } from 'primeng/selectbutton';


@Component({
  selector: 'app-calendar-toolbar',
  standalone: true,
  imports: [ButtonModule, SelectButtonModule],
  templateUrl: './calendar-toolbar.component.html',
  styleUrl: './calendar-toolbar.component.scss',
})
export class CalendarToolbarComponent {
  public title = input.required<string>();
  public view = model<FullCalendarViewEnum>(FullCalendarViewEnum.DayGridMonth);
  public onNext = model<boolean>(false);
  public onPrev = model<boolean>(false);
  public onToday = model<boolean>(false);

  @HostListener('document:keydown.shift.d')
  setViewToDay() {
    this.changeView(FullCalendarViewEnum.TimeGridDay);
  }

  @HostListener('document:keydown.shift.w')
  setViewToWeek() {
    this.changeView(FullCalendarViewEnum.TimeGridWeek);
  }

  @HostListener('document:keydown.shift.m')
  setViewToMonth() {
    this.changeView(FullCalendarViewEnum.DayGridMonth);
  }

  changeView(view: FullCalendarViewEnum) {
    this.view.set(view);
  }

  @HostListener('document:keydown.shift.arrowright')
  next() {
    this.onNext.set(!this.onNext());
  }

  @HostListener('document:keydown.shift.arrowleft')
  prev() {
    this.onPrev.set(!this.onPrev());
  }

  @HostListener('document:keydown.shift.t')
  setToday() {
    this.onToday.set(!this.onToday());
  }
}
