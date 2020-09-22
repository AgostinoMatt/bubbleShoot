using UnityEngine;
using UnityEngine.UI;

namespace Mkey
{
    public class LossWindowController : PopUpsController
    {
        public Text missionDescriptionText;

        private GameBoard MBoard { get { return GameBoard.Instance; } }
        private BubblesPlayer MPlayer { get { return BubblesPlayer.Instance; } }
        private BubblesGuiController MGui { get { return BubblesGuiController.Instance; } }
        private SoundMasterController MSound { get { return SoundMasterController.Instance; } }

        public override void RefreshWindow()
        {
            string description = (MPlayer.LcSet) ? MPlayer.LcSet.levelMission.Description : "";
            missionDescriptionText.text = description;
            missionDescriptionText.enabled = !string.IsNullOrEmpty(description);
            base.RefreshWindow();
        }

        public void Cancel_Click()
        {
            CloseWindow();
            SceneLoader.Instance.LoadScene(0);
        }

        public void Retry_Click()
        {
            CloseWindow();
            SceneLoader.Instance.LoadScene(0);
        }

    }
}