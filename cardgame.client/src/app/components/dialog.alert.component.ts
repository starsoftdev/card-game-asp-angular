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

export interface DialogAlertData {
  title: string;
  content: string;
}

@Component({
  selector: 'dialog-alert',
  templateUrl: 'dialog.alert.component.html'
})
export class DialogAlertComponent {
  public title: string = '';
  public content: string = '';

  constructor(public dialogRef: MatDialogRef<DialogAlertComponent>,
    @Inject(MAT_DIALOG_DATA) public data: DialogAlertData,) {
    this.title = data.title;
    this.content = data.content;
  }
}
