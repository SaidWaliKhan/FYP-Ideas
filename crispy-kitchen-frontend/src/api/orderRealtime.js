const RECORD_SEPARATOR = String.fromCharCode(0x1e);

export class OrderRealtimeConnection {
  constructor(token) {
    this.token = token;
    this.handlers = new Map();
    this.socket = null;
  }

  on(eventName, handler) {
    this.handlers.set(eventName, handler);
  }

  async start() {
    const apiUrl = new URL(import.meta.env.VITE_API_URL);
    const hubUrl = new URL('/hubs/orders', apiUrl.origin);
    const negotiateResponse = await fetch(`${hubUrl}/negotiate?negotiateVersion=1`, {
      method: 'POST',
      headers: { Authorization: `Bearer ${this.token}` },
    });

    if (!negotiateResponse.ok) throw new Error('Could not negotiate a real-time connection.');

    const negotiateData = await negotiateResponse.json();
    hubUrl.protocol = hubUrl.protocol === 'https:' ? 'wss:' : 'ws:';
    hubUrl.searchParams.set('id', negotiateData.connectionToken);
    hubUrl.searchParams.set('access_token', this.token);

    await new Promise((resolve, reject) => {
      this.socket = new WebSocket(hubUrl);
      this.socket.onopen = () => this.socket.send(`${JSON.stringify({ protocol: 'json', version: 1 })}${RECORD_SEPARATOR}`);
      this.socket.onerror = () => reject(new Error('Could not open a real-time connection.'));
      this.socket.onmessage = (event) => {
        const messages = event.data.split(RECORD_SEPARATOR).filter(Boolean).map(JSON.parse);
        const handshake = messages.find((message) => !message.type);
        if (handshake) resolve();
        messages.filter((message) => message.type === 1).forEach((message) => this.handlers.get(message.target)?.(...message.arguments));
      };
    });
  }

  invoke(target, ...argumentsList) {
    this.socket?.send(`${JSON.stringify({ type: 1, target, arguments: argumentsList })}${RECORD_SEPARATOR}`);
  }

  stop() {
    this.socket?.close();
    this.socket = null;
  }
}
