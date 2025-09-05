using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WhiteArrow.MultiWallet
{
    [CreateAssetMenu(fileName = "WalletServiceConfig", menuName = "White Arrow/Wallet Storage")]
    public class WalletServiceConfig : ScriptableObject
    {
#if UNITY_EDITOR
        [SerializeField] private bool _useEditorInitBalance = true;
        [SerializeField, Min(0)] private long _editorInitBalance = 100000;
#endif

        [SerializeField] private List<WalletConfig> _wallets;


        public int Count => _wallets.Count;
        public IEnumerable<string> AllCurrencies => _wallets.Select(w => w.Currency);



        public string GetCurrencyByIndex(int index)
        {
            ThrowIfInvalidIndex(index);
            return _wallets[index].Currency;
        }



        public long GetInitBalance(string currency)
        {
            var index = _wallets.FindIndex(i => i.Currency == currency);
            return GetInitBalance(index);
        }

        public long GetInitBalance(int index)
        {
            ThrowIfInvalidIndex(index);

#if UNITY_EDITOR
            if (_useEditorInitBalance)
                return _editorInitBalance;
#endif

            return _wallets[index].InitBalance;
        }



        private void ThrowIfInvalidIndex(int index)
        {
            if (index < 0 || index >= _wallets.Count)
                throw new ArgumentOutOfRangeException(nameof(index));
        }
    }
}