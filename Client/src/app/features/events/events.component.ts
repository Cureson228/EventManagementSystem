import { Component, OnInit } from '@angular/core';
import { EventService } from '../../core/services/event.service';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/services/auth.service';
import { RouterLink } from '@angular/router';
import { Event } from '../../core/models/event.model';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-events',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './events.component.html',
  styles: ``,
})
export class EventsComponent implements OnInit {
  events: Event[] = [];
  tags: string[] | null = null;
  selectedTags: string[] = [];
  currentIndex = 0;
  visibleEvents: any[] = [];
  userId: string | null = null;
  isLoading = true;
  eventsExist = false;
  constructor(
    private eventService: EventService,
    private authService: AuthService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.userId = this.authService.getUserIdFromToken();

    this.eventService.getAllTags().subscribe({
      next: (data) => {
        this.tags = data;
      },
    });
    
    this.eventService.getPublicEvents().subscribe({
         next: (data) => {
           this.events = data;
           this.updateVisibleEvents();
           this.isLoading = false;
           this.eventsExist = this.events.length > 0;
         },
         error: (err) => this.toastr.error('Failed to load events', err),
       });
    this.updateVisibleEvents();
    this.loadEvents();
  }
  
  loadEvents() {
    this.eventService.getPublicEvents(this.selectedTags).subscribe({
      next: (data) => {
        this.events = data;
        this.updateVisibleEvents();
        this.isLoading = false;
      },
      error: (err) => this.toastr.error('Failed to load events', err),
    });
    this.updateVisibleEvents();
  }

  toggleTag(tag: string) {
    const index = this.selectedTags?.indexOf(tag);
    if (index != null && index >= 0) {
      this.selectedTags?.splice(index, 1);
    } else {
      console.log('Pushed the tags');

      this.selectedTags?.push(tag);
    }
    this.loadEvents();
  }
  isSelectedTag(tag: string): boolean {
    return this.selectedTags?.includes(tag) ?? false;
  }

  clearTags() {
    this.selectedTags = [];
    this.loadEvents();
  }

  hasJoined(event: any): boolean {
    if (!event && !this.userId) return false;

    return event.participants.some((p: any) => p.userId === this.userId);
  }

  isFull = (event: any): boolean => {
    return event.capacity == event.participants.length;
  };
  updateVisibleEvents() {
    if (!this.events || this.events.length === 0) {
      this.visibleEvents = [];
      return;
    }
    this.visibleEvents = this.events.slice(
      this.currentIndex,
      this.currentIndex + 3
    );
  }

  next() {
    if (this.currentIndex < this.events.length - 3) {
      this.currentIndex++;
      this.updateVisibleEvents();
    }
  }

  previous() {
    if (this.currentIndex > 0) {
      this.currentIndex--;
      this.updateVisibleEvents();
    }
  }
}
