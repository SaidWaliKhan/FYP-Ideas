import { Link } from 'react-router-dom';

export default function NotFoundPage() {
  return <main className="page not-found-page"><section className="surface async-state"><span className="eyebrow">404</span><h1>Looks like this page is off the menu.</h1><p>We couldn’t find the page you’re looking for.</p><div className="button-row"><Link className="button" to="/">Back home</Link><Link className="button-secondary" to="/menu">Explore menu</Link></div></section></main>;
}
