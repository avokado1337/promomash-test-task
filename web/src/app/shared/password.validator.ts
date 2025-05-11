// password.validator.ts
import { AbstractControl, ValidationErrors, ValidatorFn, FormGroup } from '@angular/forms';

export class PasswordValidator {
    static strong(control: AbstractControl): ValidationErrors | null {
        const value = control.value;
        if (!value) return null;

        const hasUpperCase = /[A-Z]/.test(value);
        const hasNumber = /\d/.test(value);
        const hasMinLength = value.length >= 8;

        return hasUpperCase && hasNumber && hasMinLength ? null : { weakPassword: true };
    }

    static match(passwordKey: string, confirmPasswordKey: string): ValidatorFn {
        return (group: AbstractControl): ValidationErrors | null => {
            const formGroup = group as FormGroup;
            const password = formGroup.get(passwordKey)?.value;
            const confirmPassword = formGroup.get(confirmPasswordKey)?.value;
            if (password === confirmPassword) {
                return null;
            }
            return { passwordsNotMatching: true };
        };
    }

}
