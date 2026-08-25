import { useEffect, useState } from 'react';
import apiClient from '../api/apiClient';

const NEXT_STATUS = {
  Pending: 'Confirmed',
  Confirmed: 'Preparing',
  Preparing: 'Ready',
  Ready: 'Delivered',       // covers pickup orders
  OutForDelivery: 'Delivered',
};

// Must match CrispyKitchen.Domain.Enums.OrderStatus's numeric values exactly.
const STATUS_TO_INT = {
  Pending: 0, Confirmed: 1, Preparing: 2, Ready: 3, OutForDelivery: 4, Delivered: 5, Cancelled: 6,
};

const COLUMNS = ['Pending', 'Confirmed', 'Preparing', 'Ready', 'OutForDelivery'];

export default function KitchenDashboardPage() {
  const [orders, setOrders] = useState([]);
  const [error, setError] = useState('');

  async function loadOrders() {
    const { data } = await apiClient.get('/orders/active');
    setOrders(data);
  }

  useEffect(() => {
    loadOrders();
    const interval = setInterval(loadOrders, 5000);
    return () => clearInterval(interval);
  }, []);

  async function advance(order) {
    const nextStatus = NEXT_STATUS[order.status];
    if (!nextStatus) return;

    try {
      await apiClient.patch(`/orders/${order.id}/status`, {
        id: order.id,
        newStatus: STATUS_TO_INT[nextStatus],
      });
      loadOrders();
    } catch (err) {
      // If the backend's real state machine ever disagrees with this
      // frontend's guess, THIS is where that mismatch surfaces — not
      // a silent failure.
      setError(err.response?.data?.error ?? 'Could not update order status.');
    }
  }

  return (
    <div>
      <h2>Kitchen Dashboard</h2>
      {error && <p style={{ color: 'red' }}>{error}</p>}
      <div style={{ display: 'flex', gap: '1rem' }}>
        {COLUMNS.map((status) => (
          <div key={status} style={{ flex: 1, border: '1px solid #ccc', padding: '0.5rem' }}>
            <h4>{status}</h4>
            {orders.filter((o) => o.status === status).map((o) => (
              <div key={o.id} style={{ border: '1px solid #999', margin: '0.5rem 0', padding: '0.5rem' }}>
                <strong>#{o.id.slice(0, 8)}</strong>
                <ul>
                  {o.items.map((i) => <li key={i.productId}>{i.quantity}x {i.productName}</li>)}
                </ul>
                {NEXT_STATUS[status] && (
                  <button onClick={() => advance(o)}>Mark {NEXT_STATUS[status]}</button>
                )}
              </div>
            ))}
          </div>
        ))}
      </div>
    </div>
  );
}