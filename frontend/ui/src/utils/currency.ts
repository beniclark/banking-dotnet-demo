// Memoized currency formatter
const currencyFormatter = new Intl.NumberFormat('en-US', {
  style: 'currency',
  currency: 'USD',
});

const percentageFormatter = new Intl.NumberFormat('en-US', {
  style: 'percent',
  minimumFractionDigits: 2,
  maximumFractionDigits: 2,
});

export const formatCurrency = (amount: number): string => {
  return currencyFormatter.format(amount);
};

export const formatPercentage = (amount: number): string => {
  return percentageFormatter.format(amount);
};

export const parseAmount = (value: string): number => {
  const cleaned = value.replace(/[^0-9.-]/g, '');
  return parseFloat(cleaned) || 0;
};

export const validateAmount = (amount: string): boolean => {
  const num = parseAmount(amount);
  return !isNaN(num) && num > 0;
};