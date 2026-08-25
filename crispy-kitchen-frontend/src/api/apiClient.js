import axios from 'axios';

const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
});

// Runs before every outgoing request. If we have a token stored,
// attach it as a Bearer header — this IS handing over the JWT
// "wristband" we talked about on the backend, automatically, every time.
apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem('ck_token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export default apiClient;