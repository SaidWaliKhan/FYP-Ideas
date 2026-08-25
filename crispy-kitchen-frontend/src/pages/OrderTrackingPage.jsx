import { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import apiClient from '../api/apiClient';
import { useAuth } from '../hooks/useAuth';
import { OrderRealtimeConnection } from '../api/orderRealtime';

const STEPS = ['Pending', 'Confirmed', 'Preparing', 'Ready', 'OutForDelivery', 'Delivered'];

export default function OrderTrackingPage() {
  const { id } = useParams();
  const { role, token } = useAuth();
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

    const connection = new OrderRealtimeConnection(token);
    connection.on('OrderUpdated', (updatedOrder) => {
      if (!cancelled) setOrder(updatedOrder);
    });
    connection.start()
      .then(() => connection.invoke('SubscribeToOrder', id))
      .catch(() => {});

    return () => {
      cancelled = true;
      connection.stop();
    };
  }, [id, token]);

  if (error) return <p style={{ color: 'red' }}>{error}</p>;
  if (!order) return <p>Loading...</p>;

  const currentStepIndex = order.status === 'Cancelled' ? -1 : STEPS.indexOf(order.status);
  const canCancel = role === 'Customer' && (order.status === 'Pending' || order.status === 'Confirmed');
  const canSimulatePayment = role === 'Customer' && order.paymentStatus === 'Pending';

  async function cancelOrder() {
    if (!window.confirm('Cancel this order? The reserved items will be returned to stock.')) return;

    try {
      const { data } = await apiClient.patch(`/orders/${id}/cancel`);
      setOrder(data);
    } catch (err) {
      setError(err.response?.data?.error ?? 'Could not cancel order.');
    }
  }

  async function simulatePayment(simulateSuccess) {
    try {
      const { data } = await apiClient.post(`/orders/${id}/payment/simulate`, { id, simulateSuccess });
      setOrder(data);
    } catch (err) {
      setError(err.response?.data?.error ?? 'Could not simulate payment.');
    }
  }

  return (
    <div>
      <Link to="/orders/mine">← Back to my orders</Link>
      <h2>Order #{order.id.slice(0, 8)}</h2>
      <p><strong>Payment status:</strong> {order.paymentStatus}</p>
      {canSimulatePayment && <section>
        <p><em>Development/testing only — no real money is charged.</em></p>
        <button onClick={() => simulatePayment(true)}>Simulate payment success</button>
        <button onClick={() => simulatePayment(false)}>Simulate payment failure</button>
      </section>}

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
      {canCancel && <button onClick={cancelOrder}>Cancel order</button>}
      {order.statusHistory.length > 0 && <>
        <h3>Status history</h3>
        <ul>{order.statusHistory.map((history) => <li key={`${history.changedAtUtc}-${history.newStatus}`}>{history.previousStatus} → {history.newStatus} by {history.changedByName} at {new Date(history.changedAtUtc).toLocaleString()}</li>)}</ul>
      </>}
    </div>
  );
}
