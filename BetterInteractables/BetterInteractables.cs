using BepInEx;
using RoR2;
using UnityEngine;
using System.Collections.Generic;
using System;
using BepInEx.Configuration;
using System.Reflection;
using MonoMod.Cil;
using KinematicCharacterController;
using UnityEngine.Networking;

namespace Flood_Warning
{
    [BepInDependency("com.bepis.r2api")]

    [BepInPlugin("com.PallesenProductions.BetterInteractables", "BetterInteractables", "1.0.0")]

    public class BetterInteractables : BaseUnityPlugin
    {

        public void Awake()//Code that runs when the game starts
        {
            int randomNumber = 0;

            {
                On.EntityStates.Duplicator.Duplicating.DropDroplet += (orig, self) =>
                {
                    orig(self);

                };

                On.RoR2.ShrineChanceBehavior.Awake += (orig, self) =>
                {
                    
                        Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.ShrineChanceBehavior"), "maxPurchaseCount").SetValue(self, Run.instance.treasureRng.RangeInt(2, 5));
                        Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.ShrineChanceBehavior"), "costMultiplierPerPurchase").SetValue(self, 1.2f);
                    

                    orig(self);
                };
                On.RoR2.ShrineHealingBehavior.Awake += (orig, self) =>
                {
                    self.costMultiplierPerPurchase = 1.05f;
                    
                        Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.ShrineHealingBehavior"), "maxPurchaseCount").SetValue(self, Run.instance.treasureRng.RangeInt(5, 12));
                        Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.ShrineHealingBehavior"), "costMultiplierPerPurchase").SetValue(self, 1.2f);
                    
                    orig(self);
                };
                On.RoR2.ShrineRestackBehavior.Start += (orig, self) =>
                {
                    
                        Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.ShrineRestackBehavior"), "maxPurchaseCount").SetValue(self, Run.instance.treasureRng.RangeInt(1, 4));
                        Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.ShrineRestackBehavior"), "costMultiplierPerPurchase").SetValue(self, 1);
                    
                    orig(self);
                };

                On.RoR2.ShrineChanceBehavior.AddShrineStack += (orig, self, activator) =>
                {
                    self.tier1Weight *= 1;
                    self.tier2Weight *= 1.1f;
                    self.tier3Weight *= 1.05f;
                    orig(self, activator);
                  
                    Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.ShrineChanceBehavior"), "refreshTimer").SetValue(self, 0.1f);
                    
                };

                On.RoR2.ShrineBossBehavior.AddShrineStack += (orig, self, activator) =>
                {
                    if (TeleporterInteraction.instance.shrineBonusStacks == 0)
                    {
                        TeleporterInteraction.instance.AddShrineStack();
                    }
                    else
                    {
                        TeleporterInteraction.instance.shrineBonusStacks *= 2;
                    }
                };

                On.RoR2.SceneDirector.PlaceTeleporter += (orig, self) =>
                {
                    orig(self);
                    TeleporterInteraction.instance.clearRadius *= 1.2f;
                };

                On.RoR2.ShrineHealingBehavior.AddShrineStack += (orig, self, activator) =>
                {
                    orig(self, activator);
                    Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.ShrineHealingBehavior"), "refreshTimer").SetValue(self, 0.05f);
                };

                On.EntityStates.Duplicator.Duplicating.DropDroplet += (orig, self) =>
                {
                    orig(self);
                    if (NetworkServer.active)
                    {
                        self.outer.GetComponent<PurchaseInteraction>().Networkavailable = true;
                    }
                };

                On.EntityStates.Duplicator.Duplicating.OnEnter += (orig, self) =>
                {
                    Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("EntityStates.Duplicator.Duplicating"), "timeBetweenStartAndDropDroplet").SetValue(self, 0.05f);
                    Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("EntityStates.Duplicator.Duplicating"), "initialDelayDuration").SetValue(self, 0.05f);
                    orig(self);
                };

                On.RoR2.MultiShopController.DisableAllTerminals += (orig, self, Interactor) =>
                {
                    self.baseCost *= 12/10;
                    self.GetComponent<PurchaseInteraction>().cost *= 12 / 10;
                    self.UpdateHologramContent(self.GetHologramContentPrefab());
                };

               

            }

            bool isCustomCost = false;
            CostType customCostType = CostType.WhiteItem;
            ItemTier customTier = ItemTier.NoTier;
            int customCost = 0;

