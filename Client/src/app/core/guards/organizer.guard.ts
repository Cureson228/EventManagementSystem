import { inject, Inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { EventService } from '../services/event.service';
import { AuthService } from '../services/auth.service';
import { catchError, map, of } from 'rxjs';

export const organizerGuard: CanActivateFn = (route, state) => {
  const eventService = inject(EventService);
  const authService = inject(AuthService);
  const router = inject(Router);

  const eventId = Number(route.paramMap.get('id'));
  const currentUser = authService.getUserIdFromToken(); 

  if (!currentUser) {
    return router.createUrlTree(['/']);
  }

  return eventService.getEventDetails(eventId).pipe(
    map(event => {
      if (event.createdByUserId === currentUser) {
        return true;
      } else {
        return router.createUrlTree(['/']);
      }
    }),
    catchError(() => of(router.createUrlTree(['/'])))
  );
};
