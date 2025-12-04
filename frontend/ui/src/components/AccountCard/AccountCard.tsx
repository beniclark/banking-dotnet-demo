import type { AccountSummary } from '../../types';
import './AccountCard.css';

interface AccountCardProps {
  account: AccountSummary;
  onClick?: () => void;
}

export function AccountCard({ account, onClick }: AccountCardProps) {
  const getAccountIcon = (type: string) => {
    switch (type.toLowerCase()) {
      case 'checking':
        return (
          <svg viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
            <rect x="3" y="6" width="18" height="12" rx="2" stroke="currentColor" strokeWidth="2"/>
            <path d="M3 10H21" stroke="currentColor" strokeWidth="2"/>
            <path d="M7 14H13" stroke="currentColor" strokeWidth="2"/>
          </svg>
        );
      case 'savings':
        return (
          <svg viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
            <circle cx="12" cy="12" r="9" stroke="currentColor" strokeWidth="2"/>
            <path d="M12 6V12L16 14" stroke="currentColor" strokeWidth="2" strokeLinecap="round"/>
          </svg>
        );
      default:
        return (
          <svg viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
            <rect x="3" y="8" width="18" height="10" rx="2" stroke="currentColor" strokeWidth="2"/>
            <path d="M7 8V6C7 4.89543 7.89543 4 9 4H15C16.1046 4 17 4.89543 17 6V8" stroke="currentColor" strokeWidth="2"/>
          </svg>
        );
    }
  };

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
    }).format(amount);
  };

  return (
    <div className={`account-card ${account.accountType.toLowerCase()}`} onClick={onClick}>
      <div className="account-card-header">
        <div className="account-icon">{getAccountIcon(account.accountType)}</div>
        <div className="account-type-badge">{account.accountType}</div>
      </div>
      <div className="account-card-body">
        <h3 className="account-name">{account.accountName}</h3>
        <p className="account-number">{account.accountNumber}</p>
      </div>
      <div className="account-card-footer">
        <span className="balance-label">Available Balance</span>
        <span className="balance-amount">{formatCurrency(account.balance)}</span>
      </div>
    </div>
  );
}
