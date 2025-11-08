import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './footer.component.html',
  styles: ``
})
export class FooterComponent {
  constructor(private router : Router){}
  isUserPage(){
    return this.router.url.startsWith('/user/login') || this.router.url.startsWith('/user/signup')
  }
}
