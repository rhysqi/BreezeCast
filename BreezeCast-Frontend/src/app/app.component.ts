import { Component, Inject, NgModule, PLATFORM_ID } from '@angular/core';

import { ChatService } from './core/services/ChatService/chat.service';
import { isPlatformBrowser, NgFor } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Title } from '@angular/platform-browser';
import { MainLayoutComponent } from "./core/layout/main-layout/main-layout.component";
import { RouterOutlet } from '@angular/router';

const defaultTitle = "Breeze Chat";

@Component({
  selector: 'app-root',
  imports: [NgFor, FormsModule, RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent {

  user: string = '';
  message: string = '';
  messages: { user: string; text: string }[] = [];

  constructor(
    private chat: ChatService,
    private title: Title,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  ngOnInit(): void {
    this.title.setTitle(defaultTitle);

    if (isPlatformBrowser(this.platformId)) {
      this.chat.connectToChat();

      this.chat.onReceiveMessage((user, msg) => {
        this.messages.push({ user, text: msg });
      });

      this.chat.onRateLimit(() => {
        this.messages.push({ user: 'Server', text: '⚠️ You can send every 5 seconds per message' });
      });
  }}

  send(): void {
    if (this.user && this.message) {
      this.chat.sendMessage(this.user, this.message);
      this.message = '';
    }
  }
}
