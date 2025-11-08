import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup,ReactiveFormsModule, ValidatorFn, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { EventService } from '../../../core/services/event.service';
import { ToastrService } from 'ngx-toastr';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-edit-event',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, RouterLink],
  templateUrl: './edit-event.component.html',
  styles: ``
})
export class EditEventComponent implements OnInit {

  form! : FormGroup;
  eventId! : number;
  isLoading = true;
  isSubmitted = false;

  constructor(
    private formBuilder : FormBuilder,
    private route: ActivatedRoute,
    private eventService: EventService,
    private router: Router,
    private toastr: ToastrService
    
  ){}

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
  ngOnInit(): void {
    this.form = this.formBuilder.group({
      title: ['',[Validators.required,Validators.minLength(3)]],
      description: ['', [Validators.required,Validators.minLength(10)]],
      date: ['', Validators.required],
      hours: [0,Validators.required],
      minutes: [0,Validators.required],
      location: ['',Validators.required],
      capacity: [null],
      visibility: ['true', Validators.required]
    },{validators : this.futureDateValidator});

    this.eventId = +this.route.snapshot.paramMap.get('id')!;
    this.eventService.getEventDetails(this.eventId).subscribe({
      next: (event) => {
        console.log(event);
        
        this.form.patchValue(event);

        const date = new Date(event.dateTime);
        
        this.form.patchValue({
          date: date.toISOString().slice(0,10),
          minutes: date.getMinutes(),
          hours: date.getHours()
        })

        console.log(this.form.value);
        
        this.isLoading = false;
      },
      error: (err) => {
        this.toastr.error('Failed to load event details');
        this.router.navigateByUrl('/');
      }
    })
    
  }
  onSubmit() : void{
    if (this.form.invalid){
      this.toastr.error("Please make sure inputs are valid before submitting");
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
    
    this.eventService.updateEvent(this.eventId, formData).subscribe({
      next: () => {
        this.toastr.success("You've been successfully updated the event");
        this.router.navigate(['/event', this.eventId]);
      },
      error: () => this.toastr.error("Failed to update event")
    });
  }

  hasDisplayableError(controlName: string): Boolean {
    const control = this.form.get(controlName);
    return Boolean(control?.invalid) &&
      (this.isSubmitted || Boolean(control?.touched)|| Boolean(control?.dirty))
  }

  cancelEdit(){
    this.router.navigate(['/event', this.eventId]);
  }
}
