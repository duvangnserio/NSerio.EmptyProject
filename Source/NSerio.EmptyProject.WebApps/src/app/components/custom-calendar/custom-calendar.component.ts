import { CommonModule } from '@angular/common';
import { Component, forwardRef  } from '@angular/core';
import { ControlValueAccessor, FormsModule, NG_VALUE_ACCESSOR } from '@angular/forms';
import { ConfirmationService, PrimeNGConfig } from 'primeng/api';
import { CalendarModule } from 'primeng/calendar';
import { StepperModule } from 'primeng/stepper';
import { ConfirmDialogModule } from 'primeng/confirmdialog';

@Component({
  selector: 'app-custom-calendar',
  standalone: true,
  imports: [
    CommonModule,
    CalendarModule,    
    FormsModule,
    StepperModule,
    ConfirmDialogModule
  ],
  templateUrl: './custom-calendar.component.html',
  styleUrl: './custom-calendar.component.scss',
  providers: [ConfirmationService,
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

  constructor(private primengConfig: PrimeNGConfig, private confirmationService: ConfirmationService) {
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

 
  confirm() {
    this.confirmationService.confirm({
      message: '¿Estás seguro de que deseas proceder?',
      header: 'Confirmación',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {        
        console.log('Confirmado');
      },
      reject: () => {
        console.log('Rechazado');
      }
    });
  }
}