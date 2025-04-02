export type EventType = 'enterroom' | 'leaveroom' | 'comment' | 'highfive';

export interface DetailedChatEvent {
  id: string;
  timestamp: string;
  eventType: EventType;
  username: string;
  commentText?: string;
  recipientUsername?: string;
}

export interface AggregatedChatEvent {
  periodStart: string;
  entersCount: number;
  leavesCount: number;
  commentsCount: number;
  highFivesCount: number;
  highFiveDetails: string[];
}

export interface CreateEventRequest {
  eventType: EventType;
  username: string;
  commentText?: string;
  recipient?: string;
}

export type Granularity = 'minute' | 'hour' | 'day' | 'month'; 