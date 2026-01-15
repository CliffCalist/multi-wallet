using System;

namespace WhiteArrow.MultiWallet
{
    public class Wallet
    {
        private string _currency;
        private long _balance;



        public string Currency => _currency;
        public long Balance => _balance;



        public event Action<long> BalanceChanged;



        public Wallet(string currency)
        {
            if (string.IsNullOrEmpty(currency))
                throw new ArgumentNullException(nameof(currency));

            _currency = currency;
        }



        public void RestoreStateFrom(IWalletSnapshot snapshot)
        {
            if (snapshot is null)
                throw new ArgumentNullException(nameof(snapshot));

            _currency = snapshot.Currency;
            _balance = snapshot.Balance;

            BalanceChanged?.Invoke(Balance);
        }

        public void CaptureStateTo(IWalletSnapshot snapshot)
        {
            if (snapshot == null)
                throw new ArgumentNullException(nameof(snapshot));

            snapshot.Currency = _currency;
            snapshot.Balance = _balance;
        }



        public bool TrySpend(long amount)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount));

            if (amount > Balance)
                return false;

            Spend(amount);
            return true;
        }

        public void Spend(long amount)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount));

            if (amount > Balance)
                throw new Exception("Not enough balance.");

            _balance -= amount;
            BalanceChanged?.Invoke(Balance);
        }



        public void Add(long amount)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount));

            _balance += amount;
            BalanceChanged?.Invoke(Balance);
        }



        public void Reset()
        {
            _balance = 0;
            BalanceChanged?.Invoke(Balance);
        }
    }
}