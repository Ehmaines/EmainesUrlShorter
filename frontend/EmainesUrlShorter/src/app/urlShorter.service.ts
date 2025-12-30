import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UserService } from './user.service';

@Injectable({
    providedIn: 'root'
})
export class UrlShorterService {
    // Placeholder API URL - user needs to provide the real one
    private apiUrl = 'http://localhost:5266/api/ShortLink';

    constructor(
        private http: HttpClient,
        private userService: UserService
    ) { }

    shortenUrl(originalUrl: string): Observable<any> {
        const ownerId = this.userService.getUserId();
        return this.http.post<any>(this.apiUrl, { originalUrl, ownerId });
    }

    getUserLinks(): Observable<any[]> {
        const ownerId = this.userService.getUserId();
        // Assuming endpoint to get links by owner
        return this.http.get<any[]>(`${this.apiUrl}/by-owner/${ownerId}`);
    }
}
