import { useEffect, useRef, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import apiClient from '../api/apiClient';
import foodHero from '../assets/food-hero.png';

export default function MenuSearchOverlay({ onClose }) {
  const navigate = useNavigate();
  const inputRef = useRef(null);
  const [query, setQuery] = useState('');
  const [category, setCategory] = useState('');
  const [categories, setCategories] = useState([]);
  const [results, setResults] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(false);
  const [retryKey, setRetryKey] = useState(0);

  useEffect(() => {
    inputRef.current?.focus();
    const previousOverflow = document.body.style.overflow;
    document.body.style.overflow = 'hidden';
    const escape = (event) => { if (event.key === 'Escape') onClose(); };
    window.addEventListener('keydown', escape);
    return () => { document.body.style.overflow = previousOverflow; window.removeEventListener('keydown', escape); };
  }, [onClose]);

  useEffect(() => {
    let active = true;
    apiClient.get('/menu', { params: { pageNumber: 1, pageSize: 100 } })
      .then(({ data }) => { if (active) setCategories([...new Set((data.items ?? []).map((item) => item.category).filter(Boolean))]); })
      .catch(() => { if (active) setError(true); });
    return () => { active = false; };
  }, []);

  useEffect(() => {
    const trimmedQuery = query.trim();
    if (!trimmedQuery && !category) return undefined;
    const timer = window.setTimeout(() => {
      setLoading(true); setError(false);
      apiClient.get('/menu', { params: { pageNumber: 1, pageSize: 8, search: trimmedQuery || undefined, category: category || undefined } })
        .then(({ data }) => setResults(data.items ?? []))
        .catch(() => { setResults([]); setError(true); })
        .finally(() => setLoading(false));
    }, trimmedQuery ? 220 : 0);
    return () => window.clearTimeout(timer);
  }, [query, category, retryKey]);

  const resetSearch = () => { setQuery(''); setCategory(''); setResults([]); setError(false); inputRef.current?.focus(); };
  const viewAll = () => { const params = new URLSearchParams(); if (query.trim()) params.set('search', query.trim()); if (category) params.set('category', category); navigate(`/menu${params.size ? `?${params}` : ''}`); onClose(); };
  const selectCategory = (nextCategory) => { setCategory(nextCategory); setQuery(''); };
  const hasSearch = Boolean(query.trim() || category);

  return <div className="menu-search-overlay" role="dialog" aria-modal="true" aria-labelledby="menu-search-title" onMouseDown={(event) => { if (event.target === event.currentTarget) onClose(); }}><section className="menu-search-modal"><div className="menu-search-head"><div><span className="eyebrow">Search the menu</span><h2 id="menu-search-title">Find something delicious</h2></div><button className="icon-button menu-search-close" type="button" onClick={onClose} aria-label="Close search">×</button></div><div className="menu-search-input"><span aria-hidden="true">⌕</span><input ref={inputRef} value={query} onChange={(event) => { setQuery(event.target.value); setCategory(''); }} placeholder="Search burgers, wings, drinks..." aria-label="Search menu products" />{hasSearch && <button type="button" className="search-clear" onClick={resetSearch} aria-label="Clear search">Clear</button>}</div>{!hasSearch && !error && <div className="search-empty search-categories"><span className="eyebrow">Browse categories</span><p>Choose a category or start typing to search the full menu.</p><div>{categories.map((item) => <button type="button" key={item} onClick={() => selectCategory(item)}>{item}</button>)}</div></div>}{loading && <div className="search-loading" aria-live="polite"><span className="search-spinner" aria-hidden="true" /> Searching the menu…</div>}{error && <div className="search-empty search-error" role="status"><strong>We couldn’t load search results.</strong><p>Please check your connection and try again.</p><div><button className="button-secondary" type="button" onClick={() => setRetryKey((key) => key + 1)}>Try again</button><button className="button-quiet" type="button" onClick={viewAll}>Browse full menu</button></div></div>}{hasSearch && !loading && !error && results.length === 0 && <div className="search-empty"><strong>No dishes found</strong><p>We couldn’t find anything matching “{query.trim() || category}”.</p><div><button className="button-secondary" type="button" onClick={resetSearch}>Clear search</button><button className="button-quiet" type="button" onClick={() => { navigate('/menu'); onClose(); }}>Browse full menu</button></div></div>}{results.length > 0 && !error && <div className="search-results" aria-live="polite">{results.map((product) => <button className="search-result" type="button" key={product.id} onClick={() => { navigate(`/menu?search=${encodeURIComponent(product.name)}`); onClose(); }} aria-label={`View ${product.name} on the menu`}><img src={product.imageUrl || foodHero} alt="" /><span><small>{product.category}</small><strong>{product.name}</strong><em className={product.isAvailable ? 'available' : 'unavailable'}>{product.isAvailable ? 'Available' : 'Unavailable'}</em></span><b>${Number(product.price).toFixed(2)}</b></button>)}</div>}{hasSearch && results.length > 0 && !error && <button className="view-results" type="button" onClick={viewAll}>View all results <span>→</span></button>}</section></div>;
}
