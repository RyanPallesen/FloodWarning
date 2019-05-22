using BepInEx;
using RoR2;
using UnityEngine;

namespace ItemBlacklister
{



    [BepInDependency("com.bepis.r2api")]

    [BepInPlugin("com.PallesenProductions.ItemBlacklister", "ItemBlackLister", "2.0.1")]

    class ItemBlacklister : BaseUnityPlugin
    {


        public void Awake()
        {

            On.RoR2.RuleDef.AddChoice += (orig, self, choiceName, extraData, excludeByDefault) =>
            {
                excludeByDefault = false;
                var myvar = orig(self, choiceName, extraData, excludeByDefault);
                return myvar;
            };

            On.RoR2.RuleCatalog.HiddenTestItemsConvar += (self) =>
            {
                return false;
            };
            On.RoR2.RuleCatalog.HiddenTestTrue += (self) =>
            {
                return false;
            };

        }
    }
}