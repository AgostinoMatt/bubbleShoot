using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mkey
{
    public class PurchaseEvents : MonoBehaviour
    {
        private BubblesPlayer MPlayer { get { return BubblesPlayer.Instance; } }
        private BubblesGuiController MGui { get { return BubblesGuiController.Instance; } }

        public void AddCoins(int count)
        {
            if (MPlayer != null)
            {
                MPlayer.AddCoins(count);
            }
        }

        public void AddLife(int count)
        {
            if (MPlayer != null)
            {
                MPlayer.AddLifes(count);
            }
        }

        public void SetInfiniteLife(int count)
        {
            if (MPlayer != null)
            {
                MPlayer.StartInfiniteLife(count);
            }
        }

        internal void GoodPurchaseMessage(string prodId, string prodName)
        {
            if (MGui)
            {
                MGui.ShowMessage("Succesfull!!!", prodName + " purchased successfull.", 3, null);
            }
        }

        internal void FailedPurchaseMessage(string prodId, string prodName)
        {
            if (MGui)
            {
                MGui.ShowMessage("Sorry.", prodName + " - purchase failed.", 3, null);
            }
        }
    }
}