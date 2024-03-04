import { Component, OnInit, Inject } from '@angular/core';
import {
  MatDialog,
  MAT_DIALOG_DATA,
  MatDialogRef,
  MatDialogTitle,
  MatDialogContent,
  MatDialogActions,
  MatDialogClose,
} from '@angular/material/dialog';
import { AppService } from '../app.service';
import { DialogAlertComponent } from './dialog.alert.component';

export interface DialogAlertData {
  title: string;
  playerName: string;
}

@Component({
  templateUrl: 'dialog.prompt.component.html'
})
export class DialogPromptComponent {
  constructor(public dialogRef: MatDialogRef<DialogPromptComponent>,
    @Inject(MAT_DIALOG_DATA) public data: DialogAlertData,
    private dialog: MatDialog,
    private appService: AppService) {
  }

  showErrorMessage(title: string, content: string) {
    this.dialog.open(DialogAlertComponent, {
      data: {
        title,
        content
      }
    });
  }

  handleClickOk() {
    if (!this.data.playerName?.trim()) {
      alert('Please put your name');
    } else {
      this.appService.registerName(this.data.playerName).subscribe((data: any) => {
        if (data.result == false) {
          if (data.isDuplicated) {
            this.showErrorMessage('Player Name Error', 'Player Name has been duplicated, please input another name');
          } else {
            this.showErrorMessage('Players Exceed', 'Players exceed, Please try to sign in when the game has ended.');
          }
        } else {
          if (data.isAdmin) {
            this.dialogRef.close(true);
          } else {
            this.dialogRef.close(false);
          }
        }
      }, (error: any) => {
        this.showErrorMessage('Error Register Name', error.message);
      });
    }
  }
}
