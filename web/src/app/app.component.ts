import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RegistrationComponent } from './components/registration.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RegistrationComponent],
  template: `
    <main style="padding: 20px; max-width: 800px; margin: 0 auto;">
      <app-registration></app-registration>
    </main>
  `,
  styles: []
})
export class AppComponent {
  title = 'Имперская Гвардия';
}