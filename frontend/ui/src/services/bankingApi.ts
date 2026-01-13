import axios from 'axios';
import type {
  Customer,
  CustomerProfile,
  Account,
  Transaction,
  TransferRequest,
  TransferResult,
  DepositRequest,
  WithdrawalRequest,
  LoginRequest,
  LoginResponse,
} from '../types';

const API_BASE_URL = 'http://localhost:5000/api';

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

export const bankingApi = {
  // Authentication endpoints
  login: async (request: LoginRequest): Promise<LoginResponse> => {
    const response = await apiClient.post<LoginResponse>('/auth/login', request);
    return response.data;
  },

  // Customer endpoints
  getAllCustomers: async (): Promise<Customer[]> => {
    const response = await apiClient.get<Customer[]>('/customers');
    return response.data;
  },

  getCustomerById: async (customerId: string): Promise<Customer> => {
    const response = await apiClient.get<Customer>(`/customers/${customerId}`);
    return response.data;
  },

  getCustomerProfile: async (customerId: string): Promise<CustomerProfile> => {
    const response = await apiClient.get<CustomerProfile>(`/customers/${customerId}/profile`);
    return response.data;
  },

  // Account endpoints
  getCustomerAccounts: async (customerId: string): Promise<Account[]> => {
    const response = await apiClient.get<Account[]>(`/customers/${customerId}/accounts`);
    return response.data;
  },

  getAccountById: async (accountId: string): Promise<Account> => {
    const response = await apiClient.get<Account>(`/accounts/${accountId}`);
    return response.data;
  },

  // Transaction endpoints
  getAccountTransactions: async (accountId: string, count: number = 50): Promise<Transaction[]> => {
    const response = await apiClient.get<Transaction[]>(`/accounts/${accountId}/transactions`, {
      params: { count },
    });
    return response.data;
  },

  getTransactionById: async (transactionId: string): Promise<Transaction> => {
    const response = await apiClient.get<Transaction>(`/transactions/${transactionId}`);
    return response.data;
  },

  // Banking operations
  transfer: async (request: TransferRequest): Promise<TransferResult> => {
    const response = await apiClient.post<TransferResult>('/transfer', request);
    return response.data;
  },

  deposit: async (request: DepositRequest): Promise<Transaction> => {
    const response = await apiClient.post<Transaction>('/deposit', request);
    return response.data;
  },

  withdraw: async (request: WithdrawalRequest): Promise<Transaction> => {
    const response = await apiClient.post<Transaction>('/withdraw', request);
    return response.data;
  },
};

export default bankingApi;
