import { Component, inject, OnInit } from '@angular/core';
import { EventService } from '../../../core/services/event.service'; 
import { Event } from '../../../core/models/event.model'; 
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from "@angular/router";


interface CalendarItem {
  id : number;
  title: string;
  start: Date;
  color: string;
}

@Component({
  selector: 'app-calendar',
  standalone: true,
  imports: [DatePipe, CommonModule, FormsModule, RouterLink],
  templateUrl: 'my-events.component.html',
  styles : ``
})
export class MyEventsComponent implements OnInit {
  private eventService = inject(EventService);

  view: 'month' | 'week' = 'month';
  today = new Date();
  current = new Date(); // any date inside visible month/week

  // month: array of weeks, each week is array of Date
  monthWeeks: Date[][] = [];

  // week: 7 dates
  weekDays: Date[] = [];

  // loaded events
  events: CalendarItem[] = [];

  // weekday labels (localized could be used)
  weekdayShort = ['Mon','Tue','Wed','Thu','Fri','Sat','Sun'];
  
  ngOnInit(): void {
    this.buildMonth(this.current);
    this.buildWeek(this.current);

    this.eventService.getUserEvents().subscribe({
      next: (data: Event[]) => {
        this.events = (data || []).map(e => ({
          id: e.id,
          title: e.title,
          start: new Date(e.dateTime),
          color: '#a5b4fc',
          original: e
        }));
      },
      error: (err : any) => console.error(err)
    });
  }

  // NAV
  prev(): void {
    if (this.view === 'month') {
      this.current = new Date(this.current.getFullYear(), this.current.getMonth() - 1, 1);
      this.buildMonth(this.current);
    } else {
      this.current = this.addDays(this.current, -7);
      this.buildWeek(this.current);
    }
  }
  next(): void {
    if (this.view === 'month') {
      this.current = new Date(this.current.getFullYear(), this.current.getMonth() + 1, 1);
      this.buildMonth(this.current);
    } else {
      this.current = this.addDays(this.current, 7);
      this.buildWeek(this.current);
    }
  }
  goToday(): void {
    this.current = new Date();
    this.buildMonth(this.current);
    this.buildWeek(this.current);
  }
  setView(v: 'month'|'week'): void {
    this.view = v;
    // keep current in same visible week/month
    if (v === 'month') this.buildMonth(this.current);
    else this.buildWeek(this.current);
  }

  // Build month grid (weeks start Monday to match photo)
  buildMonth(ref: Date): void {
    const year = ref.getFullYear();
    const month = ref.getMonth();
    const firstOfMonth = new Date(year, month, 1);
    const lastOfMonth = new Date(year, month + 1, 0);

    // find Monday of the week containing firstOfMonth
    const dayOfWeek = (firstOfMonth.getDay() + 6) % 7; // convert Sun=0..Sat=6 -> Mon=0..Sun=6
    const start = this.addDays(firstOfMonth, -dayOfWeek);

    // find Sunday of lastOfMonth's week (so we include full last week)
    const lastDow = (lastOfMonth.getDay() + 6) % 7;
    const daysToAdd = 6 - lastDow;
    const end = this.addDays(lastOfMonth, daysToAdd);

    const allDays: Date[] = [];
    for (let d = new Date(start); d <= end; d = this.addDays(d, 1)) {
      allDays.push(new Date(d));
    }

    // chunk into weeks
    const weeks: Date[][] = [];
    for (let i = 0; i < allDays.length; i += 7) {
      weeks.push(allDays.slice(i, i + 7));
    }
    this.monthWeeks = weeks;
  }

  // Build week view starting Monday containing current
  buildWeek(ref: Date): void {
    const curr = new Date(ref.getFullYear(), ref.getMonth(), ref.getDate());
    const dow = (curr.getDay() + 6) % 7; // Monday = 0
    const start = this.addDays(curr, -dow);
    const days: Date[] = [];
    for (let i = 0; i < 7; i++) days.push(this.addDays(start, i));
    this.weekDays = days;
  }

  addDays(date: Date, days: number): Date {
    const d = new Date(date);
    d.setDate(d.getDate() + days);
    return d;
  }

  monthLabel(): string {
    return this.current.toLocaleString(undefined, { month: 'long', year: 'numeric' });
  }

  // return events for a day
  eventsOn(day: Date): CalendarItem[] {
    return this.events
      .filter(e =>
        e.start.getFullYear() === day.getFullYear() &&
        e.start.getMonth() === day.getMonth() &&
        e.start.getDate() === day.getDate()
      )
      .sort((a, b) => a.start.getTime() - b.start.getTime());
  }
  
  // helper for checking if day belongs to current month
  isOtherMonth(day: Date): boolean {
    return day.getMonth() !== this.current.getMonth();
  }

  // helper for responsive formatting (not necessary but handy)
  isToday(day: Date): boolean {
    return day.toDateString() === new Date().toDateString();
  }


}