import type { Transaction } from '../../types';
import './TransactionList.css';

interface TransactionListProps {
  transactions: Transaction[];
  showAccountInfo?: boolean;
}

export function TransactionList({ transactions, showAccountInfo = false }: TransactionListProps) {
  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
    }).format(amount);
  };

  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
      month: 'short',
      day: 'numeric',
      year: 'numeric',
    });
  };

  const getCategoryIcon = (category: string) => {
    switch (category.toLowerCase()) {
      case 'deposit':
        return '💰';
      case 'withdrawal':
        return '🏧';
      case 'transfer':
        return '↔️';
      case 'purchase':
        return '🛒';
      case 'payment':
        return '📄';
      default:
        return '💳';
    }
  };

  if (transactions.length === 0) {
    return (
      <div className="transaction-list-empty">
        <p>No transactions to display</p>
      </div>
    );
  }

  return (
    <div className="transaction-list">
      {transactions.map((transaction) => (
        <div key={transaction.id} className="transaction-item">
          <div className="transaction-icon">{getCategoryIcon(transaction.category)}</div>
          <div className="transaction-details">
            <div className="transaction-main">
              <span className="transaction-description">
                {transaction.merchant || transaction.description}
              </span>
              <span className={`transaction-amount ${transaction.transactionType.toLowerCase()}`}>
                {transaction.transactionType === 'Credit' ? '+' : '-'}
                {formatCurrency(transaction.amount)}
              </span>
            </div>
            <div className="transaction-meta">
              <span className="transaction-category">{transaction.category}</span>
              <span className="transaction-date">{formatDate(transaction.transactionDate)}</span>
            </div>
            {showAccountInfo && (
              <div className="transaction-reference">
                Ref: {transaction.referenceNumber}
              </div>
            )}
          </div>
        </div>
      ))}
    </div>
  );
}
