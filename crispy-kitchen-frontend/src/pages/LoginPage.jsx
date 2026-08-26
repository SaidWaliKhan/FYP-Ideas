import { useState } from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';

const emailPattern = /^\S+@\S+\.\S+$/;
function loginErrorMessage(err) { if (!err.response) return 'We couldn’t connect. Please try again.'; if (err.response.status === 401) return 'Email or password is incorrect.'; return err.response.status >= 500 ? 'We couldn’t connect. Please try again.' : 'We couldn’t sign you in. Please check your details and try again.'; }

export default function LoginPage() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [showPassword, setShowPassword] = useState(false);
  const [fieldErrors, setFieldErrors] = useState({});
  const [error, setError] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);
  const cameFromCheckout = location.state?.from?.startsWith?.('/checkout') || location.state?.from === '/checkout';

  function updateField(field, value) { if (field === 'email') setEmail(value); else setPassword(value); setFieldErrors((previous) => ({ ...previous, [field]: '' })); setError(''); }
  function validate() { const next = {}; if (!emailPattern.test(email.trim())) next.email = 'Enter a valid email address.'; if (!password) next.password = 'Enter your password.'; setFieldErrors(next); return Object.keys(next).length === 0; }
  async function handleSubmit(event) { event.preventDefault(); if (isSubmitting || !validate()) return; setError(''); setIsSubmitting(true); try { await login(email.trim(), password); navigate(location.state?.from || '/menu', { replace: true }); } catch (err) { setError(loginErrorMessage(err)); } finally { setIsSubmitting(false); } }

  return <main className="page-narrow auth-page"><form className="surface auth-card form-stack" onSubmit={handleSubmit} noValidate><header><span className="eyebrow">Welcome back</span><h1>Good food is waiting.</h1><p className="muted">Sign in to reorder your favourites and track every order.</p>{cameFromCheckout && <p className="auth-return-note">Sign in to continue to checkout. Your cart is saved.</p>}</header>{location.state?.message && <p className="alert alert-success" role="status">{location.state.message}</p>}{error && <p className="alert alert-error" role="alert">{error}</p>}<div className="field"><label htmlFor="login-email">Email address</label><input id="login-email" type="email" autoComplete="email" placeholder="you@example.com" value={email} onChange={(event) => updateField('email', event.target.value)} aria-invalid={Boolean(fieldErrors.email)} aria-describedby={fieldErrors.email ? 'login-email-error' : undefined} />{fieldErrors.email && <small id="login-email-error" className="field-error">{fieldErrors.email}</small>}</div><div className="field"><label htmlFor="login-password">Password</label><div className="password-field"><input id="login-password" type={showPassword ? 'text' : 'password'} autoComplete="current-password" placeholder="Enter your password" value={password} onChange={(event) => updateField('password', event.target.value)} aria-invalid={Boolean(fieldErrors.password)} aria-describedby={fieldErrors.password ? 'login-password-error' : undefined} /><button type="button" onClick={() => setShowPassword((visible) => !visible)} aria-label={showPassword ? 'Hide password' : 'Show password'} aria-pressed={showPassword}>{showPassword ? 'Hide' : 'Show'}</button></div>{fieldErrors.password && <small id="login-password-error" className="field-error">{fieldErrors.password}</small>}</div><button className="button auth-submit" type="submit" disabled={isSubmitting}>{isSubmitting ? 'Signing in…' : 'Log in'}</button><p className="form-note"><Link to="/forgot-password">Forgot password?</Link></p><p className="form-note">No account? <Link to="/register" state={{ from: location.state?.from }}>Register</Link></p></form></main>;
}
