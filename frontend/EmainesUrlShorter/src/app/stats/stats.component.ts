import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UrlShorterService } from '../urlShorter.service';

@Component({
    selector: 'app-stats',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './stats.component.html',
})
export class StatsComponent implements OnInit {
    stats: any[] = [];
    isLoading: boolean = true;
    errorMessage: string | null = null;

    constructor(private urlShorterService: UrlShorterService) { }

    ngOnInit() {
        this.loadStats();
    }

    loadStats() {
        this.isLoading = true;
        this.errorMessage = null;

        this.urlShorterService.getUserLinks().subscribe({
            next: (data) => {
                this.stats = data;
                this.isLoading = false;
            },
            error: (error) => {
                console.error('Error loading stats:', error);
                this.errorMessage = 'Failed to load statistics.';
                this.isLoading = false;
            }
        });
    }
}
