import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'home',
    loadComponent: () =>
      import('./views/home/home.view').then((m) => m.HomeView),
  },
  { path: '**', redirectTo: 'home', pathMatch: 'full' },
];
