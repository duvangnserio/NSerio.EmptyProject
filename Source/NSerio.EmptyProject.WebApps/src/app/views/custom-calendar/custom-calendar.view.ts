import { Component } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CustomCalendarComponent } from '@app/components/custom-calendar/custom-calendar.component';

@Component({
  selector: 'app-custom-calendar-view',
  standalone: true,
  imports: [CustomCalendarComponent, FormsModule,ReactiveFormsModule],
  templateUrl: './custom-calendar.view.html',
  styleUrl: './custom-calendar.view.scss'
})
export class CustomCalendarView {
  form: FormGroup;

  constructor(private fb: FormBuilder) {
    this.form = this.fb.group({
      customCalendar: ['']
    });
  }

  onSubmit() {
    console.log(this.form.value);
  }
}
