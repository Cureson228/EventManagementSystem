import { Directive, ElementRef, HostListener, Input } from '@angular/core';
import { NgControl } from '@angular/forms';

@Directive({
  selector: '[appNumberRange]',
  standalone: true
})
export class NumberRangeDirective {
  constructor(private ngControl: NgControl) {}

  @HostListener('input', ['$event'])
  onInput(event: Event) {
    const input = event.target as HTMLInputElement;
    let value = Number(input.value);
    const controlName = this.ngControl.name;

    if (controlName === 'hours') {
      if (value > 23) value = 23;
      if (value < 0) value = 0;
    }

    if (controlName === 'minutes') {
      if (value > 59) value = 59;
      if (value < 0) value = 0;
    }

    this.ngControl.control?.setValue(value);
  }
}