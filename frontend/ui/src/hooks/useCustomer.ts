import { useState, useEffect } from 'react';
import { bankingApi } from '../services/bankingApi';
import type { CustomerProfile, Account, Transaction } from '../types';

interface UseCustomerReturn {
  profile: CustomerProfile | null;
  accounts: Account[];
  recentTransactions: Transaction[];
  loading: boolean;
  error: string | null;
  refreshData: () => Promise<void>;
}

export function useCustomer(customerId: string | null): UseCustomerReturn {
  const [profile, setProfile] = useState<CustomerProfile | null>(null);
  const [accounts, setAccounts] = useState<Account[]>([]);
  const [recentTransactions, setRecentTransactions] = useState<Transaction[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchData = async () => {
    if (!customerId) {
      setLoading(false);
      return;
    }

    try {
      setLoading(true);
      setError(null);
      
      const [customerProfile, customerAccounts] = await Promise.all([
        bankingApi.getCustomerProfile(customerId),
        bankingApi.getCustomerAccounts(customerId),
      ]);
      
      setProfile(customerProfile);
      setAccounts(customerAccounts);

      // Get recent transactions from first account
      if (customerAccounts.length > 0) {
        const transactions = await bankingApi.getAccountTransactions(
          customerAccounts[0].id,
          10
        );
        setRecentTransactions(transactions);
      }
    } catch (err) {
      setError('Failed to load customer data. Please try again later.');
      console.error('Error fetching customer data:', err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, [customerId]);

  return {
    profile,
    accounts,
    recentTransactions,
    loading,
    error,
    refreshData: fetchData,
  };
}