import { useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import apiClient from '../api/apiClient';
import { useAuth } from '../hooks/useAuth';
import { OrderRealtimeConnection } from '../api/orderRealtime';
import { ErrorState, LoadingState } from '../components/AsyncStates';

const DELIVERY_STEPS = ['Pending', 'Confirmed', 'Preparing', 'Ready', 'OutForDelivery', 'Delivered'];
function statusDetails(status, isDelivery) {
  const journey = isDelivery ? 'delivery' : 'pickup';
  const details = {
    Pending: { title: 'Order received', sentence: `We’ve received your ${journey} order.`, detail: 'We are checking the details and will confirm it shortly.' },
    Confirmed: { title: 'Order confirmed', sentence: `Your ${journey} order has been confirmed.`, detail: 'The kitchen has your order and will begin preparing it soon.' },
    Preparing: { title: 'Being prepared', sentence: 'The kitchen is preparing your food.', detail: 'Your order is being made fresh.' },
    Ready: isDelivery ? { title: 'Ready for delivery', sentence: 'Your food is ready to leave the kitchen.', detail: 'It will be handed over for delivery next.' } : { title: 'Ready for pickup', sentence: 'Your order is ready for pickup.', detail: 'Please collect your meal when you are ready.' },
    OutForDelivery: { title: 'Out for delivery', sentence: 'Your order is on the way.', detail: 'Your delivery is heading to the address you provided.' },
    Delivered: isDelivery ? { title: 'Delivered', sentence: 'Your order has been delivered.', detail: 'We hope you enjoy every bite.' } : { title: 'Picked up / completed', sentence: 'Your pickup order is complete.', detail: 'We hope you enjoy every bite.' },
    Cancelled: { title: 'Order cancelled', sentence: 'This order was cancelled.', detail: 'Contact support if you need help with this order.' },
  };
  return details[status] ?? details.Pending;
}

const formatMoney = (value) => `$${Number(value ?? 0).toFixed(2)}`;
const formatDate = (value) => (value ? new Date(value).toLocaleString([], { dateStyle: 'medium', timeStyle: 'short' }) : '—');

export default function OrderTrackingPage() {
  const { id } = useParams();
  const { role, token } = useAuth();
  const [order, setOrder] = useState(null);
  const [error, setError] = useState('');
  const [actionError, setActionError] = useState('');
  const [isCancelDialogOpen, setIsCancelDialogOpen] = useState(false);
  const [isCancelling, setIsCancelling] = useState(false);
  const [retryKey, setRetryKey] = useState(0);

  useEffect(() => {
    let cancelled = false;
    async function fetchOrder() {
      try {
        const { data } = await apiClient.get(`/orders/${id}`);
        if (!cancelled) setOrder(data);
      } catch (err) {
        if (!cancelled) setError(err.response?.status === 404 || err.response?.status === 403 ? 'not-found' : 'connection');
      }
    }
    fetchOrder();
    const connection = new OrderRealtimeConnection(token);
    connection.on('OrderUpdated', (updatedOrder) => { if (!cancelled) setOrder(updatedOrder); });
    connection.start().then(() => connection.invoke('SubscribeToOrder', id)).catch(() => {});
    return () => { cancelled = true; connection.stop(); };
  }, [id, token, retryKey]);

  if (error) return <main className="page"><ErrorState title={error === 'not-found' ? 'Order not found' : 'We couldn’t connect'} message={error === 'not-found' ? 'We couldn’t find this order or you may not have access to it.' : 'Check your connection and try again.'} onRetry={() => { setError(''); setRetryKey((key) => key + 1); }} actions={<><Link className="button-secondary" to="/orders/mine">Back to My Orders</Link><Link className="button-quiet" to="/menu">Explore menu</Link></>} /></main>;
  if (!order) return <main className="page tracking-page"><section className="surface tracking-loading-card"><LoadingState label="Loading your order details…" /></section></main>;

  const isDelivery = order.fulfillmentType === 'Delivery';
  const status = statusDetails(order.status, isDelivery);
  const steps = isDelivery ? DELIVERY_STEPS : ['Pending', 'Confirmed', 'Preparing', 'Ready', 'Delivered'];
  const currentStepIndex = order.status === 'Cancelled' ? -1 : steps.indexOf(order.status);
  const canCancel = role === 'Customer' && (order.status === 'Pending' || order.status === 'Confirmed');
  const canSimulatePayment = import.meta.env.DEV && role === 'Customer' && order.paymentStatus === 'Pending';

  async function confirmCancelOrder() {
    setIsCancelling(true);
    setActionError('');
    try {
      const { data } = await apiClient.patch(`/orders/${id}/cancel`);
      setOrder(data);
      setIsCancelDialogOpen(false);
    } catch (err) {
      setActionError(err.response?.data?.error ?? 'Could not cancel order.');
    } finally { setIsCancelling(false); }
  }
  async function simulatePayment(simulateSuccess) {
    setActionError('');
    try {
      const { data } = await apiClient.post(`/orders/${id}/payment/simulate`, { id, simulateSuccess });
      setOrder(data);
    } catch (err) { setActionError(err.response?.data?.error ?? 'Could not simulate payment.'); }
  }

  return <main className="page tracking-page">
    <Link className="button-quiet tracking-back-link" to="/orders/mine">← Back to my orders</Link>
    <header className="dashboard-header tracking-header"><span className="eyebrow">Live order tracking</span><h1>Order #{order.id.slice(0, 8)}</h1><p className="muted">{status.sentence}</p></header>
    <div className="tracking-layout">
      <section className="surface tracking-card tracking-card--primary">
        <div className="tracking-info-grid" aria-label="Order information">
          <div className="tracking-info"><span>Fulfillment</span><strong>{order.fulfillmentType ?? 'Pickup'}</strong></div>
          <div className="tracking-info"><span>Order status</span><strong className={`tracking-status tracking-status--${order.status?.toLowerCase()}`}>{status.title}</strong></div>
          <div className="tracking-info"><span>Payment</span><strong className="tracking-payment">{order.paymentStatus ?? 'Pending'}</strong></div>
          <div className="tracking-info"><span>Contact</span><strong>{order.contactPhone || '—'}</strong></div>
          <div className="tracking-info"><span>Placed</span><strong>{formatDate(order.placedAtUtc)}</strong></div>
          {isDelivery && <div className="tracking-info tracking-info--wide"><span>Delivery address</span><strong>{[order.deliveryAddress, order.deliveryCity].filter(Boolean).join(', ') || '—'}</strong></div>}
        </div>
        {actionError && <p className="alert alert-error tracking-action-error">{actionError}</p>}
        {order.status === 'Cancelled' ? <div className="tracking-cancelled-state"><span aria-hidden="true">×</span><div><strong>{status.sentence}</strong><p>{status.detail}</p></div></div> : <>
              <div className="tracking-progress-intro"><span className="tracking-live-dot" aria-hidden="true" /><div><strong>{status.title}</strong><p>{status.detail}</p></div></div>
          <ol className="tracking-timeline" aria-label="Order progress">{steps.map((step, index) => {
            const state = index < currentStepIndex ? 'completed' : index === currentStepIndex ? 'current' : 'future';
            const stepDetails = statusDetails(step, isDelivery);
            return <li className={`tracking-step tracking-step--${state}`} key={step}><span className="tracking-step-marker" aria-hidden="true">{state === 'completed' ? '✓' : index + 1}</span><div><strong>{stepDetails.title}</strong><p>{state === 'current' ? 'Currently here' : state === 'completed' ? 'Completed' : 'Waiting'}</p></div></li>;
          })}</ol>
        </>}
        {canSimulatePayment && <details className="dev-panel tracking-dev-panel"><summary>Development payment tools</summary><p>Test-only controls. No money is charged and these are hidden in production.</p><div className="button-row"><button className="button-secondary" onClick={() => simulatePayment(true)}>Simulate success</button><button className="button-quiet" onClick={() => simulatePayment(false)}>Simulate failure</button></div></details>}
      </section>
      <aside className="surface summary-card tracking-summary-card">
        <div className="tracking-summary-heading"><div><span className="eyebrow">Your order</span><h2>Order summary</h2></div><span className="tracking-payment">{order.paymentStatus ?? 'Pending'}</span></div>
        <ul className="tracking-items">{order.items.map((item) => <li key={`${item.productId}-${item.productName}`}><div><strong>{item.productName}</strong><span>Qty {item.quantity} × {formatMoney(item.unitPrice)}</span></div><b>{formatMoney(item.lineTotal)}</b></li>)}</ul>
        <div className="tracking-price-breakdown"><div><span>Subtotal</span><b>{formatMoney(order.subtotal)}</b></div><div><span>Delivery fee</span><b>{formatMoney(order.deliveryFee)}</b></div><div className="tracking-price-total"><span>Total</span><b>{formatMoney(order.total)}</b></div></div>
        {canCancel && <button className="button button-danger tracking-cancel-button" onClick={() => setIsCancelDialogOpen(true)}>Cancel order</button>}
        {(order.statusHistory?.length ?? 0) > 0 && <details className="tracking-history"><summary>View status history</summary><div>{order.statusHistory.map((history) => <p key={`${history.changedAtUtc}-${history.newStatus}`}>{history.previousStatus} → {history.newStatus}<span>{formatDate(history.changedAtUtc)} · {history.changedByName}</span></p>)}</div></details>}
      </aside>
    </div>
    <section className="tracking-help"><div><strong>Need help with this order?</strong><p>Our team can help with order questions or delivery details.</p></div><Link className="button-secondary" to="/contact">Contact support</Link></section>
    {isCancelDialogOpen && <div className="tracking-modal-backdrop" role="presentation" onMouseDown={(event) => { if (event.target === event.currentTarget && !isCancelling) setIsCancelDialogOpen(false); }}><section className="tracking-modal" role="dialog" aria-modal="true" aria-labelledby="cancel-order-title"><button className="tracking-modal-close" aria-label="Close cancellation dialog" disabled={isCancelling} onClick={() => setIsCancelDialogOpen(false)}>×</button><span className="eyebrow">Cancel order</span><h2 id="cancel-order-title">Cancel this order?</h2><p>Your reserved items will be returned to stock. This action cannot be undone.</p><div className="button-row"><button className="button-quiet" disabled={isCancelling} onClick={() => setIsCancelDialogOpen(false)}>Keep order</button><button className="button button-danger" disabled={isCancelling} onClick={confirmCancelOrder}>{isCancelling ? 'Cancelling…' : 'Yes, cancel order'}</button></div></section></div>}
  </main>;
}
