import { Component } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, MaxLengthValidator, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { NumberRangeDirective } from '../../../core/directives/number-range.directive';
import { EventService } from '../../../core/services/event.service';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-create-event',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, NumberRangeDirective, RouterLink],
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
    visibility: ['true', Validators.required],
    tags: this.formBuilder.array<string>([], [this.maxTagsValidator(5)])
  },{validators : this.futureDateValidator});

  get tags() : FormArray{
    return this.form.get('tags') as FormArray;
  }

  addTag(input : HTMLInputElement){
    const value = input.value.trim();
    if (!value)
      return;

    const lowerCaseValue = value.toLowerCase();

    const existing = this.tags.value.map((t : string) => t.toLowerCase());

    if (existing.includes(lowerCaseValue)){
      this.toastr.warning('This tag already exists');
      input.value = '';
      return;
    }

    if (this.tags.length >= 5){
      this.toastr.warning("You can add only up to 5 tags only");
    }

    this.tags.push(this.formBuilder.control(value));
    input.value = '';
  }

  removeTag(index: number){
    this.tags.removeAt(index);
  }

  maxTagsValidator(max: number){
    return (control: AbstractControl) =>{
      const arr = control as FormArray;
      return arr.length > max ? {maxTags: true} : null;
    }
  }

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
      visibility: this.form.value.visibility,
      tags: this.form.value.tags
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
