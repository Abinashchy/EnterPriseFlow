export interface CurrentUser {
  userId: number;
  employeeId: string;
  name: string;
  email: string;
  roleId: number;
  role: string;
  departmentId: number | null;
  department: string | null;
  isActive: boolean;
  createdAt: string;
}

export interface RoleOption {
  id: number;
  name: string;
}

export interface DepartmentOption {
  departmentId: number;
  name: string;
}

export interface UserRow {
  userId: number;
  employeeId: string;
  name: string;
  email: string;
  roleId: number;
  role: string;
  departmentId: number | null;
  department: string;
  isActive: boolean;
  createdAt: string;
}

export interface CreateUserRequest {
  employeeId: string;
  name: string;
  email: string;
  password: string;
  role: string;
  department: string;
}

export interface UpdateUserRequest {
  employeeId: string;
  name: string;
  email: string;
  role: string;
  department: string;
  isActive: boolean;
}
