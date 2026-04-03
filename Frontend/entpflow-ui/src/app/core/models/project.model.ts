export interface GetProjects {
  id: number;
  name: string;
  description: string;
  createdBy: number;
  createdByName: string;
  departmentId: number | null;
  departmentName: string;
  createdAt: string;
}

export interface CreateProjectRequest {
  name: string;
  description: string;
  departmentId: number | null;

}

export interface UpdateProjectRequest {
  name: string;
  description: string;
  departmentId: number;
}

export interface ProjectMembers{
  projectId: number;
  projectName: string;
  userId: number;
  userName: string;
}

export interface CreateMembers{
  userId: number;
  projectId: number;
  // userName: string;
  // projectName: string;

}
