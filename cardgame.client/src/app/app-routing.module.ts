import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { GenerateGamePageComponent } from './generate.game.page/generate.game.page.component';
import { GameBoardPageComponent } from './game.board.page/game.board.page.component';

const routes: Routes = [
  {
    title: 'Card Game Page',
    path: '',
    component: GameBoardPageComponent
  },
  {
    title: 'generate card game page',
    path: 'generate-game-page',
    component: GenerateGamePageComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
