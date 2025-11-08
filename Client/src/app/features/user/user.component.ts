import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { trigger, transition, style, animate, query, group } from '@angular/animations';

@Component({
  selector: 'app-user',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './user.component.html',
  styles: ``,
  animations: [
    trigger('routeAnimations', [
      transition('LoginPage => RegisterPage', [
        style({ position: 'relative' }),
        query(':enter, :leave', [
          style({
            position: 'absolute',
            top: 0,
            left: 0,
            width: '100%'
          })
        ], { optional: true }),
        group([
          query(':leave', [
            animate('300ms ease', style({ opacity: 0, transform: 'translateX(-30px)' }))
          ], { optional: true }),
          query(':enter', [
            style({ opacity: 0, transform: 'translateX(30px)' }),
            animate('300ms ease', style({ opacity: 1, transform: 'translateX(0)' }))
          ], { optional: true })
        ])
      ]),
      transition('RegisterPage => LoginPage', [
        style({ position: 'relative' }),
        query(':enter, :leave', [
          style({
            position: 'absolute',
            top: 0,
            left: 0,
            width: '100%'
          })
        ], { optional: true }),
        group([
          query(':leave', [
            animate('300ms ease', style({ opacity: 0, transform: 'translateX(30px)' }))
          ], { optional: true }),
          query(':enter', [
            style({ opacity: 0, transform: 'translateX(-30px)' }),
            animate('300ms ease', style({ opacity: 1, transform: 'translateX(0)' }))
          ], { optional: true })
        ])
      ])
    ])
  ]
})

export class UserComponent {
  getRouterOutletState(outlet: RouterOutlet) {
    console.log(outlet?.activatedRouteData);
    return outlet?.activatedRouteData?.['animation'];
  }
}
