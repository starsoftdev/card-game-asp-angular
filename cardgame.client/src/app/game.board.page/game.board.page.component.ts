import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import {
  MatDialog,
} from '@angular/material/dialog';
import { Router } from '@angular/router';

import { CardI } from '../Interfaces/Card.interface';
import { AppService } from '../app.service';
import { DialogAlertComponent } from '../components/dialog.alert.component';
import { EventSourceServiceService } from '../event-source-service.service';


@Component({
  templateUrl: 'game.board.page.component.html',
  styleUrls: ['game.board.page.component.css']
})
export class GameBoardPageComponent implements OnInit, OnDestroy {
  public allCards: Array<CardI | null> = [];
  public score: string = "";

  public cardBackImgSrc: string = 'assets/cards/back.jpg';

  public activeCardData: CardI | null = null;

  private isComparingCards: boolean = false;

  private isResetGame: boolean = false;

  private signedInUser: string = '';

  public isActivatedGame: boolean = false;

  constructor(private appService: AppService, public dialog: MatDialog, private router: Router, private eventSourceService: EventSourceServiceService, private cdr: ChangeDetectorRef) { }

  ngOnInit() {
    this.loadCard();
  }
  // Events
  bringToFront(cardData: CardI, domCard: HTMLElement): void {
    if (this.isComparingCards === true) {
      alert('You are working on comparing cards');
      return;
    }
    if (this.isActivatedGame === false) {
      this.showMessage('Require Activate Game', 'You are not activated user, please wait until the activated user has ended');
      return;
    }
    if (this.activeCardData) {
      if (this.activeCardData.fileName === cardData.fileName) {
        this.activeCardData = null;
        domCard.setAttribute('src', this.cardBackImgSrc);
      } else {
        domCard.setAttribute('src', `assets/cards/${cardData.fileName}.jpg`);
        this.isComparingCards = true;
        this.appService.compareCards(this.activeCardData, cardData).subscribe(data => {
          if (data === false) {
            document.getElementById(`${this.activeCardData?.fileName}`)?.setAttribute('src', this.cardBackImgSrc);
            setTimeout(() => {
              domCard.setAttribute('src', this.cardBackImgSrc);
            }, 500);
            this.activeCardData = null;
            this.showMessage('Incorrect Cards', 'Two cards do not match. Activation is transferred to another user. You have to wait until another user fails.');
          } else {
            this.activeCardData = null;
          }
          this.isComparingCards = false;
        }, error => {
          this.isComparingCards = false;

          this.showMessage('Error Compare Cards', error.error);

          document.getElementById(`${this.activeCardData?.fileName}`)?.setAttribute('src', this.cardBackImgSrc);
          setTimeout(() => {
            domCard.setAttribute('src', this.cardBackImgSrc);
          }, 500);
          this.activeCardData = null;
        });
      }
    } else {
      this.activeCardData = cardData;
      domCard.setAttribute('src', `assets/cards/${cardData.fileName}.jpg`);
    }
  }

  showMessage(title: string, content: string) {
    this.dialog.open(DialogAlertComponent, {
      data: {
        title,
        content
      }
    });
  }

  loadCard(loadSSE: boolean = true) {
    this.appService.getGameState().subscribe(stateData => {
      const allCards = stateData.allCards;
      this.signedInUser = stateData.signedInUser;

      if (allCards.length === 0) {
        this.router.navigate(['generate-game-page']);
      } else {
        this.allCards = allCards;
        this.cdr.detectChanges();

        if (loadSSE) {
          this.eventSourceService.startSse();

          this.eventSourceService.getMessageStream().subscribe({
            next: (data: any) => {
              if (data.type == 'removeCard') {
                for (let i = 0; i < this.allCards.length; i++) {
                  if (this.allCards[i]?.value === data?.cardValue) {
                    this.allCards[i] = null;
                  }
                }
                this.cdr.detectChanges();
                this.appService.removeEliminateCard(data.cardValue).subscribe(() => {
                  if (!document.querySelector('.card-container')?.querySelectorAll('img').length) {
                    alert(`The game is ended, the result is ${this.score}`);
                  }
                });
              } else if (data.type == 'resetGame') {
                if (this.isResetGame === false) {
                  this.isResetGame = true;
                  this.eventSourceService.stopSse();
                  this.appService.removeResetGameStatus().subscribe(() => {
                    window.location.reload();
                  });
                }
                return;
              } else if (data.type == 'setScore') {
                let key = '';
                let scoreStr = '';
                for (key in data.score) {
                  scoreStr += `${key} is ${data.score[key]}, `;
                }

                scoreStr = scoreStr.substr(0, scoreStr.lastIndexOf(', '));

                this.score = scoreStr;
                this.isActivatedGame = data.isActiveGame;
                this.cdr.detectChanges();
              }

              console.log('Received event:', data);
              // Process the event data
            },
            error: err => {
              console.error('Error occurred:', err);
            }
          });
        }
      }
    });
  }

  handleResetGame() {
    this.appService.resetGameState().subscribe(() => {
      this.showMessage('Success', 'Game has been successfully reset');
    }, error => {
      this.showMessage('Error Reset Game', error.message || error.error);
    });
  }

  ngOnDestroy(): void {
    this.eventSourceService.stopSse();
  }
}
