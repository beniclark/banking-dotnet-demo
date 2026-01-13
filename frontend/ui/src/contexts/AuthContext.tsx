import { createContext, useContext, useState, useEffect } from 'react';
import type { ReactNode } from 'react';
import type { CustomerProfile } from '../types';

interface AuthContextType {
  customer: CustomerProfile | null;
  isAuthenticated: boolean;
  login: (customer: CustomerProfile) => void;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [customer, setCustomer] = useState<CustomerProfile | null>(null);

  // Load customer from localStorage on mount
  useEffect(() => {
    const storedCustomer = localStorage.getItem('customer');
    if (storedCustomer) {
      try {
        setCustomer(JSON.parse(storedCustomer));
      } catch (error) {
        console.error('Failed to parse stored customer:', error);
        localStorage.removeItem('customer');
      }
    }
  }, []);

  const login = (customer: CustomerProfile) => {
    setCustomer(customer);
    localStorage.setItem('customer', JSON.stringify(customer));
  };

  const logout = () => {
    setCustomer(null);
    localStorage.removeItem('customer');
  };

  return (
    <AuthContext.Provider
      value={{
        customer,
        isAuthenticated: !!customer,
        login,
        logout,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
}
