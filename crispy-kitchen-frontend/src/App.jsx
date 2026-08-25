import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import Navbar from './components/Navbar';
import ProtectedRoute from './components/ProtectedRoute';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import MenuPage from './pages/MenuPage';
import MyOrdersPage from './pages/MyOrdersPage';
import OrderTrackingPage from './pages/OrderTrackingPage';
import KitchenDashboardPage from './pages/KitchenDashboardPage';

export default function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Navbar />
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />

          <Route path="/menu" element={
            <ProtectedRoute><MenuPage /></ProtectedRoute>
          } />

          <Route path="/orders/mine" element={
            <ProtectedRoute allowedRoles={['Customer']}><MyOrdersPage /></ProtectedRoute>
          } />

          {/* :id is a URL parameter — useParams() reads it inside the page */}
          <Route path="/orders/:id" element={
            <ProtectedRoute><OrderTrackingPage /></ProtectedRoute>
          } />

          <Route path="/kitchen" element={
            <ProtectedRoute allowedRoles={['Admin', 'KitchenStaff']}><KitchenDashboardPage /></ProtectedRoute>
          } />

          {/* Catch-all: unknown URLs redirect to /menu instead of a blank page */}
          <Route path="*" element={<Navigate to="/menu" replace />} />
        </Routes>
      </AuthProvider>
    </BrowserRouter>
  );
}