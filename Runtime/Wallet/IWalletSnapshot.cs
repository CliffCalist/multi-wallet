namespace WhiteArrow.MultiWallet
{
    public interface IWalletSnapshot
    {
        string Currency { get; set; }
        public long Balance { get; set; }
    }
}