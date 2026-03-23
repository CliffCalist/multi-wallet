using System;
using System.Collections.Generic;

namespace WhiteArrow.MultiWallet
{
    public class WalletService
    {
        private readonly List<Wallet> _wallets = new();
        private readonly Dictionary<Wallet, Action<long>> _walletBalanceHandlers = new();


        public int Count => _wallets.Count;
        public IReadOnlyList<Wallet> Wallets => _wallets;



        public event Action<Wallet> WalletAdded;
        public event Action<Wallet> WalletRemoved;
        public event Action<Wallet> BalanceChanged;



        public void RestoreStateFrom(IWalletServiceSnapshot snapshot)
        {
            if (snapshot == null)
                throw new ArgumentNullException(nameof(snapshot));

            RemoveAllWallets();

            foreach (var walletSnapshot in snapshot.Wallets)
            {
                if (TryGetWallet(walletSnapshot.Currency, out var wallet))
                    wallet.RestoreStateFrom(walletSnapshot);
                else
                {
                    var newWallet = new Wallet(walletSnapshot.Currency);
                    SubscribeToWalletBalanceChanged(newWallet);
                    newWallet.RestoreStateFrom(walletSnapshot);
                    _wallets.Add(newWallet);
                    WalletAdded?.Invoke(newWallet);
                }
            }
        }

        public void CaptureStateTo(IWalletServiceSnapshot snapshot)
        {
            if (snapshot == null)
                throw new ArgumentNullException(nameof(snapshot));

            foreach (var wallet in _wallets)
            {
                var walletSnapshot = snapshot.CreateWalletSnapshot();
                wallet.CaptureStateTo(walletSnapshot);
                snapshot.AddWalletSnapshot(walletSnapshot);
            }
        }



        public bool HasWallet(string currency)
        {
            return _wallets.Exists(w => w.Currency == currency);
        }

        public void AddWallets(IEnumerable<Wallet> wallets)
        {
            if (wallets is null)
                throw new ArgumentNullException(nameof(wallets));

            foreach (var wallet in wallets)
            {
                if (wallet != null && !HasWallet(wallet.Currency))
                {
                    _wallets.Add(wallet);
                    SubscribeToWalletBalanceChanged(wallet);
                    WalletAdded?.Invoke(wallet);
                }
            }
        }

        public void AddWallet(Wallet wallet)
        {
            if (wallet is null)
                throw new ArgumentNullException(nameof(wallet));

            if (HasWallet(wallet.Currency))
                return;

            _wallets.Add(wallet);
            SubscribeToWalletBalanceChanged(wallet);
            WalletAdded?.Invoke(wallet);
        }

        public void RemoveWallet(string currencyId)
        {
            if (string.IsNullOrEmpty(currencyId))
                throw new ArgumentNullException(nameof(currencyId));

            if (TryGetWallet(currencyId, out var wallet))
            {
                _wallets.Remove(wallet);
                UnsubscribeFromWalletBalanceChanged(wallet);
                WalletRemoved?.Invoke(wallet);
            }
        }

        public void RemoveAllWallets()
        {
            var wallets = new List<Wallet>(_wallets);
            _wallets.Clear();

            foreach (var wallet in wallets)
            {
                UnsubscribeFromWalletBalanceChanged(wallet);
                WalletRemoved?.Invoke(wallet);
            }
        }



        public long GetBalance(string currency)
        {
            return GetWallet(currency).Balance;
        }

        public bool TrySpend(string currency, long value)
        {
            return GetWallet(currency).TrySpend(value);
        }

        public void Add(string currency, long value)
        {
            GetWallet(currency).Add(value);
        }

        public void Spend(string currency, long value)
        {
            GetWallet(currency).Spend(value);
        }

        public void Reset(string currency)
        {
            GetWallet(currency).Reset();
        }



        public bool TryGetWallet(string currency, out Wallet wallet)
        {
            wallet = _wallets.Find(w => w.Currency == currency);
            return wallet != null;
        }

        public Wallet GetWallet(string currency)
        {
            if (TryGetWallet(currency, out var wallet))
                return wallet;
            else throw new Exception($"{currency} not found.");
        }



        private void SubscribeToWalletBalanceChanged(Wallet wallet)
        {
            if (wallet == null || _walletBalanceHandlers.ContainsKey(wallet))
                return;

            Action<long> handler = b => BalanceChanged?.Invoke(wallet);
            _walletBalanceHandlers[wallet] = handler;
            wallet.BalanceChanged += handler;
        }

        private void UnsubscribeFromWalletBalanceChanged(Wallet wallet)
        {
            if (wallet == null)
                return;

            if (_walletBalanceHandlers.TryGetValue(wallet, out var handler))
            {
                wallet.BalanceChanged -= handler;
                _walletBalanceHandlers.Remove(wallet);
            }
        }
    }
}
