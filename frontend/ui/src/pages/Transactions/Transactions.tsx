import { useState, useEffect } from 'react';
import { Header, TransactionList } from '../../components';
import { bankingApi } from '../../services/bankingApi';
import type { Account, Transaction } from '../../types';
import './Transactions.css';

// Demo customer ID - Sarah Johnson
const DEMO_CUSTOMER_ID = '11111111-1111-1111-1111-111111111111';

export function Transactions() {
  const [accounts, setAccounts] = useState<Account[]>([]);
  const [selectedAccountId, setSelectedAccountId] = useState<string>('');
  const [transactions, setTransactions] = useState<Transaction[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchAccounts = async () => {
      try {
        const customerAccounts = await bankingApi.getCustomerAccounts(DEMO_CUSTOMER_ID);
        setAccounts(customerAccounts);
        if (customerAccounts.length > 0) {
          setSelectedAccountId(customerAccounts[0].id);
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

  useEffect(() => {
    if (!selectedAccountId) return;

    const fetchTransactions = async () => {
      try {
        setLoading(true);
        const accountTransactions = await bankingApi.getAccountTransactions(selectedAccountId);
        setTransactions(accountTransactions);
      } catch (err) {
        setError('Failed to load transactions');
        console.error('Error fetching transactions:', err);
      } finally {
        setLoading(false);
      }
    };

    fetchTransactions();
  }, [selectedAccountId]);

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
    }).format(amount);
  };

  const selectedAccount = accounts.find(a => a.id === selectedAccountId);

  return (
    <div className="transactions-page">
      <Header />
      <main className="transactions-content">
        <div className="transactions-container">
          <div className="page-header">
            <h1>Transaction History</h1>
            <p>View all your account transactions</p>
          </div>

          {/* Account Selector */}
          <div className="account-selector">
            <label htmlFor="account-select">Select Account</label>
            <select
              id="account-select"
              value={selectedAccountId}
              onChange={(e) => setSelectedAccountId(e.target.value)}
            >
              {accounts.map((account) => (
                <option key={account.id} value={account.id}>
                  {account.accountName} - ****{account.accountNumber.slice(-4)}
                </option>
              ))}
            </select>
          </div>

          {/* Account Summary */}
          {selectedAccount && (
            <div className="account-summary-card">
              <div className="summary-item">
                <span className="summary-label">Account</span>
                <span className="summary-value">{selectedAccount.accountName}</span>
              </div>
              <div className="summary-item">
                <span className="summary-label">Type</span>
                <span className="summary-value">{selectedAccount.accountType}</span>
              </div>
              <div className="summary-item">
                <span className="summary-label">Current Balance</span>
                <span className="summary-value highlight">{formatCurrency(selectedAccount.balance)}</span>
              </div>
            </div>
          )}

          {/* Transactions List */}
          <div className="transactions-card">
            {loading ? (
              <div className="loading-state">
                <div className="loading-spinner"></div>
                <p>Loading transactions...</p>
              </div>
            ) : error ? (
              <div className="error-state">
                <p>{error}</p>
              </div>
            ) : (
              <TransactionList transactions={transactions} showAccountInfo />
            )}
          </div>
        </div>
      </main>
    </div>
  );
}
