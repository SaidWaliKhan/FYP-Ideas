import { Link } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';

export default function Navbar() {
  const { token, role, logout } = useAuth();

  return (
    <nav style={{ display: 'flex', gap: '1rem', padding: '0.5rem 0' }}>
      <Link to="/menu">Menu</Link>
      {token ? (
        <>
          {role === 'Customer' && <Link to="/orders/mine">My Orders</Link>}
          {(role === 'Admin' || role === 'KitchenStaff') && <Link to="/kitchen">Kitchen</Link>}
          {role === 'Admin' && <Link to="/admin">Admin</Link>}
          <button onClick={logout}>Log out</button>
        </>
      ) : (
        <>
          <Link to="/login">Log in</Link>
          <Link to="/register">Register</Link>
        </>
      )}
    </nav>
  );
}
