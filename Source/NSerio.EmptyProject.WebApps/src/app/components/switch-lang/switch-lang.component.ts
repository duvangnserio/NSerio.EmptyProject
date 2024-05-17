import { CommonModule } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  OnInit,
  inject,
} from '@angular/core';
import { FormsModule } from '@angular/forms';

import { DropdownModule } from 'primeng/dropdown';

import { LanguageModel } from '@app/models/language.model';
import { LanguageStore } from '@app/store';

@Component({
  selector: 'app-switch-lang',
  standalone: true,
  imports: [CommonModule, FormsModule, DropdownModule],
  templateUrl: './switch-lang.component.html',
  styleUrl: './switch-lang.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SwitchLangComponent implements OnInit {
  //#region [Injects]

  private readonly languageStore = inject(LanguageStore);

  //#endregion [Injects]

  //#region [Properties]

  public readonly languages = this.languageStore.languages;
  public readonly selectedLanguage = this.languageStore.selectedLanguage;

  //#endregion [Properties]

  //#region [Lifecycle]

  ngOnInit(): void {
    this.languageStore.setDefaultLanguage();
  }

  //#endregion [Lifecycle]

  //#region [Public Functions]

  public switchAppLanguage(language: LanguageModel): void {
    this.languageStore.changeAppLanguage(language);
  }

  //#endregion [Public Functions]
}
