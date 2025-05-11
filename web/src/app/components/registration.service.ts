import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Segment } from '../models/segment.model';
import { System } from '../models/system.model';
import { Planet } from '../models/planet.model';
import { Guardsman } from '../models/guardsman.model';
import { ResponseDto } from '../dtos/response.dto';
import { environment } from '../assets/environments/environment';

@Injectable({
    providedIn: 'root'
})
export class RegistrationService {
    private segmentApiUrl = environment.segmentApiUrl;
    private guardsmanApiUrl = environment.guardsmanApiUrl;

    constructor(private http: HttpClient) { }

    getSegments(): Observable<Segment[]> {
        return this.http.get<Segment[]>(`${this.segmentApiUrl}`);
    }

    getSystems(segmentId: number): Observable<System[]> {
        return this.http.get<System[]>(`${this.segmentApiUrl}/${segmentId}/systems`);
    }

    getPlanets(systemId: number): Observable<Planet[]> {
        return this.http.get<Planet[]>(`${this.segmentApiUrl}/systems/${systemId}/planets`);
    }

    register(guardsman: Guardsman): Observable<ResponseDto> {
        return this.http.post<ResponseDto>(`${this.guardsmanApiUrl}`, guardsman);
    }
}