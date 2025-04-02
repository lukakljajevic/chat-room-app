import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { CommonModule } from '@angular/common';

import { EventService } from '../../services/event.service';
import { DetailedChatEvent, AggregatedChatEvent, Granularity } from '../../types/event.types';
import { CreateEventDialogComponent } from '../create-event-dialog/create-event-dialog.component';

@Component({
  selector: 'app-event-list',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './event-list.component.html',
  styleUrls: ['./event-list.component.scss']
})
export class EventListComponent implements OnInit {
  filterForm: FormGroup;
  events: DetailedChatEvent[] = [];
  aggregatedEvents: AggregatedChatEvent[] = [];
  granularities: Granularity[] = ['minute', 'hour', 'day', 'month'];
  isLoading = false;
  dateRangeError = false;

  constructor(
    private fb: FormBuilder,
    private eventService: EventService,
    private dialog: MatDialog
  ) {
    this.filterForm = this.fb.group({
      fromDate: [null],
      toDate: [null],
      granularity: ['minute']
    }, { validators: this.dateRangeValidator() });
  }

  ngOnInit(): void {
    this.loadEvents();
  }

  private dateRangeValidator() {
    return (formGroup: FormGroup) => {
      const fromDate = formGroup.get('fromDate')?.value;
      const toDate = formGroup.get('toDate')?.value;
      
      if (fromDate && toDate && fromDate > toDate) {
        this.dateRangeError = true;
        return { dateRange: true };
      }
      
      this.dateRangeError = false;
      return null;
    };
  }

  private formatDateForApi(date: Date | null, isEndDate: boolean = false): string | null {
    if (!date) return null;
    
    if (isEndDate) {
      // For end date, set to end of day (23:59:59.999)
      const endOfDay = new Date(date);
      endOfDay.setHours(23, 59, 59, 999);
      return endOfDay.toISOString();
    } else {
      // For start date, set to beginning of day (00:00:00)
      const startOfDay = new Date(date);
      startOfDay.setHours(0, 0, 0, 0);
      return startOfDay.toISOString();
    }
  }

  loadEvents(): void {
    if (this.filterForm.invalid) {
      return;
    }
    
    this.isLoading = true;
    const { fromDate, toDate, granularity } = this.filterForm.value;
    
    const fromDateString = this.formatDateForApi(fromDate, false);
    const toDateString = this.formatDateForApi(toDate, true);
    
    if (granularity === 'minute') {
      this.eventService.getDetailedEvents(fromDateString, toDateString).subscribe({
        next: (events) => {
          this.events = events;
          this.isLoading = false;
        },
        error: (error) => {
          console.error('Error loading events:', error);
          this.isLoading = false;
        }
      });
    } else {
      this.eventService.getAggregatedEvents(fromDateString, toDateString, granularity).subscribe({
        next: (events) => {
          this.aggregatedEvents = events;
          this.isLoading = false;
        },
        error: (error) => {
          console.error('Error loading aggregated events:', error);
          this.isLoading = false;
        }
      });
    }
  }

  resetFilters(): void {
    this.filterForm.reset({
      fromDate: null,
      toDate: null,
      granularity: 'minute'
    });
    this.loadEvents();
  }

  openCreateEventDialog(): void {
    const dialogRef = this.dialog.open(CreateEventDialogComponent, {
      width: '400px'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.eventService.createEvent(result).subscribe({
          next: () => {
            this.loadEvents();
          },
          error: (error) => {
            console.error('Error creating event:', error);
          }
        });
      }
    });
  }

  formatTimestamp(timestamp: string, granularity?: Granularity): string {
    const date = new Date(timestamp);
    
    if (!granularity || granularity === 'minute') {
      return date.toLocaleString();
    }
    
    switch (granularity) {
      case 'hour':
        return date.toLocaleString(undefined, { 
          year: 'numeric',
          month: 'short',
          day: 'numeric',
          hour: '2-digit',
          minute: '2-digit'
        });
      case 'day':
        return date.toLocaleDateString(undefined, {
          year: 'numeric',
          month: 'short',
          day: 'numeric'
        });
      case 'month':
        return date.toLocaleDateString(undefined, {
          year: 'numeric',
          month: 'long'
        });
      default:
        return date.toLocaleString();
    }
  }

  getEventTypeDisplay(type: string): string {
    switch (type.toLowerCase()) {
      case 'enterroom':
        return 'enters the room';
      case 'leaveroom':
        return 'leaves the room';
      case 'comment':
        return 'comments';
      case 'highfive':
        return 'high-fives';
      default:
        return type;
    }
  }
} 