export interface GetTasks{
  id: number;
  title: string;
  description: string;
  status: string;
  priority: string;
  projectId: number;
  assignedTo: number;
  createdBy: number;
  createdAt: string;
}

