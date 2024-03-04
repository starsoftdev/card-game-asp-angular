import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CardI } from './Interfaces/Card.interface';

@Injectable({
  providedIn: 'root'
})
export class AppService {

  constructor(private http: HttpClient) { }

  getGameState(): Observable<any> {
    return this.http.get('get-game-state');
  }

  compareCards(cardA: CardI, cardB: CardI): Observable<any> {
    return this.http.post('compare-cards', {
      CardA: {
        FileName: cardA.fileName,
        Value: cardA.value
      },
      CardB: {
        FileName: cardB.fileName,
        Value: cardB.value
      }
    });
  }

  registerName(playerName: string): Observable<any> {
    return this.http.post('register-player-name', {
      PlayerName: playerName
    });
  }

  checkGameState() {
    return this.http.get('check-game-state');
  }

  resetGameState() {
    return this.http.get('reset-game-state');
  }

  removeEliminateCard(cardValue: string) {
    return this.http.post('remove-eliminate-card', {
      CardValue: cardValue
    });
  }

  removeResetGameStatus() {
    return this.http.get('remove-reset-game-status');
  }

}
