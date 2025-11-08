import { Component, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from "@angular/router";
import { AuthService } from '../../../core/services/auth.service';
import { ToastrModule, ToastrService } from 'ngx-toastr';
import { CommonModule } from '@angular/common';
import { take } from 'rxjs';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [RouterLink, ReactiveFormsModule, ToastrModule, CommonModule],
  templateUrl: './login.component.html',
  styles: ``
})
export class LoginComponent implements OnInit {
   constructor(private formBuilder : FormBuilder, private authService: AuthService, private toastr : ToastrService, private router: Router){}
   isSubmitted = false;
   form = this.formBuilder.group({
    email: ['', [Validators.required,Validators.email]],
    password: ['', [Validators.required]]
   });

    ngOnInit(): void {
    this.authService.isAuthenticated$
      .pipe(take(1))
      .subscribe(isAuth => {
        if (isAuth) {
          this.router.navigate(['/']); 
        }
      });
  }

   onSubmit(){
      this.isSubmitted = true;
      if (this.form.invalid){
        this.toastr.error("Please correct the errors before submitting");
        return;
      }
      this.authService.login(this.form.value).subscribe({
        next: () => {
          this.toastr.success("Successfully logged in!");
          this.router.navigateByUrl('/');
        },
        error: (err) => this.toastr.error(err.error?.message || 'login failed')
      });
   }

    hasDisplayableError(controlName: string): Boolean {
      const control = this.form.get(controlName);
      return Boolean(control?.invalid) &&
        (this.isSubmitted || Boolean(control?.touched)|| Boolean(control?.dirty))
  }
}
