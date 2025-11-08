import { Component } from '@angular/core';
import { AbstractControl, FormBuilder, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { NumberRangeDirective } from '../../../core/directives/number-range.directive';
import { EventService } from '../../../core/services/event.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-create-event',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, NumberRangeDirective],
  templateUrl: './create-event.component.html',
  styles: ``
})
export class CreateEventComponent {
  constructor(private formBuilder: FormBuilder,private toastr: ToastrService, private eventService: EventService,private router: Router) {}
  isSubmitted = false;

    futureDateValidator: ValidatorFn = (group: AbstractControl): { [key: string]: any } | null => {
    const date = group.get('date')?.value;
    const hours = group.get('hours')?.value;
    const minutes = group.get('minutes')?.value;
    if (!date && hours == null && minutes == null)
      return null;

    const selected = new Date(date);
    
    selected.setHours(hours,minutes,0,0);

    const now = new Date();

    if(selected < now)
      return {pastDate : true};

    return null;

    
  }

  form = this.formBuilder.group({
    title: ['',[Validators.required,Validators.minLength(3)]],
    description: ['', [Validators.required,Validators.minLength(10)]],
    date: ['', Validators.required],
    hours: [0,Validators.required],
    minutes: [0,Validators.required],
    location: ['',Validators.required],
    capacity: [null],
    visibility: ['true', Validators.required]
  },{validators : this.futureDateValidator});


  onSubmit(){
     if (this.form.invalid) {
    this.toastr.error("Please correct the errors before submitting");
    return;
    }
    const dateTime = new Date(this.form.value.date!);
    dateTime.setHours(this.form.value.hours!);
    dateTime.setMinutes(this.form.value.minutes!);

    const formData = {
      title : this.form.value.title,
      description : this.form.value.description,
      dateTime : dateTime.toISOString(),
      location: this.form.value.location,
      capacity : this.form.value.capacity,
      visibility: this.form.value.visibility
    };
    console.log(formData);
    
    this.eventService.createEvent(formData).subscribe({
      next: () => {
        this.toastr.success("You've been successfully added a new event!");
        this.router.navigateByUrl('/');
      },
      error: (err) =>{
        console.log(err)
        this.toastr.error("Failed to add event")
      }
    })

  }
  hasDisplayableError(controlName: string): Boolean {
    const control = this.form.get(controlName);
    return Boolean(control?.invalid) &&
      (this.isSubmitted || Boolean(control?.touched)|| Boolean(control?.dirty))
  }
}
