import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import Navbar from './components/Navbar';
import ProtectedRoute from './components/ProtectedRoute';
import LoginPage from './pages/LoginPage';
import ForgotPasswordPage from './pages/ForgotPasswordPage';
import RegisterPage from './pages/RegisterPage';
import MenuPage from './pages/MenuPage';
import MyOrdersPage from './pages/MyOrdersPage';
import OrderTrackingPage from './pages/OrderTrackingPage';
import KitchenDashboardPage from './pages/KitchenDashboardPage';
import AdminDashboardPage from './pages/AdminDashboardPage';
import HomePage from './pages/HomePage';
import Footer from './components/Footer';
import AboutPage from './pages/AboutPage';
import ContactPage from './pages/ContactPage';
import CheckoutPage from './pages/CheckoutPage';
import OrderConfirmationPage from './pages/OrderConfirmationPage';
import NotFoundPage from './pages/NotFoundPage';
import SessionExpiryHandler from './components/SessionExpiryHandler';

export default function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <SessionExpiryHandler />
        <div className="site-shell"><Navbar />
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/about" element={<AboutPage />} />
          <Route path="/contact" element={<ContactPage />} />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/forgot-password" element={<ForgotPasswordPage />} />
          <Route path="/register" element={<RegisterPage />} />

          <Route path="/menu" element={<MenuPage />} />
          <Route path="/checkout" element={<CheckoutPage />} />

          <Route path="/orders/mine" element={
            <ProtectedRoute allowedRoles={['Customer']}><MyOrdersPage /></ProtectedRoute>
          } />

          <Route path="/orders/:id/confirmation" element={
            <ProtectedRoute allowedRoles={['Customer']}><OrderConfirmationPage /></ProtectedRoute>
          } />

          {/* :id is a URL parameter — useParams() reads it inside the page */}
          <Route path="/orders/:id" element={
            <ProtectedRoute><OrderTrackingPage /></ProtectedRoute>
          } />

          <Route path="/kitchen" element={
            <ProtectedRoute allowedRoles={['Admin', 'KitchenStaff']}><KitchenDashboardPage /></ProtectedRoute>
          } />

          <Route path="/admin" element={
            <ProtectedRoute allowedRoles={['Admin']}><AdminDashboardPage /></ProtectedRoute>
          } />

          <Route path="*" element={<NotFoundPage />} />
        </Routes><Footer /></div>
      </AuthProvider>
    </BrowserRouter>
  );
}
