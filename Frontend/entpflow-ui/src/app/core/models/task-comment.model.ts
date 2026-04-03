export interface TaskComment {
  id: number;
  taskId: number;
  userId: number;
  comment: string;
  createdAt: string;
  userName: string;
}

export interface CreateComment {
  taskId: number;
  userId: number;
  comment: string;
}

export interface UpdateComment {
  comment: string;
  userId: number;
}
