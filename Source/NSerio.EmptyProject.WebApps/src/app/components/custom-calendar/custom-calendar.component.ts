import { CommonModule } from '@angular/common';
import { Component, forwardRef  } from '@angular/core';
import { ControlValueAccessor, FormsModule, NG_VALUE_ACCESSOR } from '@angular/forms';
import { PrimeNGConfig } from 'primeng/api';
import { CalendarModule } from 'primeng/calendar';
import { StepperModule } from 'primeng/stepper';

@Component({
  selector: 'app-custom-calendar',
  standalone: true,
  imports: [
    CommonModule,
    CalendarModule,    
    FormsModule,
    StepperModule
  ],
  templateUrl: './custom-calendar.component.html',
  styleUrl: './custom-calendar.component.scss',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => CustomCalendarComponent),
      multi: true
    }]
})
export class CustomCalendarComponent implements ControlValueAccessor {
  protected innerValue: Date | null = null;

  private onChange: any = (_: any) => {};
  private onTouched: any = () => {};

  constructor(private primengConfig: PrimeNGConfig) {
    this.primengConfig.ripple = true;
  }

  writeValue(value: Date): void {
    if (value !== this.innerValue) {
      this.innerValue = value;
      console.log('Selected Date:', this.innerValue);
    }
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  setDisabledState?(isDisabled: boolean): void {
    this.isDisabled = isDisabled;
  }

  isDisabled: boolean = false;

  onDateChange(event: any) {  
    const value = event;
    this.innerValue = value;
    this.onChange(value);
    this.onTouched();
   
    console.log('Selected Date:', value);
  }
}