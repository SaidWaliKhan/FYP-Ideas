import { Navigate, useLocation } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';
import { AccessDeniedState } from './AsyncStates';

export default function ProtectedRoute({ children, allowedRoles }) {
  const { token, role } = useAuth();
  const location = useLocation();

  if (!token) {
    return <Navigate to="/login" replace state={{ from: `${location.pathname}${location.search}`, message: 'Please log in to continue.' }} />;
  }

  if (allowedRoles && !allowedRoles.includes(role)) {
    return <AccessDeniedState />;
  }

  return children;
}
