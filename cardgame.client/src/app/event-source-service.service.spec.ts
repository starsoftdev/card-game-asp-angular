import { TestBed } from '@angular/core/testing';

import { EventSourceServiceService } from './event-source-service.service';

describe('EventSourceServiceService', () => {
  let service: EventSourceServiceService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(EventSourceServiceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
