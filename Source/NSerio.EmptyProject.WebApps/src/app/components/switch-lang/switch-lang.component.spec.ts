import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SwitchLangComponent } from './switch-lang.component';

describe('SwitchLanguageComponent', () => {
  let component: SwitchLangComponent;
  let fixture: ComponentFixture<SwitchLangComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SwitchLangComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(SwitchLangComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
