import { Link } from 'react-router-dom';

export function LoadingState({ label = 'Loading…', className = '' }) {
  return <div className={`async-loading ${className}`} role="status" aria-live="polite"><span aria-hidden="true" /><p>{label}</p></div>;
}

export function EmptyState({ title, message, action, className = '' }) {
  return <section className={`surface async-state async-empty ${className}`}><span className="async-state-icon" aria-hidden="true">○</span><h2>{title}</h2><p>{message}</p>{action}</section>;
}

export function ErrorState({ title = 'Something went wrong', message = 'We couldn’t load this information right now.', onRetry, actions, className = '' }) {
  return <section className={`surface async-state async-error ${className}`} role="alert"><span className="async-state-icon" aria-hidden="true">!</span><h2>{title}</h2><p>{message}</p><div className="button-row">{onRetry && <button className="button" type="button" onClick={onRetry}>Try again</button>}{actions}</div></section>;
}

export function AccessDeniedState() {
  return <main className="page"><section className="surface async-state async-error access-denied"><span className="eyebrow">Access denied</span><h1>You don’t have access to this page.</h1><p>Please return to a page available to your account.</p><div className="button-row"><Link className="button" to="/">Go home</Link><Link className="button-secondary" to="/menu">Explore menu</Link></div></section></main>;
}
