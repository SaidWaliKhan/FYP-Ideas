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
    <main className="page-narrow"><form className="surface auth-card form-stack" onSubmit={handleSubmit}>
      <header><span className="eyebrow">Account recovery</span><h2>Reset your password</h2><p className="muted">Choose a new secure password to get back to ordering.</p></header>

      {error && <p className="alert alert-error">{error}</p>}

      <div className="field"><label>Email address</label><input
        type="email"
        placeholder="Email"
        value={email}
        onChange={(e) => setEmail(e.target.value)}
        required
      /></div>

      <div className="field"><label>New password</label><input
        type="password"
        placeholder="New password (8+ chars, 1 upper, 1 digit)"
        value={newPassword}
        onChange={(e) => setNewPassword(e.target.value)}
        required
      /></div>

      <div className="field"><label>Confirm password</label><input
        type="password"
        placeholder="Confirm new password"
        value={confirmPassword}
        onChange={(e) => setConfirmPassword(e.target.value)}
        required
      /></div>

      <button className="button" type="submit">Reset password</button>

      <p className="form-note">
        <Link to="/login">Back to login</Link>
      </p>
    </form></main>
  );
}
