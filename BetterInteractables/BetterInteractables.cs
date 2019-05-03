using BepInEx;
using RoR2;
using UnityEngine;
using System.Collections.Generic;
using System;
using BepInEx.Configuration;
using System.Reflection;
using MonoMod.Cil;
using KinematicCharacterController;

namespace Flood_Warning
{
    [BepInDependency("com.bepis.r2api")]

    [BepInPlugin("com.PallesenProductions.BetterInteractables", "BetterInteractables", "1.0.0")]

    public class BetterInteractables : BaseUnityPlugin
    {

        public void Awake()//Code that runs when the game starts
        {
            var addRule = typeof(RuleCatalog).GetMethod("AddRule", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { typeof(RuleDef) }, null);
            var addCategory = typeof(RuleCatalog).GetMethod("AddCategory", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { typeof(string), typeof(Color), typeof(string), typeof(Func<bool>) }, null);
            int randomNumber = 0;

            addCategory.Invoke(null, new object[] { "Better Triple Shops", new Color(219 / 255, 182 / 255, 19 / 255, byte.MaxValue), "", new Func<bool>(() => false) });

            RuleDef shopExistenceRule = new RuleDef("FloodWarning.BetterShopExistenceChances", "Chance of a success on each roll of a Better Triple Shop Tier");
            for (int i = 0; i <= 20; i++)
            {
                float myNum = (i * 5) / 100f;

                RuleChoiceDef myRule = shopExistenceRule.AddChoice("0", myNum * 100f, false);
                if (myNum * 100 == 50f) { shopExistenceRule.MakeNewestChoiceDefault(); }
                myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                myRule.tooltipNameToken = "" + myNum * 100 + " Base Chance";
                myRule.tooltipBodyToken = myNum * 100 + "% Chance to replace a vanilla triple shop with a custom one (Varied costs, cost types and rewards)";
                if (i == 0f)
                {
                    myRule.tooltipNameToken = "0% Chance";
                    myRule.tooltipBodyToken = "Triple shops will never have varied costs/tiers apart from vanilla";
                }
            }
            addRule.Invoke(null, new object[] { shopExistenceRule });

            RuleDef shopChancesRule = new RuleDef("FloodWarning.BetterShopChances", "Chance of a success on each roll of a Better Triple Shop Tier");
            for (int i = 0; i <= 20; i++)
            {
                float myNum = (i * 5) / 100f;

                RuleChoiceDef myRule = shopChancesRule.AddChoice("0", myNum * 100f, false);
                if (myNum * 100 == 50f) { shopChancesRule.MakeNewestChoiceDefault(); }
                myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                myRule.tooltipNameToken = "" + myNum * 100 + " Chance to go up a tier (each tier)";
                myRule.tooltipBodyToken = (Math.Pow(myNum, 1) * 100).ToString("#.###") + "% reaches White, " + (Math.Pow(myNum, 2) * 100).ToString("#.###") + "% reaches Green, " + (Math.Pow(myNum, 3) * 100).ToString("#.###") + "% reaches Red, " + (Math.Pow(myNum, 4) * 100).ToString("#.###") + "% reaches Lunar, " + (Math.Pow(myNum, 5) * 100).ToString("#.###") + "% reaches Boss ";
                if (i == 0f)
                {
                    myRule.tooltipNameToken = "0% Chance";
                    myRule.tooltipBodyToken = "Triple shops will never have varied costs/tiers apart from vanilla";
                }
            }
            addRule.Invoke(null, new object[] { shopChancesRule });

            addCategory.Invoke(null, new object[] { "Better Shrines", new Color(219 / 255, 182 / 255, 19 / 255, byte.MaxValue), "", new Func<bool>(() => false) });

            RuleDef betterShrineRule = new RuleDef("FloodWarning.betterShrine", "Whether or not the game should have better shrines");
            {
                RuleChoiceDef myRule = betterShrineRule.AddChoice("0", false, false);
                myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                myRule.tooltipNameToken = "Off";
                myRule.tooltipBodyToken = "Shrines and Duplicators are vanilla";

                RuleChoiceDef myRule2 = betterShrineRule.AddChoice("0", true, false);
                myRule2.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                myRule2.tooltipNameToken = "On";
                myRule2.tooltipBodyToken = "Shrines can be used more times (Random on generation, Different ranges for different shrines), shrines can be used quicker";
                betterShrineRule.MakeNewestChoiceDefault();
                addRule.Invoke(null, new object[] { betterShrineRule });
            }

            {
                On.EntityStates.Duplicator.Duplicating.DropDroplet += (orig, self) =>
                {
                    orig(self);

                };

                On.RoR2.ShrineChanceBehavior.Awake += (orig, self) =>
                {
                    if ((bool)Run.instance.ruleBook.GetRuleChoice(RuleCatalog.FindRuleDef("FloodWarning.betterShrine")).extraData)
                    {
                        Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.ShrineChanceBehavior"), "maxPurchaseCount").SetValue(self, Run.instance.treasureRng.RangeInt(2, 5));
                        Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.ShrineChanceBehavior"), "costMultiplierPerPurchase").SetValue(self, 1.2f);
                    }

                    orig(self);
                };
                On.RoR2.ShrineHealingBehavior.Awake += (orig, self) =>
                {
                    self.costMultiplierPerPurchase = 1.05f;
                    if ((bool)Run.instance.ruleBook.GetRuleChoice(RuleCatalog.FindRuleDef("FloodWarning.betterShrine")).extraData)
                    {
                        Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.ShrineHealingBehavior"), "maxPurchaseCount").SetValue(self, Run.instance.treasureRng.RangeInt(5, 12));
                        Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.ShrineHealingBehavior"), "costMultiplierPerPurchase").SetValue(self, 1.2f);
                    }
                    orig(self);
                };
                On.RoR2.ShrineRestackBehavior.Start += (orig, self) =>
                {
                    if ((bool)Run.instance.ruleBook.GetRuleChoice(RuleCatalog.FindRuleDef("FloodWarning.betterShrine")).extraData)
                    {
                        Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.ShrineRestackBehavior"), "maxPurchaseCount").SetValue(self, Run.instance.treasureRng.RangeInt(1, 4));
                        Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.ShrineRestackBehavior"), "costMultiplierPerPurchase").SetValue(self, 1);
                    }
                    orig(self);
                };

                On.RoR2.ShrineChanceBehavior.AddShrineStack += (orig, self, activator) =>
                {
                    self.tier1Weight *= 1;
                    self.tier2Weight *= 1.1f;
                    self.tier3Weight *= 1.05f;
                    orig(self, activator);
                    if ((bool)Run.instance.ruleBook.GetRuleChoice(RuleCatalog.FindRuleDef("FloodWarning.betterShrine")).extraData)
                    {
                        Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.ShrineChanceBehavior"), "refreshTimer").SetValue(self, 0.1f);
                    }
                };
                On.RoR2.ShrineHealingBehavior.AddShrineStack += (orig, self, activator) =>
                {

                    orig(self, activator);
                    if ((bool)Run.instance.ruleBook.GetRuleChoice(RuleCatalog.FindRuleDef("FloodWarning.betterShrine")).extraData)
                    {
                        Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.ShrineHealingBehavior"), "refreshTimer").SetValue(self, 0.1f);
                    }
                };

                On.EntityStates.Duplicator.Duplicating.OnEnter += (orig, self) =>
                {
                    orig(self);
                    if ((bool)Run.instance.ruleBook.GetRuleChoice(RuleCatalog.FindRuleDef("FloodWarning.betterShrine")).extraData)
                    {
                        Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("EntityStates.Duplicator.Duplicating"), "timeBetweenStartAndDropDroplet").SetValue(self, 1f);
                        Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("EntityStates.Duplicator.Duplicating"), "initialDelayDuration").SetValue(self, 0.2f);

                    }
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
                if (randomNumber < (float)Run.instance.ruleBook.GetRuleChoice(RuleCatalog.FindRuleDef("FloodWarning.BetterShopExistenceChances")).extraData || customTier != ItemTier.Tier1)
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
                    else if (randomNumber < 70)//10% cost health
                    {
                        switch (customTier)
                        {
                            case ItemTier.Tier1:
                                customCostType = CostType.PercentHealth;
                                customCost = 99;
                                break;
                            case ItemTier.Tier2:
                                customCostType = CostType.PercentHealth;
                                customCost = 99;
                                break;
                            case ItemTier.Tier3:
                                customCostType = CostType.PercentHealth;
                                customCost = 99;
                                break;
                            case ItemTier.Lunar:
                                customCostType = CostType.PercentHealth;
                                customCost = 99;
                                break;
                            case ItemTier.Boss:
                                customCostType = CostType.PercentHealth;
                                customCost = 99;
                                break;
                        }
                        isCustomCost = true;

                    }
                    else if (randomNumber < 80)//10% Expensive
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
                    else if (randomNumber < 90)//10% Cheap
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
                    else if (randomNumber < 100)//10% Lunar
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
                        gameObject.GetComponent<PurchaseInteraction>().costType = customCostType;
                    }
                    Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.MultiShopController"), "terminalGameObjects").SetValue(self, terminalGameObjects);

                }
                orig(self);
            };
            On.RoR2.MultiShopController.CreateTerminals += (orig, self) =>
            {
                randomNumber = Run.instance.treasureRng.RangeInt(0, 100);
                if (randomNumber < (float)Run.instance.ruleBook.GetRuleChoice(RuleCatalog.FindRuleDef("FloodWarning.BetterShopExistenceChances")).extraData)
                {
                    customTier = ItemTier.Tier1;
                    bool failed = false;
                    while (failed == false)
                    {
                        randomNumber = Run.instance.treasureRng.RangeInt(0, 100);
                        if (randomNumber < (float)Run.instance.ruleBook.GetRuleChoice(RuleCatalog.FindRuleDef("FloodWarning.BetterShopChances")).extraData || customTier == ItemTier.Boss)
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
