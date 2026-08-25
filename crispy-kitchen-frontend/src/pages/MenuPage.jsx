import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import apiClient from '../api/apiClient';

export default function MenuPage() {
  const navigate = useNavigate();

  const [products, setProducts] = useState([]);
  const [cart, setCart] = useState([]);
  const [error, setError] = useState('');

  useEffect(() => {
    apiClient.get('/menu').then((res) => setProducts(res.data));
  }, []);

  function addToCart(product) {
    setCart((prev) => {
      const existing = prev.find((i) => i.productId === product.id);

      if (existing) {
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

  const total = cart.reduce(
    (sum, i) => sum + i.price * i.quantity,
    0
  );

  async function placeOrder() {
    setError('');

    try {
      const { data } = await apiClient.post('/orders', {
        items: cart.map((i) => ({
          productId: i.productId,
          quantity: i.quantity,
        })),

        fulfillmentType: 1,
        contactPhone: '0300-0000000',
      });

      setCart([]);

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

      <ul>
        {products.map((p) => (
          <li key={p.id}>
            {p.name} — ${p.price.toFixed(2)}{' '}

            <button onClick={() => addToCart(p)}>
              Add
            </button>
          </li>
        ))}
      </ul>

      <h3>Cart — ${total.toFixed(2)}</h3>

      <ul>
        {cart.map((i) => (
          <li key={i.productId}>
            {i.name} × {i.quantity}
          </li>
        ))}
      </ul>

      {cart.length > 0 && (
        <button onClick={placeOrder}>
          Place order (pickup, test)
        </button>
      )}

      {error && (
        <p style={{ color: 'red' }}>
          {error}
        </p>
      )}
    </div>
  );
}