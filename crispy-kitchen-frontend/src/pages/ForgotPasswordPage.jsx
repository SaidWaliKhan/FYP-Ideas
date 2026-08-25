import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import apiClient from '../api/apiClient';

export default function ForgotPasswordPage() {
  const navigate = useNavigate();
  const [email, setEmail] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [error, setError] = useState('');

  async function handleSubmit(e) {
    e.preventDefault();
    setError('');

    if (newPassword !== confirmPassword) {
      setError('Passwords do not match.');
      return;
    }

    try {
      await apiClient.post('/auth/recover-password', { email, newPassword });
      navigate('/login', { replace: true, state: { message: 'Password reset. Please log in.' } });
    } catch (err) {
      setError(err.response?.data?.error ?? 'Could not reset the password.');
    }
  }

  return (
    <form onSubmit={handleSubmit}>
      <h2>Reset password</h2>

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
        placeholder="New password (8+ chars, 1 upper, 1 digit)"
        value={newPassword}
        onChange={(e) => setNewPassword(e.target.value)}
        required
      />

      <input
        type="password"
        placeholder="Confirm new password"
        value={confirmPassword}
        onChange={(e) => setConfirmPassword(e.target.value)}
        required
      />

      <button type="submit">Reset password</button>

      <p>
        <Link to="/login">Back to login</Link>
      </p>
    </form>
  );
}
