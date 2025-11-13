import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { EventService } from '../../../core/services/event.service'; 
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../../core/services/auth.service';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-event-details',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './event-details.component.html',
  styles: []
})
export class EventDetailsComponent implements OnInit {
  showConfirm = false;
  eventTags : string[] = [];
  event: any;
  isLoading = true;
  isOrganizer = false;
  isJoined = false;

  constructor(
    private route: ActivatedRoute,
    private eventService: EventService,
    private toastr: ToastrService,
    private router: Router,
    private authService: AuthService,
  ) {}

  ngOnInit(): void {
    this.loadEvent();
    
  }
  loadEvent(){
    this.isJoined = false;
    this.isOrganizer = false;
    const id = this.route.snapshot.paramMap.get('id');
    const userId = this.authService.getUserIdFromToken();
    if (id){
      this.eventService.getEventDetails(+id).subscribe({
        next: (data) =>{
          this.event = data;
          this.isLoading = false;

          if (!userId)
            return;
          
          if (userId == this.event.createdByUserId){
            console.log(userId == this.event.createdByUserId);
            
            this.isOrganizer = true;
            this.isJoined = true;
            return;
          }
          if(this.event.participants.some((p: any) => p.userId === userId)){
            this.isJoined = true;
          }
          
          
        },
        error: (err) => {
          this.toastr.error("Failed to load event", err);
          this.router.navigateByUrl('/');
        }
      })
      this.eventService.getEventTags(+id).subscribe({
        next: (data) =>{
          this.eventTags = data;
        },
        error: (err) => {
          this.toastr.error('Failed to load tags', err);
        }
      })
    }
    
  }

  openConfirmModal = () => this.showConfirm = true;

  confirmDelete(){
    this.eventService.deleteEvent(this.event.id).subscribe({
      next: () =>{
        this.toastr.info('Event has been successfully deleted');
        this.router.navigateByUrl('/');
      },
      error: (err) => this.toastr.error("Failed to delete this event", err)
    })
  }
  
  async joinEvent(){
    const isAuth = await firstValueFrom(this.authService.isAuthenticated$);
    
    if(!isAuth){
      this.toastr.warning('Please log in before joining');
      return;
    }

    this.eventService.joinEvent(this.event.id).subscribe({
      next: () =>{
        this.toastr.success('You have joined the event!');
        this.loadEvent();
      }
    })
  }

  leaveEvent(){
    this.eventService.leaveEvent(this.event.id).subscribe({
      next: () => {
        this.toastr.success('You have left the event');
        this.loadEvent();
      }
    })
  }


  goBack(){
    this.router.navigateByUrl('/');
  }
}