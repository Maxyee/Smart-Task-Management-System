import { Component, Input, Output, EventEmitter, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AiService } from '../../../core/services/ai.service';
import { NotificationService } from '../../../core/services/notification.service';
import { TaskImprovementRequest, TaskImprovementResponse, ImprovementOptions } from '../../../core/models/ai.model';
import { Subject, takeUntil, finalize } from 'rxjs';

@Component({
    selector: 'app-task-ai-improvement',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './task-ai-improvement.component.html',
    styleUrls: ['./task-ai-improvement.component.css']
})
export class TaskAiImprovementComponent implements OnInit, OnDestroy {
    @Input() taskId: string = '';
    @Input() title: string = '';
    @Input() description: string = '';
    @Input() isOpen: boolean = false;
    @Input() currentStatus: string = 'To Do';

    @Output() close = new EventEmitter<void>();
    @Output() apply = new EventEmitter<{ title: string; description: string }>();

    // AI Improvement State
    isImproving = false;
    improvementResult: TaskImprovementResponse | null = null;
    showPreview = false;
    activeStep: 'options' | 'improving' | 'preview' = 'options';

    // AI Options
    aiOptions: ImprovementOptions = {
        correctGrammar: true,
        improveClarity: true,
        makeProfessional: true,
        expandDescription: true,
        makeActionable: true,
        maxLength: 500,
        tone: 'Professional',
        language: 'English'
    };

    toneOptions = ['Professional', 'Formal', 'Friendly', 'Technical', 'Concise', 'Detailed'];
    languageOptions = ['English', 'Spanish', 'French', 'German', 'Chinese', 'Japanese'];

    // Improvement History
    improvementHistory: TaskImprovementResponse[] = [];
    showHistory = false;

    private destroy$ = new Subject<void>();

    constructor(
        private aiService: AiService,
        private notificationService: NotificationService
    ) { }

    ngOnInit(): void {
        // Load improvement history from localStorage if available
        const savedHistory = localStorage.getItem(`ai_history_${this.taskId}`);
        if (savedHistory) {
            try {
                this.improvementHistory = JSON.parse(savedHistory);
            } catch (e) {
                this.improvementHistory = [];
            }
        }
    }

    ngOnDestroy(): void {
        this.destroy$.next();
        this.destroy$.complete();
    }

    improveTask(): void {
        if (!this.title && !this.description) {
            this.notificationService.warning('Task title or description is required');
            return;
        }

        this.isImproving = true;
        this.activeStep = 'improving';

        const request: TaskImprovementRequest = {
            originalTitle: this.title,
            originalDescription: this.description,
            additionalContext: `Task Status: ${this.currentStatus}`,
            options: this.aiOptions
        };

        this.aiService.improveTaskDescription(request)
            .pipe(
                takeUntil(this.destroy$),
                finalize(() => {
                    this.isImproving = false;
                })
            )
            .subscribe({
                next: (response) => {
                    this.improvementResult = response;
                    this.activeStep = 'preview';
                    this.showPreview = true;
                    this.notificationService.success('AI improvement generated successfully');

                    // Save to history
                    this.improvementHistory.push(response);
                    if (this.improvementHistory.length > 10) {
                        this.improvementHistory = this.improvementHistory.slice(-10);
                    }
                    localStorage.setItem(`ai_history_${this.taskId}`, JSON.stringify(this.improvementHistory));
                },
                error: (error) => {
                    this.activeStep = 'options';
                    this.notificationService.error(error.message || 'Failed to improve task description');
                }
            });
    }

    applyImprovement(): void {
        if (!this.improvementResult) return;

        this.apply.emit({
            title: this.improvementResult.improvedTitle,
            description: this.improvementResult.improvedDescription
        });

        this.notificationService.success('AI improvements applied successfully');
        this.closePanel();
    }

    closePanel(): void {
        this.isOpen = false;
        this.showPreview = false;
        this.activeStep = 'options';
        this.improvementResult = null;
        this.close.emit();
    }

    resetToOptions(): void {
        this.activeStep = 'options';
        this.showPreview = false;
        this.improvementResult = null;
    }

    loadHistoryItem(item: TaskImprovementResponse): void {
        this.improvementResult = item;
        this.activeStep = 'preview';
        this.showPreview = true;
        this.showHistory = false;
    }

    getCharacterCount(text: string): number {
        return text?.length || 0;
    }

    getRemainingCharacters(text: string): number {
        return this.aiOptions.maxLength - this.getCharacterCount(text);
    }

    isOverLength(text: string): boolean {
        return this.getCharacterCount(text) > this.aiOptions.maxLength;
    }

    getImprovementSummary(): string {
        if (!this.improvementResult) return '';
        const improvements = [];
        const originalLength = this.improvementResult.metadata?.originalLength || 0;
        const improvedLength = this.improvementResult.metadata?.improvedLength || 0;

        if (improvedLength > originalLength) {
            improvements.push(`Expanded by ${improvedLength - originalLength} characters`);
        }
        if (this.improvementResult.keyPoints?.length) {
            improvements.push(`${this.improvementResult.keyPoints.length} key points identified`);
        }
        if (this.improvementResult.suggestedActions?.length) {
            improvements.push(`${this.improvementResult.suggestedActions.length} suggested actions`);
        }

        return improvements.join(' • ') || 'Improved description';
    }

    copyToClipboard(text: string): void {
        navigator.clipboard.writeText(text).then(() => {
            this.notificationService.success('Copied to clipboard');
        }).catch(() => {
            this.notificationService.error('Failed to copy');
        });
    }

    // Helper for template
    Math = Math;
    Object = Object;
}