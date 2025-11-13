import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AiAssistanceService } from '../../core/services/ai-assistance.service';
import { AuthService } from '../../core/services/auth.service';
import { firstValueFrom } from 'rxjs';
import { ToastrService } from 'ngx-toastr';

interface Message {
  role: 'user' | 'assistant';
  text: string;
}

@Component({
  selector: 'app-ai-assistant',
  standalone: true,
  imports: [CommonModule,FormsModule],
  templateUrl: './ai-assistance.component.html'
})
export class AiAssistanceComponent {
  isOpen = false;
  question = '';
  messages: Message[] = [];

  constructor(private http: HttpClient, private assistanceService : AiAssistanceService
    , public authService : AuthService, private toastr: ToastrService
  ) {}

  ask() {
    if (!this.question.trim()) return;

    this.messages.push({ role: 'user', text: this.question });
  

    this.assistanceService.getAnswer(this.question).subscribe(res => {
        this.messages.push({ role: 'assistant', text: res.answer });
        this.question = '';
      });
  }

  getMessageClass(role: 'user' | 'assistant') {
    return role === 'user' ? 'text-right text-blue-600' : 'text-left text-gray-800';
  }
  async openChat(){
    const isAuth = await firstValueFrom(this.authService.isAuthenticated$);

    if (isAuth){
      console.log(isAuth);
      
      this.isOpen = true;
    }
    else {
      this.toastr.warning('Please authenticate to use AI Assistance');
      return;
    }
  }
}