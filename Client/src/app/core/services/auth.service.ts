import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject, catchError, map, Observable, tap, throwError } from 'rxjs';
import { jwtDecode } from 'jwt-decode';

export interface AuthResponse {
  token: string,
  expiration: string
}

export interface DecodedClaims {
  nameid : string;
  FullName : string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly baseUrl = "https://localhost:7189/api";
  private curentUserSubject = new BehaviorSubject<string | null>(null);
  currentUser$ = this.curentUserSubject.asObservable();


  constructor(private http:HttpClient,private toastr: ToastrService, private router: Router){
    const token = localStorage.getItem('token');
    if (token){
      this.curentUserSubject.next(token);
    }
  }

  getUserIdFromToken(): string | null {
    const token = this.token;
    if(!token)
      return null;

    const decoded : DecodedClaims = jwtDecode(token);
    return decoded?.nameid || null;
  }

  getFullNameFromToken() : string | null {
    const token = this.token;
    if (!token)
      return null;
    const decoded : DecodedClaims = jwtDecode(token);
    
    return decoded?.FullName || null;

  }

  register(formData: any){
    return this.http.post<AuthResponse>(this.baseUrl + '/register', formData)
    .pipe(tap((res)=>{
      localStorage.setItem('token', res.token);
      this.curentUserSubject.next(res.token);
    }));
    
  }

  login(formData: any) : Observable<AuthResponse>{
    return this.http.post<AuthResponse>(this.baseUrl + '/login', formData)
    .pipe(tap((res) =>{
      localStorage.setItem('token', res.token);
      this.curentUserSubject.next(res.token);
    }),
    catchError(err => {
      console.error('Login error response:', err); 
      this.toastr.error(err.error?.message || 'Login failed');
      return throwError(() => err);
    })
  );
}
  logout(){
    localStorage.removeItem('token');
    this.curentUserSubject.next(null);
    if (this.router.url == '/'){
      window.location.reload();
    }
    else {
      this.router.navigateByUrl('/');
    }
    
  }
  isAuthenticated$ = this.currentUser$.pipe(map(token => !!token));

  get token() : string | null{
    return localStorage.getItem('token');
  }

}
