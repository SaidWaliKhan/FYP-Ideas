import { useState } from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';

const emailPattern = /^\S+@\S+\.\S+$/;
const passwordRequirements = [{ label: 'At least 8 characters', test: (value) => value.length >= 8 }, { label: 'One uppercase letter', test: (value) => /[A-Z]/.test(value) }, { label: 'One number', test: (value) => /[0-9]/.test(value) }];
function registerErrorMessage(err) { if (!err.response) return 'We couldn’t connect. Please try again.'; if (err.response.status === 409) return 'An account with this email already exists.'; return err.response.status >= 500 ? 'We couldn’t connect. Please try again.' : 'We couldn’t create your account. Please review your details and try again.'; }

export default function RegisterPage() {
  const { register } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [fullName, setFullName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [showPassword, setShowPassword] = useState(false);
  const [fieldErrors, setFieldErrors] = useState({});
  const [error, setError] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);
  const cameFromCheckout = location.state?.from?.startsWith?.('/checkout') || location.state?.from === '/checkout';

  function updateField(field, value) { ({ fullName: setFullName, email: setEmail, password: setPassword })[field](value); setFieldErrors((previous) => ({ ...previous, [field]: '' })); setError(''); }
  function validate() { const next = {}; if (fullName.trim().length < 2) next.fullName = 'Enter your full name.'; if (!emailPattern.test(email.trim())) next.email = 'Enter a valid email address.'; if (passwordRequirements.some((requirement) => !requirement.test(password))) next.password = 'Your password does not meet all requirements.'; setFieldErrors(next); return Object.keys(next).length === 0; }
  async function handleSubmit(event) { event.preventDefault(); if (isSubmitting || !validate()) return; setError(''); setIsSubmitting(true); try { await register(fullName.trim(), email.trim(), password); navigate(location.state?.from || '/menu', { replace: true }); } catch (err) { setError(registerErrorMessage(err)); } finally { setIsSubmitting(false); } }

  return <main className="page-narrow auth-page"><form className="surface auth-card form-stack" onSubmit={handleSubmit} noValidate><header><span className="eyebrow">Freshly made for you</span><h1>Create your account</h1><p className="muted">Save your cart, order with ease, and keep track of every meal.</p>{cameFromCheckout && <p className="auth-return-note">Create an account to continue to checkout. Your cart is saved.</p>}</header>{error && <p className="alert alert-error" role="alert">{error}</p>}<div className="field"><label htmlFor="register-name">Full name</label><input id="register-name" autoComplete="name" placeholder="Your full name" value={fullName} onChange={(event) => updateField('fullName', event.target.value)} aria-invalid={Boolean(fieldErrors.fullName)} aria-describedby={fieldErrors.fullName ? 'register-name-error' : undefined} />{fieldErrors.fullName && <small id="register-name-error" className="field-error">{fieldErrors.fullName}</small>}</div><div className="field"><label htmlFor="register-email">Email address</label><input id="register-email" type="email" autoComplete="email" placeholder="you@example.com" value={email} onChange={(event) => updateField('email', event.target.value)} aria-invalid={Boolean(fieldErrors.email)} aria-describedby={fieldErrors.email ? 'register-email-error' : undefined} />{fieldErrors.email && <small id="register-email-error" className="field-error">{fieldErrors.email}</small>}</div><div className="field"><label htmlFor="register-password">Password</label><div className="password-field"><input id="register-password" type={showPassword ? 'text' : 'password'} autoComplete="new-password" placeholder="Create a secure password" value={password} onChange={(event) => updateField('password', event.target.value)} aria-invalid={Boolean(fieldErrors.password)} aria-describedby="password-requirements" /><button type="button" onClick={() => setShowPassword((visible) => !visible)} aria-label={showPassword ? 'Hide password' : 'Show password'} aria-pressed={showPassword}>{showPassword ? 'Hide' : 'Show'}</button></div><ul id="password-requirements" className="password-requirements" aria-label="Password requirements">{passwordRequirements.map((requirement) => <li className={requirement.test(password) ? 'met' : ''} key={requirement.label}>{requirement.test(password) ? '✓' : '○'} {requirement.label}</li>)}</ul>{fieldErrors.password && <small className="field-error">{fieldErrors.password}</small>}</div><button className="button auth-submit" type="submit" disabled={isSubmitting}>{isSubmitting ? 'Creating account…' : 'Create account'}</button><p className="form-note">Already have an account? <Link to="/login" state={{ from: location.state?.from }}>Log in</Link></p></form></main>;
}
