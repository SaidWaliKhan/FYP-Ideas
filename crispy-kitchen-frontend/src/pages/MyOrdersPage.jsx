import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import apiClient from '../api/apiClient';

export default function MyOrdersPage() {
  const [orders, setOrders] = useState([]);
  const [status, setStatus] = useState('');
  const [pageNumber, setPageNumber] = useState(1);
  const [totalPages, setTotalPages] = useState(1);

  useEffect(() => {
    apiClient.get('/orders/mine', { params: { pageNumber, pageSize: 10, status: status || undefined } })
      .then((res) => { setOrders(res.data.items); setTotalPages(res.data.totalPages); });
  }, [pageNumber, status]);

  return (
    <div>
      <h2>My Orders</h2>
      <select value={status} onChange={(e) => { setStatus(e.target.value); setPageNumber(1); }}><option value="">All statuses</option>{['Pending', 'Confirmed', 'Preparing', 'Ready', 'OutForDelivery', 'Delivered', 'Cancelled'].map((item) => <option key={item}>{item}</option>)}</select>
      {orders.length === 0 && <p>No orders yet.</p>}
      <ul>
        {orders.map((o) => (
          <li key={o.id}>
            <Link to={`/orders/${o.id}`}>
              Order #{o.id.slice(0, 8)} — {o.status} — ${o.total.toFixed(2)}
            </Link>
          </li>
        ))}
      </ul>
      <button onClick={() => setPageNumber((page) => page - 1)} disabled={pageNumber === 1}>Previous page</button>
      <span> Page {pageNumber} of {totalPages || 1} </span>
      <button onClick={() => setPageNumber((page) => page + 1)} disabled={pageNumber >= totalPages}>Next page</button>
    </div>
  );
}
