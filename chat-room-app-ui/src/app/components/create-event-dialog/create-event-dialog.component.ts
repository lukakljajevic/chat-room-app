import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';

import { EventType, CreateEventRequest } from '../../types/event.types';

@Component({
  selector: 'app-create-event-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule
  ],
  templateUrl: './create-event-dialog.component.html',
  styleUrls: ['./create-event-dialog.component.scss']
})
export class CreateEventDialogComponent {
  eventForm: FormGroup;
  eventTypes: EventType[] = ['enterroom', 'leaveroom', 'comment', 'highfive'];

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<CreateEventDialogComponent>
  ) {
    this.eventForm = this.fb.group({
      eventType: ['', Validators.required],
      username: ['', Validators.required],
      commentText: [''],
      recipient: ['']
    });

    this.eventForm.get('eventType')?.valueChanges.subscribe(type => {
      const commentTextControl = this.eventForm.get('commentText');
      const recipientControl = this.eventForm.get('recipient');

      if (type === 'comment') {
        commentTextControl?.setValidators([Validators.required]);
        recipientControl?.clearValidators();
      } else if (type === 'highfive') {
        commentTextControl?.clearValidators();
        recipientControl?.setValidators([Validators.required]);
      } else {
        commentTextControl?.clearValidators();
        recipientControl?.clearValidators();
      }
    });
  }

  onSubmit(): void {
    if (this.eventForm.valid) {
      const event: CreateEventRequest = {
        eventType: this.eventForm.value.eventType,
        username: this.eventForm.value.username,
        commentText: this.eventForm.value.commentText,
        recipient: this.eventForm.value.recipient
      };
      this.dialogRef.close(event);
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  getEventTypeDisplay(type: string): string {
    switch (type.toLowerCase()) {
      case 'enterroom':
        return 'Enter Room';
      case 'leaveroom':
        return 'Leave Room';
      case 'comment':
        return 'Comment';
      case 'highfive':
        return 'High Five';
      default:
        return type;
    }
  }
} 