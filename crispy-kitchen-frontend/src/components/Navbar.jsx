import { Link, NavLink } from 'react-router-dom';
import { useState } from 'react';
import { useAuth } from '../hooks/useAuth';
import MenuSearchOverlay from './MenuSearchOverlay';

function SearchIcon() { return <svg viewBox="0 0 24 24" aria-hidden="true"><circle cx="11" cy="11" r="6.5" /><path d="m16 16 4.2 4.2" /></svg>; }
function CartIcon() { return <svg viewBox="0 0 24 24" aria-hidden="true"><path d="M3 4h2l1.8 10.2h10.8L20 7H7" /><circle cx="9" cy="19" r="1" /><circle cx="17" cy="19" r="1" /></svg>; }

export default function Navbar() {
  const { token, role, logout } = useAuth();
  const [isOpen, setIsOpen] = useState(false);
  const [isSearchOpen, setIsSearchOpen] = useState(false);
  const closeMenu = () => setIsOpen(false);

  return (
    <nav className="navbar">
      <Link className="brand" to="/" onClick={closeMenu}>Crispy<span>Kitchen</span></Link>
      <button className="nav-toggle" onClick={() => setIsOpen((open) => !open)} aria-label="Toggle navigation">☰</button>
      <div className={`nav-links ${isOpen ? 'open' : ''}`}>
        <NavLink to="/" end onClick={closeMenu}>Home</NavLink><NavLink to="/menu" onClick={closeMenu}>Menu</NavLink>
      {token ? (
        <>
          {role === 'Customer' && <NavLink to="/orders/mine" onClick={closeMenu}>My orders</NavLink>}
          {(role === 'Admin' || role === 'KitchenStaff') && <Link to="/kitchen" onClick={closeMenu}>Kitchen</Link>}
          {role === 'Admin' && <Link to="/admin" onClick={closeMenu}>Admin</Link>}
        </>
      ) : <NavLink to="/orders/mine" onClick={closeMenu}>My orders</NavLink>}<NavLink to="/about" onClick={closeMenu}>About</NavLink><NavLink to="/contact" onClick={closeMenu}>Contact</NavLink>
      </div>
      <div className="nav-actions"><button className="nav-icon" type="button" onClick={() => setIsSearchOpen(true)} aria-label="Search menu"><SearchIcon /></button><Link className="nav-icon nav-cart" to="/menu" aria-label="Open cart"><CartIcon /></Link>{token ? <button className="button-quiet" onClick={logout}>Log out</button> : <Link className="button register-link" to="/login">Log in</Link>}</div>
      {isSearchOpen && <MenuSearchOverlay onClose={() => setIsSearchOpen(false)} />}</nav>
  );
}
