import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { AppModule } from './app/app.module';
import { LOCALE_ID } from '@angular/core';

platformBrowserDynamic([{ provide: LOCALE_ID, useValue: $localize`:@@locale:en` }]).bootstrapModule(AppModule)
  .catch(err => console.error(err));
