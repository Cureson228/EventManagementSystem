import { Component } from '@angular/core';
import { Router, RouterLink, RouterState } from "@angular/router";
import { CommonModule } from '@angular/common'; 
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterLink, CommonModule],
  templateUrl: './navbar.component.html',
  styles: ``
})
export class NavbarComponent {
  constructor(public authService : AuthService){
  }

  
  logout(){
    this.authService.logout()
    
  }
}
