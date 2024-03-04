import { Component, OnInit, Inject } from '@angular/core';
import {
  MatDialog,
} from '@angular/material/dialog';
import { Router } from '@angular/router';

import { DialogPromptComponent } from '../components/dialog.prompt.component';
import { AppService } from '../app.service';

@Component({
  template: ''
})
export class GenerateGamePageComponent implements OnInit {
  public title: string = '';
  public playerName: string = '';

  constructor(public dialog: MatDialog, private appService: AppService, private router: Router) {
    this.title = 'Generate Game';
  }

  ngOnInit(): void {
    this.appService.checkGameState().subscribe(isSignedIn => {
      if (isSignedIn) {
        this.router.navigate(['/']);
      } else {
        const dialogRef = this.dialog.open(DialogPromptComponent, {
          data: {
            title: 'Register name'
          }
        });
        dialogRef.disableClose = true;

        dialogRef.afterClosed().subscribe(result => {
          this.router.navigate(['/']);
        });
      }
    }, () => {
      alert(`We can't load the state of game, please try again`);
    });
  }
}
