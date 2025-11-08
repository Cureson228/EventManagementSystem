import { Component } from '@angular/core';
import { EventsComponent } from "../events/events.component";


@Component({
  selector: 'app-home',
  standalone: true,
  imports: [EventsComponent],
  templateUrl: './home.component.html',
  styles: ``
})
export class HomeComponent {

}
