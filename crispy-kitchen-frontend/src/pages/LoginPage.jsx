import { useState } from 'react';
import { useNavigate, Link, useLocation } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';

export default function LoginPage() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();

  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');

  async function handleSubmit(e) {
    e.preventDefault();
    setError('');

    try {
      await login(email, password);

      // After successful login, go to the menu page
      navigate('/menu');
    } catch (err) {
      setError(err.response?.data?.error ?? 'Login failed.');
    }
  }

  return (
    <form onSubmit={handleSubmit}>
      <h2>Login</h2>

      {location.state?.message && <p style={{ color: 'green' }}>{location.state.message}</p>}
      {error && <p style={{ color: 'red' }}>{error}</p>}

      <input
        type="email"
        placeholder="Email"
        value={email}
        onChange={(e) => setEmail(e.target.value)}
        required
      />

      <input
        type="password"
        placeholder="Password"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
        required
      />

      <button type="submit">Log in</button>

      <p>
        <Link to="/forgot-password">Forgot password?</Link>
      </p>

      <p>
        No account? <Link to="/register">Register</Link>
      </p>
    </form>
  );
}
