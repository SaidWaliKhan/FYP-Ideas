import { useCallback, useEffect, useState } from 'react';
import apiClient from '../api/apiClient';

const CATEGORIES = ['Chicken', 'Burgers', 'Sides', 'Drinks', 'Desserts'];

const emptyProduct = {
  name: '',
  description: '',
  price: '',
  category: 0,
  imageUrl: '',
  stockQuantity: '',
  isFeatured: false,
};

const emptyStaff = {
  fullName: '',
  email: '',
  password: '',
  role: 2,
};

export default function AdminDashboardPage() {
  const [products, setProducts] = useState([]);
  const [staffUsers, setStaffUsers] = useState([]);
  const [productSearch, setProductSearch] = useState('');
  const [pageNumber, setPageNumber] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [product, setProduct] = useState(emptyProduct);
  const [editingProductId, setEditingProductId] = useState(null);
  const [staff, setStaff] = useState(emptyStaff);
  const [error, setError] = useState('');
  const [message, setMessage] = useState('');
  const [loading, setLoading] = useState(true);

  const loadProducts = useCallback(async () => {
    try {
      setLoading(true);
      const { data } = await apiClient.get('/menu/all', { params: { pageNumber, pageSize: 20, search: productSearch || undefined } });
      setProducts(data.items);
      setTotalPages(data.totalPages);
    } catch (err) {
      setError(err.response?.data?.error ?? 'Could not load products.');
    } finally {
      setLoading(false);
    }
  }, [pageNumber, productSearch]);

  const loadStaff = useCallback(async () => {
    try {
      const { data } = await apiClient.get('/users/staff');
      setStaffUsers(data);
    } catch (err) {
      setError(err.response?.data?.error ?? 'Could not load staff accounts.');
    }
  }, []);

  useEffect(() => {
    const loadTimer = window.setTimeout(() => {
      loadProducts();
      loadStaff();
    }, 0);

    return () => window.clearTimeout(loadTimer);
  }, [loadProducts, loadStaff]);

  function showError(err, fallback) {
    setMessage('');
    setError(err.response?.data?.error ?? fallback);
  }

  async function saveProduct(event) {
    event.preventDefault();
    setError('');

    try {
      const productDetails = {
        name: product.name,
        description: product.description,
        price: Number(product.price),
        category: Number(product.category),
        imageUrl: product.imageUrl || null,
        isFeatured: product.isFeatured,
      };

      if (editingProductId) {
        await apiClient.put(`/menu/${editingProductId}`, { id: editingProductId, ...productDetails });
      } else {
        await apiClient.post('/menu', { ...productDetails, stockQuantity: Number(product.stockQuantity) });
      }

      setProduct(emptyProduct);
      setEditingProductId(null);
      setMessage(editingProductId ? 'Product updated.' : 'Product created.');
      await loadProducts();
    } catch (err) {
      showError(err, 'Could not create product.');
    }
  }

  function startEditing(item) {
    setError('');
    setMessage('');
    setEditingProductId(item.id);
    setProduct({
      name: item.name,
      description: item.description,
      price: item.price,
      category: CATEGORIES.indexOf(item.category),
      imageUrl: item.imageUrl ?? '',
      stockQuantity: item.stockQuantity,
      isFeatured: item.isFeatured,
    });
  }

  function cancelEditing() {
    setEditingProductId(null);
    setProduct(emptyProduct);
  }

  async function setAvailability(item) {
    setError('');
    try {
      await apiClient.patch(`/menu/${item.id}/availability`, {
        id: item.id,
        isAvailable: !item.isAvailable,
      });
      setMessage(`${item.name} is now ${item.isAvailable ? 'hidden' : 'available'}.`);
      await loadProducts();
    } catch (err) {
      showError(err, 'Could not update availability.');
    }
  }

  async function restock(item) {
    const answer = window.prompt(`How many ${item.name} items should be added?`, '1');
    const quantity = Number(answer);

    if (!Number.isInteger(quantity) || quantity <= 0) return;

    setError('');
    try {
      await apiClient.patch(`/menu/${item.id}/restock`, { id: item.id, quantity });
      setMessage(`${item.name} restocked by ${quantity}.`);
      await loadProducts();
    } catch (err) {
      showError(err, 'Could not restock product.');
    }
  }

  async function createStaff(event) {
    event.preventDefault();
    setError('');

    try {
      await apiClient.post('/users/staff', staff);
      setStaff(emptyStaff);
      setMessage('Staff account created.');
      await loadStaff();
    } catch (err) {
      showError(err, 'Could not create staff account.');
    }
  }

  async function updateStaffRole(user, role) {
    try {
      await apiClient.patch(`/users/staff/${user.id}/role`, { id: user.id, role: Number(role) });
      setMessage('Staff role updated.');
      await loadStaff();
    } catch (err) { showError(err, 'Could not update staff role.'); }
  }

  async function setStaffActive(user) {
    try {
      await apiClient.patch(`/users/staff/${user.id}/active`, { id: user.id, isActive: !user.isActive });
      setMessage(`Staff account ${user.isActive ? 'deactivated' : 'activated'}.`);
      await loadStaff();
    } catch (err) { showError(err, 'Could not update staff account.'); }
  }

  async function resetStaffPassword(user) {
    const newPassword = window.prompt(`New password for ${user.fullName}:`);
    if (!newPassword) return;

    try {
      await apiClient.post(`/users/staff/${user.id}/reset-password`, { id: user.id, newPassword });
      setMessage(`Password reset for ${user.fullName}.`);
    } catch (err) { showError(err, 'Could not reset the password.'); }
  }

  async function deleteStaff(user) {
    if (!window.confirm(`Permanently delete ${user.fullName}'s staff account?`)) return;

    try {
      await apiClient.delete(`/users/staff/${user.id}`);
      setMessage('Staff account deleted.');
      await loadStaff();
    } catch (err) { showError(err, 'Could not delete the staff account.'); }
  }

  return (
    <div>
      <h2>Admin Dashboard</h2>
      {error && <p style={{ color: 'red' }}>{error}</p>}
      {message && <p style={{ color: 'green' }}>{message}</p>}

      <section>
        <h3>{editingProductId ? 'Edit menu product' : 'Create a menu product'}</h3>
        <form onSubmit={saveProduct}>
          <input placeholder="Name" value={product.name} onChange={(e) => setProduct({ ...product, name: e.target.value })} required />
          <input placeholder="Description" value={product.description} onChange={(e) => setProduct({ ...product, description: e.target.value })} required />
          <input type="number" min="0.01" step="0.01" placeholder="Price" value={product.price} onChange={(e) => setProduct({ ...product, price: e.target.value })} required />
          {!editingProductId && <input type="number" min="0" step="1" placeholder="Starting stock" value={product.stockQuantity} onChange={(e) => setProduct({ ...product, stockQuantity: e.target.value })} required />}
          <select value={product.category} onChange={(e) => setProduct({ ...product, category: Number(e.target.value) })}>
            {CATEGORIES.map((category, index) => <option key={category} value={index}>{category}</option>)}
          </select>
          <input type="url" placeholder="Image URL (optional)" value={product.imageUrl} onChange={(e) => setProduct({ ...product, imageUrl: e.target.value })} />
          <label><input type="checkbox" checked={product.isFeatured} onChange={(e) => setProduct({ ...product, isFeatured: e.target.checked })} /> Featured</label>
          <button type="submit">{editingProductId ? 'Save changes' : 'Create product'}</button>
          {editingProductId && <button type="button" onClick={cancelEditing}>Cancel edit</button>}
        </form>
      </section>

      <section>
        <h3>Inventory</h3>
        <input placeholder="Search inventory" value={productSearch} onChange={(e) => { setProductSearch(e.target.value); setPageNumber(1); }} />
        {loading ? <p>Loading products...</p> : <ul>
          {products.map((item) => <li key={item.id}>
            {item.name} — {item.stockQuantity} in stock — {item.isAvailable ? 'available' : 'hidden'}
            <button onClick={() => restock(item)}>Restock</button>
            <button onClick={() => setAvailability(item)}>{item.isAvailable ? 'Hide' : 'Show'}</button>
            <button onClick={() => startEditing(item)}>Edit</button>
          </li>)}
        </ul>}
        <button onClick={() => setPageNumber((page) => page - 1)} disabled={pageNumber === 1}>Previous page</button>
        <span> Page {pageNumber} of {totalPages || 1} </span>
        <button onClick={() => setPageNumber((page) => page + 1)} disabled={pageNumber >= totalPages}>Next page</button>
      </section>

      <section>
        <h3>Staff accounts</h3>
        <ul>{staffUsers.map((user) => <li key={user.id}>{user.fullName} ({user.email}) — {user.isActive ? 'active' : 'inactive'}
          <select value={user.role === 'Admin' ? 1 : 2} onChange={(e) => updateStaffRole(user, e.target.value)}><option value={1}>Admin</option><option value={2}>Kitchen staff</option></select>
          <button onClick={() => setStaffActive(user)}>{user.isActive ? 'Deactivate' : 'Activate'}</button>
          <button onClick={() => resetStaffPassword(user)}>Reset password</button>
          <button onClick={() => deleteStaff(user)}>Delete</button>
        </li>)}</ul>
      </section>

      <section>
        <h3>Create a staff account</h3>
        <form onSubmit={createStaff}>
          <input placeholder="Full name" value={staff.fullName} onChange={(e) => setStaff({ ...staff, fullName: e.target.value })} required />
          <input type="email" placeholder="Email" value={staff.email} onChange={(e) => setStaff({ ...staff, email: e.target.value })} required />
          <input type="password" placeholder="Password" value={staff.password} onChange={(e) => setStaff({ ...staff, password: e.target.value })} required />
          <select value={staff.role} onChange={(e) => setStaff({ ...staff, role: Number(e.target.value) })}>
            <option value={2}>Kitchen staff</option>
            <option value={1}>Admin</option>
          </select>
          <button type="submit">Create account</button>
        </form>
      </section>
    </div>
  );
}
