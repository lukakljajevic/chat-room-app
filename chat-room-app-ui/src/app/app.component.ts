import { Component } from '@angular/core';
import { EventListComponent } from './components/event-list/event-list.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [EventListComponent],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'chat-room-app-ui';
}
