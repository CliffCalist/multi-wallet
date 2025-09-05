using System.Collections.Generic;

namespace WhiteArrow.MultiWallet
{
    public interface IWalletServiceSnapshot
    {
        IEnumerable<IWalletSnapshot> Wallets { get; }



        IWalletSnapshot CreateWalletSnapshot();
        void AddWalletSnapshot(IWalletSnapshot snapshot);
    }
}