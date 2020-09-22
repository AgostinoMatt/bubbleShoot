using UnityEngine.UI;
using UnityEngine;

namespace Mkey
{
    public class SettingsMenuController : PopUpsController
    {
        public Button musicButton;
        public Sprite musicButtonOnSprite;
        public Sprite musicButtonOffSprite;
        public Image musicIcon;
        public Sprite musicIconOnSprite;
        public Sprite musicIconOffSprite;


        public Button soundButton;
        public Sprite soundButtonOnSprite;
        public Sprite soundButtonOffSprite;
        public Image  soundIcon;
        public Sprite soundIconOnSprite;
        public Sprite soundIconOffSprite;
        public Text facebookButtonText;
        public Text getCoinsText;

        [SerializeField]
        private string ANDROID_RATE_URL;
        [SerializeField]
        private string IOS_RATE_URL;


        private GameBoard MBoard { get { return GameBoard.Instance; } }
        private BubblesPlayer MPlayer { get { return BubblesPlayer.Instance; } }
        private BubblesGuiController MGui { get { return BubblesGuiController.Instance; } }
        private SoundMasterController MSound { get { return SoundMasterController.Instance; } }
        private FBholder FB { get { return FBholder.Instance; } }

        #region regular
        private void Start()
        {
            FBholder.LoginEvent += FacebookLoginHandler;
            FBholder.LogoutEvent += FacebookLogoutHandler;
        }

        private void OnDestroy()
        {
            FBholder.LoginEvent -= FacebookLoginHandler;
            FBholder.LogoutEvent -= FacebookLogoutHandler;
        }
        #endregion regular

        public void MusikButton_Click()
        {
            MSound.SetMusic (!MSound.MusicOn);
            SetMusicButtSprite(MSound.MusicOn);
        }

        public void SoundButton_Click()
        {
            if (MSound)
            {
                MSound.SetSound(!MSound.SoundOn);
                SetSoundButtSprite(MSound.SoundOn);
            }
        }

        public void FacebookButton_Click()
        {
            if (FB)
            {
                if (!FBholder.IsLogined) FB.FBlogin();
                else FB.FBlogOut(() => { SetFBButtText(); });
            }
        }

        public void AboutButton_Click()
        {
           if (MGui)  MGui.ShowAbout();
        }

        public void RateButton_Click()
        {
#if UNITY_ANDROID
            if (!string.IsNullOrEmpty(ANDROID_RATE_URL)) Application.OpenURL(ANDROID_RATE_URL);
#elif UNITY_IOS
            if (!string.IsNullOrEmpty(IOS_RATE_URL)) Application.OpenURL(IOS_RATE_URL);
#else
            if (!string.IsNullOrEmpty(ANDROID_RATE_URL)) Application.OpenURL(ANDROID_RATE_URL);
#endif
       
        }

        private void SetMusicButtSprite(bool musicOn)
        {
            musicButton.image.sprite = (musicOn)? musicButtonOnSprite : musicButtonOffSprite;
            musicIcon.sprite = (musicOn) ? musicIconOnSprite : musicIconOffSprite;
        }

        private void SetSoundButtSprite(bool soundOn)
        {
            soundButton.image.sprite = (soundOn) ? soundButtonOnSprite : soundButtonOffSprite;
            soundIcon.sprite = (soundOn) ? soundIconOnSprite : soundIconOffSprite;
        }

        private void SetFBButtText()
        {
            if(facebookButtonText)
                facebookButtonText.text = (FBholder.IsLogined) ? "Logout" : "Login";
        }

        public override void RefreshWindow()
        {
           // SetSoundButtVolume(SoundMasterController.Instance.Volume); // not used
            SetMusicButtSprite(SoundMasterController.Instance.MusicOn);
            SetSoundButtSprite(SoundMasterController.Instance.SoundOn);
            SetFBButtText();
            if (getCoinsText) getCoinsText.text = "get +" + MPlayer.defFBCoinsGift;
            base.RefreshWindow();
        }

        #region set volume (not used)
        private Image[] volume;
        public void SoundPlusButton_Click()
        {
            SoundMasterController.Instance.SetVolume(SoundMasterController.Instance.Volume + 0.1f) ;
            SetSoundButtVolume(SoundMasterController.Instance.Volume);
        }

        public void SoundMinusButton_Click()
        {
            MSound.SetVolume(MSound.Volume - 0.1f);
            SetSoundButtVolume(SoundMasterController.Instance.Volume);
        }

        private void SetSoundButtVolume(float soundVolume)
        {
            if (volume != null && volume.Length > 0)
            {
                int length = volume.Length;
                float vpl = 1.0f / (float)length;
                int count = Mathf.RoundToInt(soundVolume / vpl);
                Debug.Log("soundVol: " + soundVolume + " ; count: " + count + " ;s/vpl: " + soundVolume / vpl);
                SetVolume(count);
            }
        }

        private void SetVolume(int count)
        {
            for (int i = 0; i < volume.Length; i++)
            {
                volume[i].gameObject.SetActive(i < count);
            }
        }
        #endregion volume

        public void FacebookLoginHandler(bool logined, string message)
        {
            if (facebookButtonText)
                facebookButtonText.text = (!logined) ? "Login" : "Logout";
            if (logined) MPlayer.AddFbCoins();
        }

        public void FacebookLogoutHandler()
        {
            if (facebookButtonText)
                facebookButtonText.text = (!FBholder.IsLogined) ? "Login" : "Logout";
        }
    }
}