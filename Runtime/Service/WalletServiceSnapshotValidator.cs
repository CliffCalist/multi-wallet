using System;
using System.Linq;

namespace WhiteArrow.MultiWallet
{
    public static class WalletServiceSnapshotValidator
    {
        public static void Validate(IWalletServiceSnapshot snapshot, WalletServiceConfig config)
        {
            if (snapshot is null)
                throw new ArgumentNullException(nameof(snapshot));

            for (int i = 0; i < config.Count; i++)
            {
                var currency = config.GetCurrencyByIndex(i);
                if (snapshot.Wallets.Any(w => w.Currency == currency))
                    continue;

                var walletSnapshot = snapshot.CreateWalletSnapshot();

                walletSnapshot.Currency = currency;
                walletSnapshot.Balance = config.GetInitBalance(i);

                snapshot.AddWalletSnapshot(walletSnapshot);
            }
        }
    }
}