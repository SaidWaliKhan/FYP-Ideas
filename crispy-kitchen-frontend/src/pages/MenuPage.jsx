import { useEffect, useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import apiClient from '../api/apiClient';
import { useAuth } from '../hooks/useAuth';
import foodHero from '../assets/food-hero.png';

const CART_STORAGE_KEY = 'ck_cart';
const categoryImages = { Burgers: 'https://images.unsplash.com/photo-1568901346375-23c9450c58cd?auto=format&fit=crop&w=800&q=80', Chicken: 'https://images.unsplash.com/photo-1562967914-608f82629710?auto=format&fit=crop&w=800&q=80', Sides: 'https://images.unsplash.com/photo-1573080496219-bb080dd4f877?auto=format&fit=crop&w=800&q=80', Drinks: 'https://images.unsplash.com/photo-1551024506-0bccd828d307?auto=format&fit=crop&w=800&q=80', Desserts: 'https://images.unsplash.com/photo-1551024506-0bccd828d307?auto=format&fit=crop&w=800&q=80' };
function readSavedCart() { try { const saved = JSON.parse(localStorage.getItem(CART_STORAGE_KEY) ?? '[]'); return Array.isArray(saved) ? saved : []; } catch { return []; } }

export default function MenuPage() {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const { token, role } = useAuth();
  const [products, setProducts] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [search, setSearch] = useState(() => searchParams.get('search') ?? '');
  const [category, setCategory] = useState(() => searchParams.get('category') ?? '');
  const [pageNumber, setPageNumber] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [cart, setCart] = useState(readSavedCart);
  const [error, setError] = useState('');
  const [loadedCartToken, setLoadedCartToken] = useState(null);

  useEffect(() => {
    const syncTimer = window.setTimeout(() => {
      setSearch(searchParams.get('search') ?? '');
      setCategory(searchParams.get('category') ?? '');
      setPageNumber(1);
    }, 0);
    return () => window.clearTimeout(syncTimer);
  }, [searchParams]);

  useEffect(() => {
    const timer = window.setTimeout(() => { setIsLoading(true); setError(''); apiClient.get('/menu', { params: { pageNumber, pageSize: 12, search: search || undefined, category: category || undefined } }).then((res) => { setProducts(res.data.items); setTotalPages(res.data.totalPages); }).catch((err) => setError(err.response?.data?.error ?? 'Could not load the menu.')).finally(() => setIsLoading(false)); }, 0);
    return () => window.clearTimeout(timer);
  }, [pageNumber, search, category]);
  useEffect(() => { localStorage.setItem(CART_STORAGE_KEY, JSON.stringify(cart)); window.dispatchEvent(new Event('storage')); }, [cart]);
  useEffect(() => { if (!token || role !== 'Customer') return; apiClient.get('/cart').then((res) => { const savedImages = Object.fromEntries(readSavedCart().map((item) => [item.productId, item.imageUrl])); setCart(res.data.items.map((item) => ({ productId: item.productId, name: item.productName, price: item.unitPrice, quantity: item.quantity, imageUrl: savedImages[item.productId] }))); setLoadedCartToken(token); }).catch(() => {}); }, [token, role]);
  useEffect(() => { if (!token || role !== 'Customer' || loadedCartToken !== token) return; const saveTimer = window.setTimeout(() => { apiClient.put('/cart', { items: cart.map((item) => ({ productId: item.productId, quantity: item.quantity })) }).catch(() => {}); }, 400); return () => window.clearTimeout(saveTimer); }, [cart, token, role, loadedCartToken]);

  function addToCart(product) { setCart((previous) => { const existing = previous.find((item) => item.productId === product.id); if (existing) { if (existing.quantity >= product.stockQuantity) return previous; return previous.map((item) => item.productId === product.id ? { ...item, quantity: item.quantity + 1 } : item); } return [...previous, { productId: product.id, name: product.name, price: product.price, imageUrl: product.imageUrl || categoryImages[product.category] || foodHero, quantity: 1 }]; }); }
  function changeQuantity(productId, amount) { setCart((previous) => previous.flatMap((item) => { if (item.productId !== productId) return [item]; const product = products.find((candidate) => candidate.id === productId); const quantity = item.quantity + amount; if (quantity <= 0) return []; if (product && quantity > product.stockQuantity) return [item]; return [{ ...item, quantity }]; })); }
  function removeFromCart(productId) { setCart((previous) => previous.filter((item) => item.productId !== productId)); }
  function beginCheckout() { if (!token || role !== 'Customer') { navigate('/login', { state: { from: '/checkout' } }); return; } navigate('/checkout'); }
  const subtotal = cart.reduce((sum, item) => sum + item.price * item.quantity, 0);
  const itemCount = cart.reduce((sum, item) => sum + item.quantity, 0);

  return <main className="page"><section className="menu-intro"><span className="eyebrow">Our menu</span><h1>Made fresh. Served fast.</h1><p>Choose your favourites, then make it yours.</p></section><div className="menu-layout"><section><div className="menu-toolbar"><div className="search-field"><span>⌕</span><input className="filter-input" placeholder="Search the menu" value={search} onChange={(event) => { setSearch(event.target.value); setPageNumber(1); }} />{search && <button className="search-clear" onClick={() => setSearch('')} aria-label="Clear search">×</button>}</div><button className="filter-button" onClick={() => { setSearch(''); setCategory(''); setPageNumber(1); }}>Reset filters</button></div><div className="category-chips"><button className={!category ? 'chip active' : 'chip'} onClick={() => { setCategory(''); setPageNumber(1); }}>All items</button>{['Chicken', 'Burgers', 'Sides', 'Drinks', 'Desserts'].map((item) => <button className={category === item ? 'chip active' : 'chip'} onClick={() => { setCategory(item); setPageNumber(1); }} key={item}>{item}</button>)}</div><div className="product-grid">{isLoading ? Array.from({ length: 6 }).map((_, index) => <div className="surface product-card skeleton" key={index} />) : products.map((product) => <article className="surface product-card" key={product.id}><div className="product-image"><img src={product.imageUrl || categoryImages[product.category] || foodHero} alt={product.name} /><div className="product-badges">{product.isFeatured && <span className="badge badge-orange">Featured</span>}{(!product.isAvailable || product.stockQuantity === 0) && <span className="badge badge-red">{product.stockQuantity === 0 ? 'Sold out' : 'Unavailable'}</span>}</div></div><div className="product-card__body"><span className="product-category">{product.category}</span><h3>{product.name}</h3><p>{product.description || 'Made fresh to order.'}</p><div className="product-card__footer"><span className="price">${product.price.toFixed(2)}</span><button className="button" onClick={() => addToCart(product)} disabled={!product.isAvailable || product.stockQuantity === 0 || cart.find((item) => item.productId === product.id)?.quantity >= product.stockQuantity}>{!product.isAvailable ? 'Unavailable' : product.stockQuantity === 0 ? 'Sold out' : 'Add +'}</button></div></div></article>)}</div>{!isLoading && products.length === 0 && <div className="surface empty-state"><strong>No matches found</strong><p>Try another search or browse the full menu.</p><button className="button-secondary" onClick={() => { setSearch(''); setCategory(''); }}>Show all items</button></div>}<div className="pagination"><button className="button-quiet" onClick={() => setPageNumber((page) => page - 1)} disabled={pageNumber === 1}>Previous</button><span>Page {pageNumber} of {totalPages || 1}</span><button className="button-quiet" onClick={() => setPageNumber((page) => page + 1)} disabled={pageNumber >= totalPages}>Next</button></div></section><aside className="surface cart-panel"><div className="cart-head"><div><span className="eyebrow">Quick cart</span><h3>Your cart</h3></div><span className="badge badge-orange">{itemCount} items</span></div>{cart.length === 0 ? <div className="empty-state cart-empty"><strong>Your cart is empty</strong><p>Add something delicious to get started.</p><button className="button-secondary" onClick={() => document.querySelector('.menu-intro')?.scrollIntoView({ behavior: 'smooth' })}>Browse menu</button></div> : <><ul className="cart-items">{cart.map((item) => <li className="cart-item" key={item.productId}><img src={item.imageUrl || foodHero} alt="" /><div className="cart-item__content"><div className="cart-item__top"><div><strong>{item.name}</strong><small>{item.quantity} × ${Number(item.price).toFixed(2)}</small></div><span>${Number(item.price * item.quantity).toFixed(2)}</span></div><div className="cart-item__actions"><button className="icon-button" onClick={() => changeQuantity(item.productId, -1)} aria-label={`Decrease ${item.name} quantity`}>−</button><span className="qty">{item.quantity}</span><button className="icon-button" onClick={() => changeQuantity(item.productId, 1)} aria-label={`Increase ${item.name} quantity`} disabled={item.quantity >= products.find((product) => product.id === item.productId)?.stockQuantity}>+</button><button className="button-quiet" onClick={() => removeFromCart(item.productId)}>Remove</button></div></div></li>)}</ul><div className="cart-total"><span>Subtotal</span><strong>${subtotal.toFixed(2)}</strong></div><button className="button cart-checkout-button" onClick={beginCheckout}>Checkout <span>→</span></button><p className="cart-checkout-note">Choose pickup or delivery at checkout.</p></>}</aside></div>{error && <p className="alert alert-error">{error}</p>}</main>;
}
