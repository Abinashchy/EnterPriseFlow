import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { TaskCommentService } from '../../core/services/task-comment.service';
import { TaskComment } from '../../core/models/task-comment.model';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-task-details',
  imports: [DatePipe, FormsModule],
  templateUrl: './task-details.html',
  styleUrl: './task-details.css',
})
export class TaskDetails implements OnInit {
  task: any = null;
  comments: TaskComment[] = [];
  newComment = '';
  editingCommentId: number | null = null;
  editingCommentText = '';
  currentUserId: number = 0;

  constructor(
    private router: Router,
    private commentService: TaskCommentService,
    private authService: AuthService,
    private cdRef: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.task = history.state?.task || null;
    if (!this.task) {
      this.router.navigate(['/tasks']);
      return;
    }
    this.currentUserId = this.authService.getCurrentUser()?.userId ?? 0;
    this.loadComments();
  }

  loadComments(): void {
    this.commentService.getComments(this.task.id).subscribe({
      next: (comments) => { this.comments = comments; this.cdRef.detectChanges(); },
      error: (err) => console.error('Failed to load comments', err)
    });
  }

  addComment(): void {
    if (!this.newComment.trim()) return;
    this.commentService.addComment({
      taskId: this.task.id,
      userId: this.currentUserId,
      comment: this.newComment.trim()
    }).subscribe({
      next: () => {
        this.newComment = '';
        this.loadComments();
      },
      error: (err) => console.error('Failed to add comment', err)
    });
  }

  startEdit(comment: TaskComment): void {
    this.editingCommentId = comment.id;
    this.editingCommentText = comment.comment;
  }

  cancelEdit(): void {
    this.editingCommentId = null;
    this.editingCommentText = '';
  }

  saveEdit(commentId: number): void {
    if (!this.editingCommentText.trim()) return;
    this.commentService.updateComment(commentId, {
      comment: this.editingCommentText.trim(),
      userId: this.currentUserId
    }).subscribe({
      next: () => {
        this.editingCommentId = null;
        this.editingCommentText = '';
        this.loadComments();
      },
      error: (err) => console.error('Failed to update comment', err)
    });
  }

  deleteComment(commentId: number): void {
    this.commentService.deleteComment(commentId, this.currentUserId).subscribe({
      next: () => this.loadComments(),
      error: (err) => console.error('Failed to delete comment', err)
    });
  }

  goBack(): void {
    this.router.navigate(['/tasks']);
  }
}
