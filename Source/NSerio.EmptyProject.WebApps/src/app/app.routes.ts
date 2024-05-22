import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'home',
    loadComponent: () =>
      import('./views/home/home.view').then((m) => m.HomeView),
  },
  { path: 'custom-calendar', loadComponent: () => import('./views/custom-calendar/custom-calendar.view').then(m => m.CustomCalendarView) }
];
