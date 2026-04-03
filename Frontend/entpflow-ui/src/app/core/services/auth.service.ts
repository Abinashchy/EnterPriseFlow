// import { Injectable } from '@angular/core';
// import { HttpClient } from '@angular/common/http';
// import { Observable, tap } from 'rxjs';
// import { LoginRequest, LoginResponse } from '../models/auth.model';
// import { CurrentUser } from '../models/user.model';

// @Injectable({
//   providedIn: 'root'
// })
// export class AuthService {
//   private readonly apiUrl = '/api/auth';
//   private readonly tokenKey = 'entpflow.token';
//   private readonly userKey = 'entpflow.user';

//   constructor(private http: HttpClient) {}

//   login(payload: LoginRequest): Observable<LoginResponse> {
//     return this.http.post<LoginResponse>(`${this.apiUrl}/login`, payload).pipe(
//       tap((response) => {
//         localStorage.setItem(this.tokenKey, response.token);
//         localStorage.setItem(this.userKey, JSON.stringify(response.user));
//       })
//     );
//   }

//   logout(): void {
//     localStorage.removeItem(this.tokenKey);
//     localStorage.removeItem(this.userKey);
//   }

//   getToken(): string | null {
//     return localStorage.getItem(this.tokenKey);
//   }

//   getCurrentUser(): CurrentUser | null {
//     const raw = localStorage.getItem(this.userKey);
//     return raw ? JSON.parse(raw) as CurrentUser : null;
//   }

//   isLoggedIn(): boolean {
//     return !!this.getToken();
//   }

//   hasRole(...roles: string[]): boolean {
//     const currentUser = this.getCurrentUser();
//     return !!currentUser && roles.includes(currentUser.roleName);
//   }
// }


import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { isPlatformBrowser } from '@angular/common'; // Add this import
import { Observable, tap, of } from 'rxjs';
import { LoginRequest, LoginResponse } from '../models/auth.model';
import { CurrentUser } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly apiUrl = '/api/auth';
  private readonly tokenKey = 'entpflow.token';
  private readonly userKey = 'entpflow.user';

  // Create a helper boolean to check if we are in the browser
  private isBrowser: boolean;

  constructor(
    private http: HttpClient,
    @Inject(PLATFORM_ID) platformId: Object // Inject the Platform ID
  ) {
    this.isBrowser = isPlatformBrowser(platformId);
  }

  login(payload: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, payload).pipe(
      tap((response) => {
        if (this.isBrowser) {
          localStorage.setItem(this.tokenKey, response.token);
          localStorage.setItem(this.userKey, JSON.stringify(response.user));
        }
      })
    );
  }

  logout(): void {
    if (this.isBrowser) {
      localStorage.removeItem(this.tokenKey);
      localStorage.removeItem(this.userKey);
    }
  }

  getToken(): string | null {
    if (this.isBrowser) {
      return localStorage.getItem(this.tokenKey);
    }
    return null;
  }

  getCurrentUser(): CurrentUser | null {
    if (this.isBrowser) {
      const raw = localStorage.getItem(this.userKey);
      return raw ? (JSON.parse(raw) as CurrentUser) : null;
    }
    return null;
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  hasRole(...roles: string[]): boolean {
    const currentUser = this.getCurrentUser();
    return !!currentUser && roles.includes(currentUser.role);
  }
}
