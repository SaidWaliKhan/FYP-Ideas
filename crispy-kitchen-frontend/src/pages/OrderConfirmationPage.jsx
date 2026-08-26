import { useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import apiClient from '../api/apiClient';

const money = (value) => `$${Number(value ?? 0).toFixed(2)}`;
const dateTime = (value) => (value ? new Date(value).toLocaleString([], { dateStyle: 'medium', timeStyle: 'short' }) : '—');

export default function OrderConfirmationPage() {
  const { id } = useParams();
  const [order, setOrder] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [hasError, setHasError] = useState(false);
  const [retryKey, setRetryKey] = useState(0);

  useEffect(() => {
    let active = true;
    apiClient.get(`/orders/${id}`)
      .then(({ data }) => { if (active) setOrder(data); })
      .catch(() => { if (active) setHasError(true); })
      .finally(() => { if (active) setIsLoading(false); });
    return () => { active = false; };
  }, [id, retryKey]);

  function retry() { setIsLoading(true); setHasError(false); setRetryKey((key) => key + 1); }

  if (isLoading) return <main className="page confirmation-page"><section className="surface confirmation-skeleton" aria-label="Loading order confirmation"><span /><span /><div /><div /></section></main>;
  if (hasError || !order) return <main className="page confirmation-page"><section className="surface confirmation-error"><span className="eyebrow">Order confirmation</span><h1>Order details unavailable</h1><p>We couldn’t load this order right now.</p><div className="button-row"><button className="button" type="button" onClick={retry}>Try again</button><Link className="button-secondary" to="/orders/mine">My orders</Link></div></section></main>;

  const isDelivery = order.fulfillmentType === 'Delivery';
  const itemCount = order.items.reduce((sum, item) => sum + item.quantity, 0);
  return <main className="page confirmation-page"><section className="confirmation-hero"><span className="confirmation-check" aria-hidden="true">✓</span><div><span className="eyebrow">Order received</span><h1>Order confirmed!</h1><p>We’ve received your order and will start processing it shortly.</p></div></section><section className="surface confirmation-number"><span>Your order number</span><strong>Order #{order.id.slice(0, 8)}</strong><p>{isDelivery ? 'We’ll prepare your order and keep you updated as it moves through delivery.' : 'We’ll let you know when your order is ready for pickup.'}</p></section><div className="confirmation-layout"><section className="surface confirmation-details"><div className="confirmation-card-heading"><div><span className="eyebrow">Order details</span><h2>Your order at a glance</h2></div><span className="confirmation-payment">{order.paymentStatus ?? 'Pending'}</span></div><dl className="confirmation-meta"><div><dt>Fulfillment</dt><dd>{order.fulfillmentType ?? 'Pickup'}</dd></div><div><dt>Order status</dt><dd>{order.status ?? 'Pending'}</dd></div><div><dt>Payment status</dt><dd>{order.paymentStatus ?? 'Pending'}</dd></div><div><dt>Contact</dt><dd>{order.contactPhone || '—'}</dd></div><div><dt>Placed</dt><dd>{dateTime(order.placedAtUtc)}</dd></div>{isDelivery && <div className="confirmation-meta-wide"><dt>Delivery address</dt><dd>{[order.deliveryAddress, order.deliveryCity].filter(Boolean).join(', ') || '—'}</dd></div>}</dl></section><aside className="surface confirmation-summary"><div className="confirmation-card-heading"><div><span className="eyebrow">Order summary</span><h2>{itemCount} {itemCount === 1 ? 'item' : 'items'}</h2></div></div><ul className="confirmation-items">{order.items.map((item) => <li key={`${item.productId}-${item.productName}`}><div><strong>{item.productName}</strong><span>Qty {item.quantity} × {money(item.unitPrice)}</span></div><b>{money(item.lineTotal)}</b></li>)}</ul><div className="confirmation-totals"><div><span>Subtotal</span><b>{money(order.subtotal)}</b></div><div><span>Delivery fee</span><b>{money(order.deliveryFee)}</b></div><div className="confirmation-total"><span>Total</span><b>{money(order.total)}</b></div></div></aside></div><section className="confirmation-actions"><div><strong>What happens next?</strong><p>{isDelivery ? 'Follow each update from preparation to delivery.' : 'Follow preparation updates and collect your meal when it is ready.'}</p></div><div className="button-row"><Link className="button" to={`/orders/${order.id}`}>Track my order <span>→</span></Link><Link className="button-secondary" to="/menu">Back to menu</Link></div></section></main>;
}
