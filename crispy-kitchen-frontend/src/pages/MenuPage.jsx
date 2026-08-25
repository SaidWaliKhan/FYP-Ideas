import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import apiClient from '../api/apiClient';
import { useAuth } from '../hooks/useAuth';

const CART_STORAGE_KEY = 'ck_cart';

function readSavedCart() {
  try {
    const savedCart = JSON.parse(localStorage.getItem(CART_STORAGE_KEY) ?? '[]');
    return Array.isArray(savedCart) ? savedCart : [];
  } catch {
    return [];
  }
}

export default function MenuPage() {
  const navigate = useNavigate();
  const { token, role } = useAuth();

  const [products, setProducts] = useState([]);
  const [search, setSearch] = useState('');
  const [category, setCategory] = useState('');
  const [pageNumber, setPageNumber] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [cart, setCart] = useState(readSavedCart);
  const [error, setError] = useState('');
  const [fulfillmentType, setFulfillmentType] = useState('Pickup');
  const [contactPhone, setContactPhone] = useState('');
  const [deliveryAddress, setDeliveryAddress] = useState('');
  const [deliveryCity, setDeliveryCity] = useState('');
  const [isReviewing, setIsReviewing] = useState(false);
  const [loadedCartToken, setLoadedCartToken] = useState(null);

  useEffect(() => {
    apiClient.get('/menu', { params: { pageNumber, pageSize: 12, search: search || undefined, category: category || undefined } })
      .then((res) => { setProducts(res.data.items); setTotalPages(res.data.totalPages); });
  }, [pageNumber, search, category]);

  useEffect(() => {
    localStorage.setItem(CART_STORAGE_KEY, JSON.stringify(cart));
  }, [cart]);

  useEffect(() => {
    if (!token || role !== 'Customer') return;
    apiClient.get('/cart').then((res) => {
      setCart(res.data.items.map((item) => ({ productId: item.productId, name: item.productName, price: item.unitPrice, quantity: item.quantity })));
      setLoadedCartToken(token);
    });
  }, [token, role]);

  useEffect(() => {
    if (!token || role !== 'Customer' || loadedCartToken !== token) return;
    const saveTimer = window.setTimeout(() => {
      apiClient.put('/cart', { items: cart.map((item) => ({ productId: item.productId, quantity: item.quantity })) });
    }, 400);
    return () => window.clearTimeout(saveTimer);
  }, [cart, token, role, loadedCartToken]);

  function addToCart(product) {
    setCart((prev) => {
      const existing = prev.find((i) => i.productId === product.id);

      if (existing) {
        if (existing.quantity >= product.stockQuantity) return prev;

        return prev.map((i) =>
          i.productId === product.id
            ? { ...i, quantity: i.quantity + 1 }
            : i
        );
      }

      return [
        ...prev,
        {
          productId: product.id,
          name: product.name,
          price: product.price,
          quantity: 1,
        },
      ];
    });
  }

  function changeQuantity(productId, amount) {
    setCart((previous) => {
      const product = products.find((item) => item.id === productId);

      return previous.flatMap((item) => {
        if (item.productId !== productId) return [item];

        const quantity = item.quantity + amount;
        if (quantity <= 0) return [];
        if (product && quantity > product.stockQuantity) return [item];
        return [{ ...item, quantity }];
      });
    });
    setIsReviewing(false);
  }

  function removeFromCart(productId) {
    setCart((previous) => previous.filter((item) => item.productId !== productId));
    setIsReviewing(false);
  }

  const total = cart.reduce(
    (sum, i) => sum + i.price * i.quantity,
    0
  );
  const deliveryFee = fulfillmentType === 'Delivery' ? 1.50 : 0;
  const checkoutTotal = total + deliveryFee;

  function reviewOrder(event) {
    event.preventDefault();
    setError('');

    if (!token || role !== 'Customer') {
      navigate('/login');
      return;
    }

    setIsReviewing(true);
  }

  async function placeOrder() {
    setError('');

    try {
      const { data } = await apiClient.post('/orders', {
        items: cart.map((i) => ({
          productId: i.productId,
          quantity: i.quantity,
        })),
        fulfillmentType: fulfillmentType === 'Delivery' ? 0 : 1,
        contactPhone,
        deliveryAddress: fulfillmentType === 'Delivery' ? deliveryAddress : null,
        deliveryCity: fulfillmentType === 'Delivery' ? deliveryCity : null,
      });

      setCart([]);
      localStorage.removeItem(CART_STORAGE_KEY);
      try {
        await apiClient.put('/cart', { items: [] });
      } catch {
        // The order has already been created; cart synchronization will retry later.
      }

      navigate(`/orders/${data.id}`);
    } catch (err) {
      setError(
        err.response?.data?.error ?? 'Order failed.'
      );
    }
  }

  return (
    <div>
      <h2>Menu</h2>
      <input placeholder="Search menu" value={search} onChange={(e) => { setSearch(e.target.value); setPageNumber(1); }} />
      <select value={category} onChange={(e) => { setCategory(e.target.value); setPageNumber(1); }}><option value="">All categories</option>{['Chicken', 'Burgers', 'Sides', 'Drinks', 'Desserts'].map((item) => <option key={item}>{item}</option>)}</select>

      <ul>
        {products.map((p) => (
          <li key={p.id}>
            {p.name} — ${p.price.toFixed(2)}{' '}

            <button onClick={() => addToCart(p)} disabled={p.stockQuantity === 0 || cart.find((item) => item.productId === p.id)?.quantity >= p.stockQuantity}>
              {p.stockQuantity === 0 ? 'Sold out' : 'Add'}
            </button>
          </li>
        ))}
      </ul>
      <button onClick={() => setPageNumber((page) => page - 1)} disabled={pageNumber === 1}>Previous menu page</button>
      <span> Page {pageNumber} of {totalPages || 1} </span>
      <button onClick={() => setPageNumber((page) => page + 1)} disabled={pageNumber >= totalPages}>Next menu page</button>

      <h3>Cart — ${total.toFixed(2)}</h3>

      <ul>
        {cart.map((i) => (
          <li key={i.productId}>
            {i.name} × {i.quantity} — ${Number(i.price * i.quantity).toFixed(2)}
            <button onClick={() => changeQuantity(i.productId, -1)} aria-label={`Decrease ${i.name} quantity`}>−</button>
            <button onClick={() => changeQuantity(i.productId, 1)} aria-label={`Increase ${i.name} quantity`} disabled={i.quantity >= products.find((product) => product.id === i.productId)?.stockQuantity}>+</button>
            <button onClick={() => removeFromCart(i.productId)}>Remove</button>
          </li>
        ))}
      </ul>

      {cart.length > 0 && (
        <form onSubmit={reviewOrder}>
          <h3>Checkout</h3>

          <label>
            <input
              type="radio"
              name="fulfillmentType"
              checked={fulfillmentType === 'Pickup'}
              onChange={() => setFulfillmentType('Pickup')}
            />
            Pickup
          </label>

          <label>
            <input
              type="radio"
              name="fulfillmentType"
              checked={fulfillmentType === 'Delivery'}
              onChange={() => setFulfillmentType('Delivery')}
            />
            Delivery
          </label>

          <input
            type="tel"
            placeholder="Contact phone"
            value={contactPhone}
            onChange={(e) => setContactPhone(e.target.value)}
            required
          />

          {fulfillmentType === 'Delivery' && (
            <>
              <input
                placeholder="Delivery address"
                value={deliveryAddress}
                onChange={(e) => setDeliveryAddress(e.target.value)}
                required
              />
              <input
                placeholder="City"
                value={deliveryCity}
                onChange={(e) => setDeliveryCity(e.target.value)}
                required
              />
            </>
          )}

          <button type="submit">Review {fulfillmentType.toLowerCase()} order</button>
        </form>
      )}

      {isReviewing && (
        <section>
          <h3>Review your order</h3>
          <p><strong>Order type:</strong> {fulfillmentType}</p>
          <p><strong>Contact phone:</strong> {contactPhone}</p>
          {fulfillmentType === 'Delivery' && <p><strong>Delivery to:</strong> {deliveryAddress}, {deliveryCity}</p>}
          <p><strong>Items:</strong> ${total.toFixed(2)}</p>
          {fulfillmentType === 'Delivery' && <p><strong>Delivery fee:</strong> ${deliveryFee.toFixed(2)}</p>}
          <p><strong>Total:</strong> ${checkoutTotal.toFixed(2)}</p>
          <button onClick={() => setIsReviewing(false)}>Back to checkout</button>
          <button onClick={placeOrder}>Place order</button>
        </section>
      )}

      {error && (
        <p style={{ color: 'red' }}>
          {error}
        </p>
      )}
    </div>
  );
}
