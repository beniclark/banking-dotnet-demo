import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import { Header, AccountCard, TransactionList } from '../../components';
import { bankingApi } from '../../services/bankingApi';
import type { CustomerProfile, Transaction } from '../../types';
import './Dashboard.css';

export function Dashboard() {
  const navigate = useNavigate();
  const { customer } = useAuth();
  const [profile, setProfile] = useState<CustomerProfile | null>(customer);
  const [recentTransactions, setRecentTransactions] = useState<Transaction[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchData = async () => {
      if (!customer) {
        setLoading(false);
        return;
      }

      try {
        setLoading(true);
        const customerProfile = await bankingApi.getCustomerProfile(customer.id);
        setProfile(customerProfile);

        // Get transactions from the first account
        if (customerProfile.accounts.length > 0) {
          const transactions = await bankingApi.getAccountTransactions(
            customerProfile.accounts[0].id,
            10
          );
          setRecentTransactions(transactions);
        }
      } catch (err) {
        setError('Failed to load account information. Please try again later.');
        console.error('Error fetching data:', err);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [customer]);

  const calculateTotalBalance = () => {
    if (!profile) return 0;
    return profile.accounts.reduce((sum, account) => sum + account.balance, 0);
  };

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
    }).format(amount);
  };

  if (loading) {
    return (
      <div className="dashboard-page">
        <Header />
        <main className="dashboard-content">
          <div className="loading-state">
            <div className="loading-spinner"></div>
            <p>Loading your accounts...</p>
          </div>
        </main>
      </div>
    );
  }

  if (error) {
    return (
      <div className="dashboard-page">
        <Header />
        <main className="dashboard-content">
          <div className="error-state">
            <p>{error}</p>
            <button onClick={() => window.location.reload()}>Retry</button>
          </div>
        </main>
      </div>
    );
  }

  return (
    <div className="dashboard-page">
      <Header customerName={profile?.fullName.split(' ')[0]} />
      <main className="dashboard-content">
        <div className="dashboard-container">
          {/* Welcome Section */}
          <section className="welcome-section">
            <div className="welcome-text">
              <h1>Good {getGreeting()}, {profile?.fullName.split(' ')[0]}!</h1>
              <p>Here's your financial overview</p>
            </div>
            <div className="total-balance-card">
              <span className="total-label">Total Balance</span>
              <span className="total-amount">{formatCurrency(calculateTotalBalance())}</span>
            </div>
          </section>

          {/* Quick Actions */}
          <section className="quick-actions">
            <button className="quick-action-btn" onClick={() => navigate('/transfer')}>
              <span className="action-icon">↔️</span>
              <span>Transfer</span>
            </button>
            <button className="quick-action-btn" onClick={() => navigate('/deposit')}>
              <span className="action-icon">💰</span>
              <span>Deposit</span>
            </button>
            <button className="quick-action-btn" onClick={() => navigate('/transactions')}>
              <span className="action-icon">📊</span>
              <span>History</span>
            </button>
          </section>

          {/* Accounts Section */}
          <section className="accounts-section">
            <div className="section-header">
              <h2>Your Accounts</h2>
            </div>
            <div className="accounts-grid">
              {profile?.accounts.map((account) => (
                <AccountCard
                  key={account.id}
                  account={account}
                  onClick={() => navigate(`/accounts/${account.id}`)}
                />
              ))}
            </div>
          </section>

          {/* Recent Transactions */}
          <section className="transactions-section">
            <div className="section-header">
              <h2>Recent Activity</h2>
              <button className="view-all-btn" onClick={() => navigate('/transactions')}>
                View All
              </button>
            </div>
            <div className="transactions-card">
              <TransactionList transactions={recentTransactions} />
            </div>
          </section>
        </div>
      </main>
    </div>
  );
}

function getGreeting() {
  const hour = new Date().getHours();
  if (hour < 12) return 'morning';
  if (hour < 17) return 'afternoon';
  return 'evening';
}
