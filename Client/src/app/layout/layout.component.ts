import { Component } from '@angular/core';
import { NavbarComponent } from './navbar/navbar.component';
import { FooterComponent } from './footer/footer.component';
import { RouterOutlet } from '@angular/router';
import { AiAssistanceComponent } from '../features/ai-assistance/ai-assistance.component';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [NavbarComponent, FooterComponent, RouterOutlet, AiAssistanceComponent],
  templateUrl: './layout.component.html',
  styles: ``
})
export class LayoutComponent {

}
