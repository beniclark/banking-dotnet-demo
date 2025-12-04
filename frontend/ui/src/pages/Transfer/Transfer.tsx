import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Header } from '../../components';
import { bankingApi } from '../../services/bankingApi';
import type { Account, TransferResult } from '../../types';
import './Transfer.css';

// Demo customer ID - Sarah Johnson
const DEMO_CUSTOMER_ID = '11111111-1111-1111-1111-111111111111';

export function Transfer() {
  const navigate = useNavigate();
  const [accounts, setAccounts] = useState<Account[]>([]);
  const [fromAccountId, setFromAccountId] = useState('');
  const [toAccountId, setToAccountId] = useState('');
  const [amount, setAmount] = useState('');
  const [description, setDescription] = useState('');
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [result, setResult] = useState<TransferResult | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchAccounts = async () => {
      try {
        const customerAccounts = await bankingApi.getCustomerAccounts(DEMO_CUSTOMER_ID);
        setAccounts(customerAccounts);
        if (customerAccounts.length > 0) {
          setFromAccountId(customerAccounts[0].id);
          if (customerAccounts.length > 1) {
            setToAccountId(customerAccounts[1].id);
          }
        }
      } catch (err) {
        setError('Failed to load accounts');
        console.error('Error fetching accounts:', err);
      } finally {
        setLoading(false);
      }
    };

    fetchAccounts();
  }, []);

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
    }).format(amount);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!fromAccountId || !toAccountId || !amount) {
      setError('Please fill in all required fields');
      return;
    }

    if (fromAccountId === toAccountId) {
      setError('Please select different accounts for transfer');
      return;
    }

    const amountNum = parseFloat(amount);
    if (isNaN(amountNum) || amountNum <= 0) {
      setError('Please enter a valid amount');
      return;
    }

    try {
      setSubmitting(true);
      setError(null);
      const transferResult = await bankingApi.transfer({
        fromAccountId,
        toAccountId,
        amount: amountNum,
        description,
      });
      setResult(transferResult);
    } catch (err) {
      setError('Transfer failed. Please try again.');
      console.error('Transfer error:', err);
    } finally {
      setSubmitting(false);
    }
  };

  if (loading) {
    return (
      <div className="transfer-page">
        <Header />
        <main className="transfer-content">
          <div className="loading-state">
            <div className="loading-spinner"></div>
            <p>Loading accounts...</p>
          </div>
        </main>
      </div>
    );
  }

  if (result?.success) {
    return (
      <div className="transfer-page">
        <Header />
        <main className="transfer-content">
          <div className="transfer-container">
            <div className="success-card">
              <div className="success-icon">✓</div>
              <h2>Transfer Successful!</h2>
              <p className="success-amount">{formatCurrency(parseFloat(amount))}</p>
              <p className="success-message">{result.message}</p>
              <p className="reference-number">Reference: {result.referenceNumber}</p>
              <div className="success-actions">
                <button className="btn-primary" onClick={() => navigate('/')}>
                  Back to Dashboard
                </button>
                <button className="btn-secondary" onClick={() => {
                  setResult(null);
                  setAmount('');
                  setDescription('');
                }}>
                  Make Another Transfer
                </button>
              </div>
            </div>
          </div>
        </main>
      </div>
    );
  }

  return (
    <div className="transfer-page">
      <Header />
      <main className="transfer-content">
        <div className="transfer-container">
          <div className="page-header">
            <h1>Transfer Money</h1>
            <p>Move money between your accounts instantly</p>
          </div>

          <form className="transfer-form" onSubmit={handleSubmit}>
            {error && <div className="error-message">{error}</div>}
            
            <div className="form-group">
              <label htmlFor="fromAccount">From Account</label>
              <select
                id="fromAccount"
                value={fromAccountId}
                onChange={(e) => setFromAccountId(e.target.value)}
                required
              >
                <option value="">Select account</option>
                {accounts.map((account) => (
                  <option key={account.id} value={account.id}>
                    {account.accountName} - ****{account.accountNumber.slice(-4)} ({formatCurrency(account.balance)})
                  </option>
                ))}
              </select>
            </div>

            <div className="transfer-arrow">↓</div>

            <div className="form-group">
              <label htmlFor="toAccount">To Account</label>
              <select
                id="toAccount"
                value={toAccountId}
                onChange={(e) => setToAccountId(e.target.value)}
                required
              >
                <option value="">Select account</option>
                {accounts
                  .filter((a) => a.id !== fromAccountId)
                  .map((account) => (
                    <option key={account.id} value={account.id}>
                      {account.accountName} - ****{account.accountNumber.slice(-4)} ({formatCurrency(account.balance)})
                    </option>
                  ))}
              </select>
            </div>

            <div className="form-group">
              <label htmlFor="amount">Amount</label>
              <div className="amount-input-wrapper">
                <span className="currency-symbol">$</span>
                <input
                  type="number"
                  id="amount"
                  value={amount}
                  onChange={(e) => setAmount(e.target.value)}
                  placeholder="0.00"
                  min="0.01"
                  step="0.01"
                  required
                />
              </div>
            </div>

            <div className="form-group">
              <label htmlFor="description">Description (Optional)</label>
              <input
                type="text"
                id="description"
                value={description}
                onChange={(e) => setDescription(e.target.value)}
                placeholder="What's this transfer for?"
                maxLength={100}
              />
            </div>

            <button
              type="submit"
              className="submit-btn"
              disabled={submitting}
            >
              {submitting ? 'Processing...' : 'Transfer Money'}
            </button>
          </form>
        </div>
      </main>
    </div>
  );
}
