using System;
using System.Collections.Generic;

namespace WhiteArrow.MultiWallet
{
    public class WalletService
    {
        private readonly List<Wallet> _wallets = new();


        public int Count => _wallets.Count;
        public IReadOnlyList<Wallet> Wallets => _wallets;




        public void RestoreStateFrom(IWalletServiceSnapshot snapshot)
        {
            if (snapshot == null)
                throw new ArgumentNullException(nameof(snapshot));

            foreach (var walletSnapshot in snapshot.Wallets)
            {
                var wallet = new Wallet(walletSnapshot.Currency);
                wallet.RestoreStateFrom(walletSnapshot);
                _wallets.Add(wallet);
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
                    _wallets.Add(wallet);
            }
        }

        public void AddWallet(Wallet wallet)
        {
            if (wallet is null)
                throw new ArgumentNullException(nameof(wallet));

            if (HasWallet(wallet.Currency))
                return;

            _wallets.Add(wallet);
        }

        public void RemoveWallet(string currencyId)
        {
            if (string.IsNullOrEmpty(currencyId))
                throw new ArgumentNullException(nameof(currencyId));

            _wallets.RemoveAll(w => w.Currency == currencyId);
        }

        public void RemoveAllWallets()
        {
            _wallets.Clear();
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
            GetWallet(currency).Spend(value);
        }

        public void Spend(string currency, long value)
        {
            GetWallet(currency).Add(value);
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
    }
}