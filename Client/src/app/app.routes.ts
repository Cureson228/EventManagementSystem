import { Routes } from '@angular/router';
import { HomeComponent } from './features/home/home.component';
import { UserComponent } from './features/user/user.component';
import { RegisterComponent } from './features/user/register/register.component';
import { LoginComponent } from './features/user/login/login.component';
import { CreateEventComponent } from './features/events/create-event/create-event.component';
import { authGuard } from './core/guards/auth.guard';
import { EventDetailsComponent } from './features/events/event-details/event-details.component';
import { EditEventComponent } from './features/events/edit-event/edit-event.component';
import { organizerGuard } from './core/guards/organizer.guard';
import { MyEventsComponent } from './features/events/my-events/my-events.component';

export const routes: Routes = [
    {path: '', component: HomeComponent },
    {path: 'user', component: UserComponent, children: [{
        path: 'signup', component: RegisterComponent, data: {animation: 'RegisterPage'}
    },{
        path: 'login', component: LoginComponent, data: {animation: 'LoginPage'}
    }]},
    {path: 'user/myevents', component : MyEventsComponent, canActivate: [authGuard]},
    {path: 'event/create', component: CreateEventComponent, canActivate: [authGuard]},
    {path: 'event/:id', component: EventDetailsComponent},
    {path: 'event/:id/edit', component: EditEventComponent , canActivate: [organizerGuard]},
    {path: '**', redirectTo: '/'}
];
