using UnityEngine.UI;
using UnityEngine;

namespace Mkey
{
    public class AboutWindowController : PopUpsController
    {
        public string SUPPORT_URL;

        private GameBoard MBoard { get { return GameBoard.Instance; } }
        private BubblesPlayer MPlayer { get { return BubblesPlayer.Instance; } }
        private BubblesGuiController MGui { get { return BubblesGuiController.Instance; } }
        private SoundMasterController MSound { get { return SoundMasterController.Instance; } }
        private FBholder FB { get { return FBholder.Instance; } }


        public void SupportButton_Click()
        {
            Debug.Log("Support");
            if (!string.IsNullOrEmpty(SUPPORT_URL)) Application.OpenURL(SUPPORT_URL);
        }
    }
}