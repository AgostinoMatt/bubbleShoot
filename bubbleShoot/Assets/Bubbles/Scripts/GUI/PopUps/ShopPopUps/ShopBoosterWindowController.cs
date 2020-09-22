using System.Collections.Generic;
using UnityEngine;

namespace Mkey
{
    public enum InGameShopType { None, BoosterMulticolor, BoosterFireBall, BoosterEyeBall};
    public class ShopBoosterWindowController : PopUpsController
    {
        public RectTransform ThingsParent;
        private List<ShopThingHelper> shopThings;
        [SerializeField]
        private InGameShopType shopType;

        void Start()
        {
           
        }

        public override void RefreshWindow()
        {
            CreateThingTab();
            base.RefreshWindow();
        }

        private void CreateThingTab()
        {
            ShopThingHelper[] sT = ThingsParent.GetComponentsInChildren<ShopThingHelper>();
            foreach (var item in sT)
            {
                DestroyImmediate(item.gameObject);
            }

            InGamePurchaser p = InGamePurchaser.Instance;
            if (p == null) return;

            List<ShopThingDataInGame> products = new List<ShopThingDataInGame>();
            if (p.gameProducts != null && p.gameProducts.Length > 0) products.AddRange(p.gameProducts);


            if (products.Count == 0) return;

            shopThings = new List<ShopThingHelper>();
            for (int i = 0; i < products.Count; i++)
            {
                if (products[i] != null && (products[i].shopType == shopType) && products[i].prefab) shopThings.Add (ShopThingHelper.CreateShopThingsHelper(products[i].prefab, ThingsParent, products[i]));
            }
        }
    }
}
