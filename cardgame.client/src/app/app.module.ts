import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppRoutingModule } from './app-routing.module';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import {
  MatDialog,
  MatDialogActions,
  MatDialogClose,
  MatDialogContent,
  MatDialogTitle,
  MatDialogModule,
} from '@angular/material/dialog';
import { TooltipPosition, MatTooltipModule } from '@angular/material/tooltip';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

import { AppComponent } from './app.component';
import { DialogAlertComponent } from './components/dialog.alert.component';
import { GenerateGamePageComponent } from './generate.game.page/generate.game.page.component';
import { GameBoardPageComponent } from './game.board.page/game.board.page.component';
import { DialogPromptComponent } from './components/dialog.prompt.component';


import { AppService } from './app.service';
import { EventSourceServiceService } from './event-source-service.service';

@NgModule({
  declarations: [
    AppComponent,
    DialogAlertComponent,
    GameBoardPageComponent,
    GenerateGamePageComponent,
    DialogPromptComponent
  ],
  imports: [
    BrowserAnimationsModule,
    HttpClientModule,
    AppRoutingModule,
    MatGridListModule,
    MatButtonModule,
    MatCardModule,
    MatDialogModule,
    MatDialogTitle,
    MatDialogContent,
    MatDialogActions,
    MatDialogClose,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
  ],
  providers: [
    AppService,
    EventSourceServiceService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
