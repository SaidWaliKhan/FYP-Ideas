import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import apiClient from '../api/apiClient';

export default function MyOrdersPage() {
  const [orders, setOrders] = useState([]);

  useEffect(() => {
    apiClient.get('/orders/mine').then((res) => setOrders(res.data));
  }, []);

  return (
    <div>
      <h2>My Orders</h2>
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
    </div>
  );
}