            On.RoR2.MultiShopController.Start += (orig, self) =>
            {
                customTier = (ItemTier)Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.MultiShopController"), "itemTier").GetValue(self);

                randomNumber = Run.instance.treasureRng.RangeInt(0, 100);
                if (randomNumber < 50)
                {
                    randomNumber = Run.instance.treasureRng.RangeInt(0, 100);
                    if (randomNumber < 10)//15% chance to swap out item
                    {
                        switch (customTier)
                        {
                            case ItemTier.Tier1:
                                customCostType = CostType.WhiteItem;
                                customCost = 1;
                                break;
                            case ItemTier.Tier2:
                                customCostType = CostType.GreenItem;
                                customCost = 1;
                                break;
                            case ItemTier.Tier3:
                                customCostType = CostType.RedItem;
                                customCost = 1;
                                break;
                            case ItemTier.Lunar:
                                customCostType = CostType.RedItem;
                                customCost = 1;
                                break;
                            case ItemTier.Boss:
                                customCostType = CostType.GreenItem;
                                customCost = 1;
                                break;
                        }
                        isCustomCost = true;
                    }
                    else if (randomNumber < 20)//10% Swap 2 for 1
                    {
                        switch (customTier)
                        {
                            case ItemTier.Tier1:
                                customCostType = CostType.WhiteItem;
                                customCost = 2;
                                break;
                            case ItemTier.Tier2:
                                customCostType = CostType.GreenItem;
                                customCost = 2;
                                break;
                            case ItemTier.Tier3:
                                customCostType = CostType.RedItem;
                                customCost = 2;
                                break;
                            case ItemTier.Lunar:
                                customCostType = CostType.RedItem;
                                customCost = 2;
                                break;
                            case ItemTier.Boss:
                                customCostType = CostType.GreenItem;
                                customCost = 2;
                                break;
                        }
                        isCustomCost = true;

                    }
                    else if (randomNumber < 30)//10% Trade Up (Cheap)
                    {
                        isCustomCost = true;

                        switch (customTier)
                        {
                            case ItemTier.Tier1:
                                customCostType = CostType.Money;
                                customCost = (int)Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.MultiShopController"), "baseCost").GetValue(self) / 3;
                                isCustomCost = false;
                                break;
                            case ItemTier.Tier2:
                                customCostType = CostType.WhiteItem;
                                customCost = 2;
                                break;
                            case ItemTier.Tier3:
                                customCostType = CostType.GreenItem;
                                customCost = 4;
                                break;
                            case ItemTier.Lunar:
                                customCostType = CostType.GreenItem;
                                customCost = 5;
                                break;
                            case ItemTier.Boss:
                                customCostType = CostType.GreenItem;
                                customCost = 1;
                                break;
                        }
                    }
                    else if (randomNumber < 45)//15% Trade Up (Normal)
                    {
                        isCustomCost = true;

                        switch (customTier)
                        {
                            case ItemTier.Tier1:
                                customCostType = CostType.Money;
                                customCost = (int)Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.MultiShopController"), "baseCost").GetValue(self) / 2;
                                isCustomCost = false;
                                break;
                            case ItemTier.Tier2:
                                customCostType = CostType.WhiteItem;
                                customCost = 3;
                                break;
                            case ItemTier.Tier3:
                                customCostType = CostType.GreenItem;
                                customCost = 5;
                                break;
                            case ItemTier.Lunar:
                                customCostType = CostType.GreenItem;
                                customCost = 7;
                                break;
                            case ItemTier.Boss:
                                customCostType = CostType.GreenItem;
                                customCost = 2;
                                break;
                        }
                    }
                    else if (randomNumber < 60)//15% Trade Up (Expensive)
                    {
                        isCustomCost = true;

                        switch (customTier)
                        {
                            case ItemTier.Tier1:
                                customCostType = CostType.Money;
                                customCost = (int)Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.MultiShopController"), "baseCost").GetValue(self) / 2;
                                isCustomCost = false;
                                break;
                            case ItemTier.Tier2:
                                customCostType = CostType.WhiteItem;
                                customCost = 5;
                                break;
                            case ItemTier.Tier3:
                                customCostType = CostType.GreenItem;
                                customCost = 7;
                                break;
                            case ItemTier.Lunar:
                                customCostType = CostType.GreenItem;
                                customCost = 10;
                                break;
                            case ItemTier.Boss:
                                customCostType = CostType.GreenItem;
                                customCost = 3;
                                break;
                        }
                    }
                    else if (randomNumber < 80)//20% Expensive
                    {
                        switch (customTier)
                        {
                            case ItemTier.Tier1:
                                customCostType = CostType.Money;
                                customCost = (int)Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.MultiShopController"), "baseCost").GetValue(self) * 2;
                                break;
                            case ItemTier.Tier2:
                                customCostType = CostType.Money;
                                customCost = (int)Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.MultiShopController"), "baseCost").GetValue(self) * 3;
                                break;
                            case ItemTier.Tier3:
                                customCostType = CostType.Money;
                                customCost = (int)Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.MultiShopController"), "baseCost").GetValue(self) * 5;
                                break;
                            case ItemTier.Lunar:
                                customCostType = CostType.Money;
                                customCost = (int)Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.MultiShopController"), "baseCost").GetValue(self) * 5;
                                break;
                            case ItemTier.Boss:
                                customCostType = CostType.Money;
                                customCost = (int)Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.MultiShopController"), "baseCost").GetValue(self) * 4;
                                break;
                        }
                        isCustomCost = false;

                    }
                    else if (randomNumber < 95)//15% Cheap
                    {
                        switch (customTier)
                        {
                            case ItemTier.Tier1:
                                customCostType = CostType.Money;
                                customCost = (int)Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.MultiShopController"), "baseCost").GetValue(self) / 2;
                                break;
                            case ItemTier.Tier2:
                                customCostType = CostType.Money;
                                customCost = (int)Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.MultiShopController"), "baseCost").GetValue(self);
                                break;
                            case ItemTier.Tier3:
                                customCostType = CostType.Money;
                                customCost = (int)Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.MultiShopController"), "baseCost").GetValue(self) * 3;
                                break;
                            case ItemTier.Lunar:
                                customCostType = CostType.Money;
                                customCost = (int)Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.MultiShopController"), "baseCost").GetValue(self) * 2;
                                break;
                            case ItemTier.Boss:
                                customCostType = CostType.Money;
                                customCost = (int)Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.MultiShopController"), "baseCost").GetValue(self) * 2;
                                break;
                        }
                        isCustomCost = false;
                    }
                    else if (randomNumber < 100)//5% Lunar
                    {
                        switch (customTier)
                        {
                            case ItemTier.Tier1:
                                customCostType = CostType.WhiteItem;
                                customCost = 1;
                                break;
                            case ItemTier.Tier2:
                                customCostType = CostType.GreenItem;
                                customCost = 2;
                                break;
                            case ItemTier.Tier3:
                                customCostType = CostType.Lunar;
                                customCost = 3;
                                break;
                            case ItemTier.Lunar:
                                customCostType = CostType.Lunar;
                                customCost = 2;
                                break;
                            case ItemTier.Boss:
                                customCostType = CostType.Lunar;
                                customCost = 2;
                                break;
                        }
                        isCustomCost = true;
                    }
                    Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.MultiShopController"), "baseCost").SetValue(self, customCost);
                    Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.MultiShopController"), "costType").SetValue(self, customCostType);
                }
                if (isCustomCost)
                {
                    GameObject[] terminalGameObjects = (GameObject[])Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.MultiShopController"), "terminalGameObjects").GetValue(self);

                    foreach (GameObject gameObject in terminalGameObjects)
                    {
                        gameObject.GetComponent<PurchaseInteraction>().automaticallyScaleCostWithDifficulty = false;
                        gameObject.GetComponent<PurchaseInteraction>().cost = customCost;
                        gameObject.GetComponent<PurchaseInteraction>().Networkcost = customCost;
                        gameObject.GetComponent<PurchaseInteraction>().Networkavailable = true;
                        gameObject.GetComponent<PurchaseInteraction>().costType = customCostType;
                    }
                    Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.MultiShopController"), "terminalGameObjects").SetValue(self, terminalGameObjects);

                }
                orig(self);
            };
            On.RoR2.MultiShopController.CreateTerminals += (orig, self) =>
            {
                randomNumber = Run.instance.treasureRng.RangeInt(0, 100);
                if (randomNumber < 50)
                {
                    customTier = ItemTier.Tier1;
                    bool failed = false;
                    while (failed == false)
                    {
                        randomNumber = Run.instance.treasureRng.RangeInt(0, 100);
                        if (randomNumber > 15 || customTier == ItemTier.Boss)
                        {
                            failed = true;
                            break;
                        }
                        else
                        {
                            customTier += 1;
                        }
                    }
                    Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.MultiShopController"), "itemTier").SetValue(self, customTier);
                }

                orig(self);
            };
            On.RoR2.Run.GetDifficultyScaledCost += (orig, self, baseCost) =>
            {

                if (isCustomCost)
                {

                    isCustomCost = false;
                    return baseCost;
                }
                else
                {
                    return orig(self, baseCost);
                }

            };
        }
    }

    
}
