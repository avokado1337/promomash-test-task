import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatStepperModule } from '@angular/material/stepper';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatStepper } from '@angular/material/stepper';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { RegistrationService } from './registration.service';
import { PasswordValidator } from '../shared/password.validator';
import { Segment } from '../models/segment.model'
import { System } from '../models/system.model'
import { Planet } from '../models/planet.model'
import { ResponseDto } from '../dtos/response.dto';
import { Guardsman } from '../models/guardsman.model';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';


@Component({
    selector: 'app-registration',
    templateUrl: './registration.component.html',
    styleUrls: ['./registration.component.scss'],
    imports: [
        MatStepperModule,
        MatButtonModule,
        MatInputModule,
        MatFormFieldModule,
        MatSelectModule,
        FormsModule,
        ReactiveFormsModule,
        MatIconModule,
        MatCheckboxModule,
        MatStepper,
        CommonModule
    ]
})
export class RegistrationComponent implements OnInit {
    registrationForm: FormGroup;
    currentStep = 1;
    segments: Segment[] = [];
    systems: System[] = [];
    planets: Planet[] = [];
    isLoading = false;
    step1Submitted = false;
    step2Submitted = false;
    step1Errors: { [key: string]: boolean } = {};
    step2Errors: { [key: string]: boolean } = {};
    constructor(
        private fb: FormBuilder,
        private registrationService: RegistrationService
    ) {
        this.registrationForm = this.fb.group({
            step1: this.fb.group({
                voxAddress: ['', [Validators.required, Validators.email]],
                password: ['', [Validators.required, PasswordValidator.strong]],
                confirmPassword: ['', Validators.required],
                agree: [false, Validators.requiredTrue]
            }, {
                validator: PasswordValidator.match('password', 'confirmPassword')
            }),
            step2: this.fb.group({
                segmentId: ['', Validators.required],
                systemId: ['', Validators.required],
                planetId: ['', Validators.required]
            })
        });
    }

    ngOnInit(): void {
        this.loadSegments();
    }

    checkPasswords(group: FormGroup) {
        const pass = group.get('password')?.value;
        const confirmPass = group.get('confirmPassword')?.value;
        return pass === confirmPass ? null : { notSame: true };
    }

    get step1() {
        return this.registrationForm.get('step1') as FormGroup;
    }

    get step2() {
        return this.registrationForm.get('step2') as FormGroup;
    }

    loadSegments(): void {
        this.isLoading = true;
        this.registrationService.getSegments().subscribe({
            next: (segments) => {
                this.segments = segments;
                this.isLoading = false;
            },
            error: () => {
                this.isLoading = false;
            }
        });
    }

    onSegmentChange(): void {
        const segmentId = this.step2.get('segmentId')?.value;
        this.step2.get('systemId')?.reset();
        this.step2.get('planetId')?.reset();
        this.systems = [];
        this.planets = [];

        if (segmentId) {
            this.isLoading = true;
            this.registrationService.getSystems(segmentId).subscribe({
                next: (systems) => {
                    this.systems = systems;
                    this.isLoading = false;
                },
                error: () => {
                    this.isLoading = false;
                }
            });
        }
    }

    onSystemChange(): void {
        const systemId = this.step2.get('systemId')?.value;
        this.step2.get('planetId')?.reset();
        this.planets = [];

        if (systemId) {
            this.isLoading = true;
            this.registrationService.getPlanets(systemId).subscribe({
                next: (planets) => {
                    this.planets = planets;
                    this.isLoading = false;
                },
                error: () => {
                    this.isLoading = false;
                }
            });
        }
    }

    onSubmitStep1(stepper: MatStepper): void {
        this.step1Submitted = true;
        Object.keys(this.step1.controls).forEach(field => {
            this.step1Errors[field] = this.step1.get(field)?.invalid ?? false;
        });

        if (this.step1.valid) {
            stepper.next();
        }
    }

    onSubmitStep2() {
        this.step2Submitted = true;
        Object.keys(this.step2.controls).forEach(field => {
            this.step2Errors[field] = this.step2.get(field)?.invalid ?? false;
        });

        if (this.step2.valid) {
            this.isLoading = true;
            const formData: Guardsman = {
                voxAddress: this.step1.get('voxAddress')?.value,
                password: this.step1.get('password')?.value,
                segmentId: this.step2.get('segmentId')?.value,
                systemId: this.step2.get('systemId')?.value,
                planetId: this.step2.get('planetId')?.value
            };

            this.registrationService.register(formData).subscribe({
                next: (response) => {
                    this.isLoading = false;
                    alert(response.message || 'Регистрация прошла успешно!');
                },
                error: (error) => {
                    this.isLoading = false;

                    const err = error.error as ResponseDto;
                    alert(err.message || 'Ошибка регистрации. Попробуйте позже.');
                }
            });
        }
    }

    shouldShowError(step: 'step1' | 'step2', fieldName: string): boolean {
        if (step === 'step1') {
            return this.step1Submitted && this.step1Errors[fieldName];
        } else {
            return this.step2Submitted && this.step2Errors[fieldName];
        }
    }

    prevStep(stepper: MatStepper): void {
        stepper.previous();
    }

}