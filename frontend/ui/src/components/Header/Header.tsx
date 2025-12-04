import { Link } from 'react-router-dom';
import './Header.css';

interface HeaderProps {
  customerName?: string;
}

export function Header({ customerName }: HeaderProps) {
  return (
    <header className="header">
      <div className="header-content">
        <Link to="/" className="logo">
          <div className="logo-icon">
            <svg viewBox="0 0 40 40" fill="none" xmlns="http://www.w3.org/2000/svg">
              <path d="M20 4L4 12V16H36V12L20 4Z" fill="currentColor"/>
              <rect x="6" y="18" width="4" height="14" fill="currentColor"/>
              <rect x="14" y="18" width="4" height="14" fill="currentColor"/>
              <rect x="22" y="18" width="4" height="14" fill="currentColor"/>
              <rect x="30" y="18" width="4" height="14" fill="currentColor"/>
              <rect x="4" y="34" width="32" height="4" fill="currentColor"/>
            </svg>
          </div>
          <span className="logo-text">Three Rivers Bank</span>
        </Link>
        <nav className="nav">
          <Link to="/" className="nav-link">Dashboard</Link>
          <Link to="/transfer" className="nav-link">Transfer</Link>
          <Link to="/transactions" className="nav-link">Transactions</Link>
        </nav>
        {customerName && (
          <div className="user-info">
            <span className="user-greeting">Welcome, {customerName}</span>
          </div>
        )}
      </div>
    </header>
  );
}
