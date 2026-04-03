import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { AuthService } from './auth.service';
import { PLATFORM_ID } from '@angular/core';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    localStorage.clear();
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        { provide: PLATFORM_ID, useValue: 'browser' },
      ],
    });
    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
    localStorage.clear();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('login', () => {
    it('should POST credentials and store token + user in localStorage', () => {
      const mockResponse = {
        token: 'test-token',
        expiresAt: '2026-12-31',
        user: { userId: 1, name: 'Test', email: 'test@test.com', role: 'Admin', employeeId: 'E001', roleId: 1, departmentId: null, department: null, isActive: true, createdAt: '' },
      };

      service.login({ email: 'test@test.com', password: 'pass' }).subscribe((res) => {
        expect(res.token).toBe('test-token');
        expect(localStorage.getItem('entpflow.token')).toBe('test-token');
        expect(localStorage.getItem('entpflow.user')).toBeTruthy();
      });

      const req = httpMock.expectOne('/api/auth/login');
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual({ email: 'test@test.com', password: 'pass' });
      req.flush(mockResponse);
    });
  });

  describe('logout', () => {
    it('should remove token and user from localStorage', () => {
      localStorage.setItem('entpflow.token', 'some-token');
      localStorage.setItem('entpflow.user', '{}');

      service.logout();

      expect(localStorage.getItem('entpflow.token')).toBeNull();
      expect(localStorage.getItem('entpflow.user')).toBeNull();
    });
  });

  describe('getToken', () => {
    it('should return token from localStorage', () => {
      localStorage.setItem('entpflow.token', 'my-token');
      expect(service.getToken()).toBe('my-token');
    });

    it('should return null if no token', () => {
      expect(service.getToken()).toBeNull();
    });
  });

  describe('getCurrentUser', () => {
    it('should return parsed user from localStorage', () => {
      const user = { userId: 1, name: 'Test', role: 'Admin' };
      localStorage.setItem('entpflow.user', JSON.stringify(user));
      const result = service.getCurrentUser();
      expect(result?.userId).toBe(1);
      expect(result?.name).toBe('Test');
    });

    it('should return null if no user stored', () => {
      expect(service.getCurrentUser()).toBeNull();
    });
  });

  describe('isLoggedIn', () => {
    it('should return true when token exists', () => {
      localStorage.setItem('entpflow.token', 'token');
      expect(service.isLoggedIn()).toBe(true);
    });

    it('should return false when no token', () => {
      expect(service.isLoggedIn()).toBe(false);
    });
  });

  describe('hasRole', () => {
    it('should return true if user has matching role', () => {
      localStorage.setItem('entpflow.user', JSON.stringify({ userId: 1, role: 'Admin' }));
      expect(service.hasRole('Admin')).toBe(true);
    });

    it('should return false if user has different role', () => {
      localStorage.setItem('entpflow.user', JSON.stringify({ userId: 1, role: 'Employee' }));
      expect(service.hasRole('Admin', 'Manager')).toBe(false);
    });

    it('should return false if no user', () => {
      expect(service.hasRole('Admin')).toBe(false);
    });
  });
});
