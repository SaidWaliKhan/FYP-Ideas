import { useEffect } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';

export default function SessionExpiryHandler() {
  const { logout } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();

  useEffect(() => {
    const handleExpiredSession = () => {
      logout();
      const from = `${location.pathname}${location.search}`;
      navigate('/login', { replace: true, state: { from, message: 'Your session expired. Please log in again.' } });
    };
    window.addEventListener('ck:session-expired', handleExpiredSession);
    return () => window.removeEventListener('ck:session-expired', handleExpiredSession);
  }, [location.pathname, location.search, logout, navigate]);

  return null;
}
