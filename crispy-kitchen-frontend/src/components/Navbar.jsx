import { Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export default function Navbar() {
  const { token, role, logout } = useAuth();

  if (!token) return null;

  return (
    <nav style={{ display: 'flex', gap: '1rem', padding: '0.5rem 0' }}>
      <Link to="/menu">Menu</Link>
      {role === 'Customer' && <Link to="/orders/mine">My Orders</Link>}
      {(role === 'Admin' || role === 'KitchenStaff') && <Link to="/kitchen">Kitchen</Link>}
      <button onClick={logout}>Log out</button>
    </nav>
  );
}