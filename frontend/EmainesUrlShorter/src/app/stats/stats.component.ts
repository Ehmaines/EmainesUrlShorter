import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UrlShorterService } from '../urlShorter.service';

@Component({
    selector: 'app-stats',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './stats.component.html',
})
export class StatsComponent implements OnInit {
    stats: Array<{ originalUrl: string; shortUrl: string; clicks: number }> = [];
    isLoading: boolean = true;
    errorMessage: string | null = null;

    constructor(
        private urlShorterService: UrlShorterService,
        private cdr: ChangeDetectorRef
    ) { }

    ngOnInit() {
        this.loadStats();
    }

    loadStats() {
        this.isLoading = true;
        this.errorMessage = null;

        this.urlShorterService.getUserLinks().subscribe({
            next: (data) => {
                console.log('Raw stats data:', data);
                this.stats = (data ?? []).map((item: any) => ({
                    originalUrl: item.originalUrl ?? item.OriginalUrl ?? '',
                    shortUrl: item.shortUrl ?? item.ShortUrl ?? '',
                    clicks: item.totalClicks ?? 0
                }));
                this.isLoading = false;
                // Ensure UI updates in zoneless mode.
                this.cdr.detectChanges();
                console.log('Stats loaded:', this.stats);
            },
            error: (error) => {
                console.error('Error loading stats:', error);
                this.errorMessage = 'Failed to load statistics.';
                this.isLoading = false;
            }
        });
    }
}
