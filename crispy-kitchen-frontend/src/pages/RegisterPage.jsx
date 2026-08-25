import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export default function RegisterPage() {
  const { register } = useAuth();
  const navigate = useNavigate();

  const [fullName, setFullName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');

  async function handleSubmit(e) {
    e.preventDefault();
    setError('');

    try {
      await register(fullName, email, password);

      // After successful registration, go to the menu page
      navigate('/menu');
    } catch (err) {
      setError(err.response?.data?.error ?? 'Registration failed.');
    }
  }

  return (
    <form onSubmit={handleSubmit}>
      <h2>Register</h2>

      {error && <p style={{ color: 'red' }}>{error}</p>}

      <input
        placeholder="Full name"
        value={fullName}
        onChange={(e) => setFullName(e.target.value)}
        required
      />

      <input
        type="email"
        placeholder="Email"
        value={email}
        onChange={(e) => setEmail(e.target.value)}
        required
      />

      <input
        type="password"
        placeholder="Password (8+ chars, 1 upper, 1 digit)"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
        required
      />

      <button type="submit">Create account</button>

      <p>
        Already have an account? <Link to="/login">Log in</Link>
      </p>
    </form>
  );
}