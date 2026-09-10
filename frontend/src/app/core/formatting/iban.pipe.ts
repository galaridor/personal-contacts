import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'iban' })
export class IbanPipe implements PipeTransform {
  transform(value: string | null | undefined): string {
    return value ? (value.match(/.{1,4}/g) ?? []).join(' ') : '';
  }
}
