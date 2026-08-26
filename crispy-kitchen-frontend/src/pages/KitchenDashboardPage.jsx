import { useCallback, useEffect, useState } from 'react';
import apiClient from '../api/apiClient';
import { useAuth } from '../hooks/useAuth';
import { OrderRealtimeConnection } from '../api/orderRealtime';

const NEXT_STATUS = {
  Pending: 'Confirmed',
  Confirmed: 'Preparing',
  Preparing: 'Ready',
  OutForDelivery: 'Delivered',
};

function getNextStatus(order) {
  if (order.status === 'Ready') {
    return order.fulfillmentType === 'Delivery' ? 'OutForDelivery' : 'Delivered';
  }

  return NEXT_STATUS[order.status];
}

// Must match CrispyKitchen.Domain.Enums.OrderStatus's numeric values exactly.
const STATUS_TO_INT = {
  Pending: 0, Confirmed: 1, Preparing: 2, Ready: 3, OutForDelivery: 4, Delivered: 5, Cancelled: 6,
};

const COLUMNS = ['Pending', 'Confirmed', 'Preparing', 'Ready', 'OutForDelivery'];

export default function KitchenDashboardPage() {
  const { token } = useAuth();
  const [orders, setOrders] = useState([]);
  const [error, setError] = useState('');
  const [pageNumber, setPageNumber] = useState(1);
  const [totalPages, setTotalPages] = useState(1);

  const loadOrders = useCallback(async () => {
    const { data } = await apiClient.get('/orders/active', { params: { pageNumber, pageSize: 50 } });
    setOrders(data.items);
    setTotalPages(data.totalPages);
  }, [pageNumber]);

  useEffect(() => {
    const loadTimer = window.setTimeout(() => {
      loadOrders();
    }, 0);
    const connection = new OrderRealtimeConnection(token);
    connection.on('OrderCreated', loadOrders);
    connection.on('OrderUpdated', loadOrders);
    connection.start()
      .then(() => connection.invoke('SubscribeToKitchen'))
      .catch((connectionError) => setError(connectionError.message));
    return () => {
      window.clearTimeout(loadTimer);
      connection.stop();
    };
  }, [loadOrders, token]);

  async function advance(order) {
    const nextStatus = getNextStatus(order);
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
    <main className="page"><div className="dashboard-header"><span className="eyebrow">Live kitchen</span><h2>Order board</h2><p className="muted">Keep every order moving, in real time.</p></div>
      {error && <p className="alert alert-error">{error}</p>}
      <div className="kitchen-board">
        {COLUMNS.map((status) => (
          <div key={status} className="kitchen-column">
            <h3>{status}<span className="badge">{orders.filter((o) => o.status === status).length}</span></h3>
            {orders.filter((o) => o.status === status).map((o) => (
              <div key={o.id} className="kitchen-ticket">
                <strong>#{o.id.slice(0, 8)}</strong>
                <ul className="ticket-items">
                  {o.items.map((i) => <li key={i.productId}>{i.quantity}x {i.productName}</li>)}
                </ul>
                {getNextStatus(o) && (
                  <button className="button" onClick={() => advance(o)}>Mark {getNextStatus(o)}</button>
                )}
              </div>
            ))}
          </div>
        ))}
      </div>
      <div className="pagination"><button className="button-quiet" onClick={() => setPageNumber((page) => page - 1)} disabled={pageNumber === 1}>Previous</button><span>Page {pageNumber} of {totalPages || 1}</span><button className="button-quiet" onClick={() => setPageNumber((page) => page + 1)} disabled={pageNumber >= totalPages}>Next</button></div></main>
  );
}
