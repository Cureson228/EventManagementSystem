import { HttpEvent, HttpHandler, HttpInterceptor, HttpInterceptorFn, HttpRequest } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { AuthService } from "../services/auth.service";
import { Observable } from "rxjs";

export const JwtInterceptor: HttpInterceptorFn = (req, next)=>{
    const authService = inject(AuthService);

    const token = authService.token;

    if (token){
        const cloned = req.clone({
            setHeaders: {Authorization: 'Bearer ' + token}
        });
        return next(cloned);

    }
    return next(req);
}