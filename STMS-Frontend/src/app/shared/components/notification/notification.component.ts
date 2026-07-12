import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotificationService, Notification } from '../../../core/services/notification.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-notification',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './notification.component.html',
  styleUrls: ['./notification.component.css']
})
export class NotificationComponent implements OnInit, OnDestroy {
  notifications: (Notification & { id: number })[] = [];
  private subscription: Subscription | null = null;
  private idCounter = 0;

  constructor(private notificationService: NotificationService) {}

  ngOnInit(): void {
    this.subscription = this.notificationService.notifications$.subscribe(notification => {
      const id = this.idCounter++;
      this.notifications.push({ ...notification, id });
      
      if (notification.duration && notification.duration > 0) {
        setTimeout(() => {
          this.removeNotification(id);
        }, notification.duration);
      }
    });
  }

  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  removeNotification(id: number): void {
    this.notifications = this.notifications.filter(n => n.id !== id);
  }

  getNotificationClasses(type: string): string {
    const baseClasses = 'fixed top-4 right-4 z-50 p-4 rounded-lg shadow-lg max-w-md transform transition-all duration-500';
    const typeClasses = {
      success: 'bg-green-50 border-l-4 border-green-500 text-green-700',
      error: 'bg-red-50 border-l-4 border-red-500 text-red-700',
      warning: 'bg-yellow-50 border-l-4 border-yellow-500 text-yellow-700',
      info: 'bg-blue-50 border-l-4 border-blue-500 text-blue-700'
    };
    return `${baseClasses} ${typeClasses[type as keyof typeof typeClasses] || typeClasses.info}`;
  }

  getIcon(type: string): string {
    const icons = {
      success: '✓',
      error: '✕',
      warning: '⚠',
      info: 'ℹ'
    };
    return icons[type as keyof typeof icons] || icons.info;
  }
}