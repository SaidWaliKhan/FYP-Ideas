import { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import apiClient from '../api/apiClient';

const STEPS = ['Pending', 'Confirmed', 'Preparing', 'Ready', 'OutForDelivery', 'Delivered'];

export default function OrderTrackingPage() {
  const { id } = useParams();
  const [order, setOrder] = useState(null);
  const [error, setError] = useState('');

  useEffect(() => {
    let cancelled = false;

    async function fetchOrder() {
      try {
        const { data } = await apiClient.get(`/orders/${id}`);
        if (!cancelled) setOrder(data);
      } catch (err) {
        if (!cancelled) setError(err.response?.data?.error ?? 'Could not load order.');
      }
    }

    fetchOrder();

    // Polling: ask the server every 5 seconds instead of true real-time
    // push. SignalR (true real-time, server pushes updates to the
    // browser instantly) is the correct Phase 2 upgrade — this is the
    // simplest thing that still feels "live" without adding a
    // persistent WebSocket connection yet. Honest trade-off: up to a
    // 5-second delay, and it keeps polling even if nothing changed.
    const interval = setInterval(fetchOrder, 5000);

    return () => {
      cancelled = true;
      clearInterval(interval);
    };
  }, [id]);

  if (error) return <p style={{ color: 'red' }}>{error}</p>;
  if (!order) return <p>Loading...</p>;

  const currentStepIndex = order.status === 'Cancelled' ? -1 : STEPS.indexOf(order.status);

  return (
    <div>
      <Link to="/orders/mine">← Back to my orders</Link>
      <h2>Order #{order.id.slice(0, 8)}</h2>

      {order.status === 'Cancelled' ? (
        <p style={{ color: 'red' }}>This order was cancelled.</p>
      ) : (
        <ol>
          {STEPS.map((step, index) => (
            <li key={step} style={{ fontWeight: index === currentStepIndex ? 'bold' : 'normal' }}>
              {index <= currentStepIndex ? '✅' : '⚪'} {step}
            </li>
          ))}
        </ol>
      )}

      <h3>Items</h3>
      <ul>
        {order.items.map((i) => (
          <li key={i.productId}>{i.productName} × {i.quantity} — ${i.lineTotal.toFixed(2)}</li>
        ))}
      </ul>
      <p><strong>Total: ${order.total.toFixed(2)}</strong></p>
    </div>
  );
}