// Types for Three Rivers Bank API

export interface Customer {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  address: string;
  city: string;
  state: string;
  zipCode: string;
  dateOfBirth: string;
  createdDate: string;
  isActive: boolean;
  customerNumber: string;
}

export interface AccountSummary {
  id: string;
  accountNumber: string;
  accountType: string;
  accountName: string;
  balance: number;
}

export interface CustomerProfile {
  id: string;
  fullName: string;
  email: string;
  customerNumber: string;
  memberSince: string;
  accounts: AccountSummary[];
}

export interface Account {
  id: string;
  accountNumber: string;
  accountType: string;
  accountName: string;
  balance: number;
  availableBalance: number;
  customerId: string;
  openedDate: string;
  isActive: boolean;
  currency: string;
}

export interface Transaction {
  id: string;
  accountId: string;
  transactionType: string;
  category: string;
  amount: number;
  balanceAfter: number;
  description: string;
  merchant: string;
  transactionDate: string;
  status: string;
  referenceNumber: string;
}

export interface TransferRequest {
  fromAccountId: string;
  toAccountId: string;
  amount: number;
  description: string;
}

export interface TransferResult {
  success: boolean;
  message: string;
  referenceNumber: string;
  fromTransaction?: Transaction;
  toTransaction?: Transaction;
}

export interface DepositRequest {
  accountId: string;
  amount: number;
  description: string;
}

export interface WithdrawalRequest {
  accountId: string;
  amount: number;
  description: string;
}
