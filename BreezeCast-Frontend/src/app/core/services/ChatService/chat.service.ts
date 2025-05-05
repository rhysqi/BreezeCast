import { Injectable, PLATFORM_ID } from '@angular/core';
import * as Chat from '@microsoft/signalr';

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private hubConnection!: Chat.HubConnection;

  private listenersReady = false;
  private onReceiveMessageCallbacks: ((user: string, message: string) => void)[] = [];
  private onRateLimitCallbacks: (() => void)[] = [];

  public connectToChat(): void {
    this.hubConnection = new Chat.HubConnectionBuilder()
      .withUrl('http://localhost:8080/chat')
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start()
      .then(() => {
        console.log('Connected to chat hub!');
        this.registerListeners();
      })
      .catch(err => console.error('Error connecting to chat hub:', err));
  }

  private registerListeners(): void {
    if (this.listenersReady) return;

    this.hubConnection.on('ReceiveMessage', (user: string, message: string) => {
      this.onReceiveMessageCallbacks.forEach(cb => cb(user, message));
    });

    this.hubConnection.on('RateLimitExceeded', () => {
      this.onRateLimitCallbacks.forEach(cb => cb());
    });

    this.listenersReady = true;
  }

  public onReceiveMessage(callback: (user: string, message: string) => void): void {
    this.onReceiveMessageCallbacks.push(callback);
  }

  public onRateLimit(callback: () => void): void {
    this.onRateLimitCallbacks.push(callback);
  }

  public sendMessage(user: string, message: string): void {
    if (this.hubConnection?.state === Chat.HubConnectionState.Connected) {
      this.hubConnection.invoke('SendMessage', user, message)
        .catch(err => console.error('Error sending message:', err));
    } else {
      console.warn('Hub not connected yet.');
    }
  }
}

