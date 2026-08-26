import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import apiClient from '../api/apiClient';
import { EmptyState, ErrorState } from '../components/AsyncStates';

export default function MyOrdersPage() {
  const [orders, setOrders] = useState([]);
  const [status, setStatus] = useState('');
  const [pageNumber, setPageNumber] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [retryKey, setRetryKey] = useState(0);

  useEffect(() => {
    const timer = window.setTimeout(() => {
      setLoading(true); setError('');
      apiClient.get('/orders/mine', { params: { pageNumber, pageSize: 10, status: status || undefined } })
        .then((res) => { setOrders(res.data.items); setTotalPages(res.data.totalPages); })
        .catch(() => setError('Unable to load orders.'))
        .finally(() => setLoading(false));
    }, 0);
    return () => window.clearTimeout(timer);
  }, [pageNumber, status, retryKey]);

  return (
    <main className="page orders-page"><div className="section-heading orders-header"><div><span className="eyebrow">Order history</span><h2>Your orders</h2><p className="muted">Follow every order from our kitchen to your door.</p></div><select className="filter-input order-filter" value={status} onChange={(e) => { setStatus(e.target.value); setPageNumber(1); }}><option value="">All statuses</option>{['Pending', 'Confirmed', 'Preparing', 'Ready', 'OutForDelivery', 'Delivered', 'Cancelled'].map((item) => <option key={item}>{item}</option>)}</select></div>
      {error && !loading && <ErrorState title="We couldn’t load your orders" message="Check your connection and try again." onRetry={() => setRetryKey((key) => key + 1)} />}{loading && <div className="order-list" aria-label="Loading orders">{Array.from({ length: 3 }).map((_, index) => <div className="surface order-card skeleton" key={index} />)}</div>}
      {!loading && !error && orders.length === 0 && <EmptyState title={status ? 'No matching orders' : 'No orders yet'} message={status ? 'Try another status or view all of your orders.' : 'When you place an order, you’ll be able to track it here.'} action={status ? <button className="button-secondary" type="button" onClick={() => { setStatus(''); setPageNumber(1); }}>View all orders</button> : <Link className="button-secondary" to="/menu">Explore menu</Link>} />}
      {!loading && !error && <div className="order-list">
        {orders.map((o) => (
          <Link className="surface order-card" key={o.id} to={`/orders/${o.id}`}><div><span className="order-id">Order #{o.id.slice(0, 8)}</span><div className="order-card__meta"><span className={`badge status-${o.status.toLowerCase()}`}>{o.status}</span><span>{o.items?.length ?? 0} items</span><span>{o.fulfillmentType ?? 'Pickup'}</span><span>{o.createdAtUtc ? new Date(o.createdAtUtc).toLocaleDateString() : 'Recent order'}</span></div></div><div className="order-card__end"><span className="order-total">${o.total.toFixed(2)}</span><span>Track order →</span></div></Link>
        ))}
      </div>}<div className="pagination"><button className="button-quiet" onClick={() => setPageNumber((page) => page - 1)} disabled={pageNumber === 1}>Previous</button><span>Page {pageNumber} of {totalPages || 1}</span><button className="button-quiet" onClick={() => setPageNumber((page) => page + 1)} disabled={pageNumber >= totalPages}>Next</button></div></main>
  );
}
