import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidatorFn, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { FirstKeyPipe } from '../../../shared/pipes/first-key.pipe';
import { Toast, ToastrModule, ToastrService } from 'ngx-toastr';
import { Router, RouterLink } from "@angular/router";
import { take } from 'rxjs';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, FirstKeyPipe, ToastrModule, RouterLink],
  templateUrl: './register.component.html',
  styles: ``
})
export class RegisterComponent implements OnInit {
  constructor(private formBuilder : FormBuilder, private toastr: ToastrService, private authService: AuthService, private router: Router){}

    ngOnInit(): void {
    this.authService.isAuthenticated$
      .pipe(take(1))
      .subscribe(isAuth => {
        if (isAuth) {
          this.router.navigate(['/']); 
        }
      });
  }
  
    passwordMatchValidator: ValidatorFn = (control: AbstractControl): null => {
    const password = control.get('password')
    const confirmPassword = control.get('confirmPassword')

    if (password && confirmPassword && password.value != confirmPassword.value)
      confirmPassword?.setErrors({ passwordMismatch: true })
    else
      confirmPassword?.setErrors(null)

    return null;
  }

  isSubmitted = false;
  form = this.formBuilder.group({
    fullName : ['', Validators.required],
    email: ['', [Validators.required,Validators.email]],
    password: ['',[Validators.required,Validators.minLength(8),Validators.pattern('^(?=.*[A-Z])(?=(?:.*\\d){5,}).*$')]],
    confirmPassword: ['']
  },{validators : this.passwordMatchValidator});



  onSubmit(){
    this.isSubmitted = true;

    if (this.form.invalid){
      this.toastr.error("Please correct the errors before submitting");
      return;
    }
    this.authService.register(this.form.value).subscribe({
      next: () =>{
        this.toastr.success("Successfully registered");
        this.router.navigateByUrl("/");
      },
      error: (err) => this.toastr.error(err.error?.message || 'Registration failed')
    })
    
    
  }

    hasDisplayableError(controlName: string): Boolean {
    const control = this.form.get(controlName);
    return Boolean(control?.invalid) &&
      (this.isSubmitted || Boolean(control?.touched)|| Boolean(control?.dirty))
  }
}
