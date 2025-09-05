using System;
using UnityEngine;

namespace WhiteArrow.MultiWallet
{
    [Serializable]
    public class WalletConfig
    {
        [SerializeField] private string _currency;
        [SerializeField, Min(0)] private long _initBalance;



        public string Currency => _currency;
        public long InitBalance => _initBalance;
    }
}