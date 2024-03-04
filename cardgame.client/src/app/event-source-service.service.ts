import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class EventSourceServiceService {
  private eventSource: EventSource | undefined;
  constructor() { }

  private messageSubject: Subject<string> = new Subject<string>();

  startSse(): void {
    this.eventSource = new EventSource('update-cards');

    this.eventSource.onmessage = (event) => {
      this.messageSubject.next(JSON.parse(event.data));
    };

    this.eventSource.onerror = (error) => {
      console.error('SSE error:', error);
    };
  }

  getMessageStream(): Observable<string> {
    return this.messageSubject.asObservable();
  }

  stopSse(): void {
    if (this.eventSource) {
      this.eventSource.close();
    }
  }
}
