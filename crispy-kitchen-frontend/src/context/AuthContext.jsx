import { createContext, useContext, useState } from 'react';
import apiClient from '../api/apiClient';

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  // Reading from localStorage on first render means refreshing the
  // page doesn't log you out — the token survives.
  const [token, setToken] = useState(() => localStorage.getItem('ck_token'));
  const [role, setRole] = useState(() => localStorage.getItem('ck_role'));

  function saveSession(authResult) {
    localStorage.setItem('ck_token', authResult.token);
    localStorage.setItem('ck_role', authResult.role);
    setToken(authResult.token);
    setRole(authResult.role);
  }

  async function login(email, password) {
    const { data } = await apiClient.post('/auth/login', { email, password });
    saveSession(data);
  }

  async function register(fullName, email, password) {
    const { data } = await apiClient.post('/auth/register', { fullName, email, password });
    saveSession(data);
  }

  function logout() {
    localStorage.removeItem('ck_token');
    localStorage.removeItem('ck_role');
    setToken(null);
    setRole(null);
  }

  return (
    <AuthContext.Provider value={{ token, role, login, register, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  return useContext(AuthContext);
}