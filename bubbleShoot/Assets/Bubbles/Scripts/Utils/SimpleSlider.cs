using UnityEngine;
using UnityEngine.UI;
/*
 13.01.19
    -add fillImage exist
 */

namespace Mkey
{
    public class SimpleSlider : MonoBehaviour
    {
        public Image fillImage;

        public float value
        {
            get
            {
                return (fillImage)?fillImage.fillAmount:0;
            }
            set
            {
               if (fillImage) fillImage.fillAmount = value;
            }
        }
    }
}