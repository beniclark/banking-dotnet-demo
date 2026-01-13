import { useState } from 'react';
import type { FormEvent } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import { bankingApi } from '../../services/bankingApi';
import './Login.css';

export function Login() {
  const navigate = useNavigate();
  const { login } = useAuth();
  const [email, setEmail] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError(null);
    setLoading(true);

    try {
      const response = await bankingApi.login({ email });
      
      if (response.success && response.customer) {
        login(response.customer);
        navigate('/');
      } else {
        setError(response.message || 'Login failed. Please try again.');
      }
    } catch (err: any) {
      console.error('Login error:', err);
      if (err.response?.status === 401) {
        setError('No account found with this email address.');
      } else {
        setError('Unable to connect to the server. Please try again later.');
      }
    } finally {
      setLoading(false);
    }
  };

  const quickLogin = (demoEmail: string) => {
    setEmail(demoEmail);
  };

  return (
    <div className="login-page">
      <div className="login-container">
        <div className="login-header">
          <h1>🏦 Three Rivers Bank</h1>
          <p>Welcome! Please log in to access your accounts.</p>
        </div>

        <form onSubmit={handleSubmit} className="login-form">
          <div className="form-group">
            <label htmlFor="email">Email Address</label>
            <input
              type="email"
              id="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              placeholder="Enter your email"
              required
              disabled={loading}
              autoFocus
            />
          </div>

          {error && <div className="error-message">{error}</div>}

          <button type="submit" disabled={loading} className="login-button">
            {loading ? 'Logging in...' : 'Log In'}
          </button>
        </form>

        <div className="demo-section">
          <p className="demo-label">Demo Accounts - Quick Login:</p>
          <div className="demo-buttons">
            <button
              type="button"
              onClick={() => quickLogin('sarah.johnson@email.com')}
              className="demo-button"
              disabled={loading}
            >
              Sarah Johnson
            </button>
            <button
              type="button"
              onClick={() => quickLogin('michael.chen@email.com')}
              className="demo-button"
              disabled={loading}
            >
              Michael Chen
            </button>
            <button
              type="button"
              onClick={() => quickLogin('emily.rodriguez@email.com')}
              className="demo-button"
              disabled={loading}
            >
              Emily Rodriguez
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
