import { Component, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UrlShorterService } from '../urlShorter.service';

@Component({
    selector: 'app-home',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './home.component.html',
})

export class HomeComponent {
    originalUrl: string = '';
    shortenedUrl: string | null = null;
    isLoading: boolean = false;
    errorMessage: string | null = null;
    copied: boolean = false;

    constructor(private urlShorterService: UrlShorterService, private cdr: ChangeDetectorRef) { }

    shortenUrl() {
        if (!this.originalUrl) return;

        this.isLoading = true;
        this.errorMessage = null;
        this.shortenedUrl = null;
        this.copied = false;

        this.urlShorterService.shortenUrl(this.originalUrl).subscribe({
            next: (response) => {
                console.log('API Response:', response);
                this.shortenedUrl = response.shortUrl || response.ShortUrl;
                this.isLoading = false;
                this.cdr.detectChanges();
            },
            error: (error) => {
                console.error('Error shortening URL:', error);
                this.errorMessage = 'Failed to shorten URL. Please try again.';
                this.isLoading = false;
                this.cdr.detectChanges();
            }
        });
    }

    copyToClipboard() {
        if (this.shortenedUrl) {
            navigator.clipboard.writeText(this.shortenedUrl).then(() => {
                this.copied = true;
                setTimeout(() => this.copied = false, 2000);
            });
            this.cdr.detectChanges();
        }
    }
}
