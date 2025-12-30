import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class UserService {
    private readonly STORAGE_KEY = 'guest_user_id';

    constructor() { }

    getUserId(): string {
        let userId = localStorage.getItem(this.STORAGE_KEY);

        if (!userId) {
            userId = crypto.randomUUID();
            localStorage.setItem(this.STORAGE_KEY, userId);
        }

        return userId;
    }
}
