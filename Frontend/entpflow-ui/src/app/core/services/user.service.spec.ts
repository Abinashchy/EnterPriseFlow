import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { UserService } from './user.service';

describe('UserService', () => {
  let service: UserService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(UserService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('getUsers', () => {
    it('should GET users list', () => {
      const mockUsers = [{ userId: 1, name: 'Test', email: 'test@test.com' }];

      service.getUsers().subscribe((users) => {
        expect(users.length).toBe(1);
        expect(users[0].name).toBe('Test');
      });

      const req = httpMock.expectOne('/api/users');
      expect(req.request.method).toBe('GET');
      req.flush(mockUsers);
    });
  });

  describe('getRoles', () => {
    it('should GET roles list', () => {
      const mockRoles = [{ id: 1, name: 'Admin' }];

      service.getRoles().subscribe((roles) => {
        expect(roles.length).toBe(1);
      });

      const req = httpMock.expectOne('/api/roles');
      expect(req.request.method).toBe('GET');
      req.flush(mockRoles);
    });
  });

  describe('getDepartments', () => {
    it('should GET departments list', () => {
      const mockDepts = [{ departmentId: 1, name: 'IT' }];

      service.getDepartments().subscribe((depts) => {
        expect(depts.length).toBe(1);
      });

      const req = httpMock.expectOne('/api/department');
      expect(req.request.method).toBe('GET');
      req.flush(mockDepts);
    });
  });

  describe('createUser', () => {
    it('should POST new user', () => {
      const payload = { name: 'New', email: 'new@test.com', password: 'pass', roleId: 1, departmentId: 1, employeeId: 'E002' } as any;

      service.createUser(payload).subscribe();

      const req = httpMock.expectOne('/api/users');
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(payload);
      req.flush({});
    });
  });

  describe('updateUser', () => {
    it('should PUT updated user', () => {
      const payload = { name: 'Updated' } as any;

      service.updateUser(1, payload).subscribe();

      const req = httpMock.expectOne('/api/users/1');
      expect(req.request.method).toBe('PUT');
      req.flush({});
    });
  });

  describe('deleteUser', () => {
    it('should DELETE user by id', () => {
      service.deleteUser(1).subscribe();

      const req = httpMock.expectOne('/api/users/1');
      expect(req.request.method).toBe('DELETE');
      req.flush({});
    });
  });
});
