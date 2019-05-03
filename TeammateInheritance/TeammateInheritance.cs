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
using RoR2.CharacterAI;

namespace Flood_Warning
{
    [BepInDependency("com.bepis.r2api")]

    [BepInPlugin("com.PallesenProductions.TeammateInheritance", "TeammateInheritance", "1.0.0")]

    public class SizeManager : BaseUnityPlugin
    {

        public void Awake()
        {

            var addRule = typeof(RuleCatalog).GetMethod("AddRule", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { typeof(RuleDef) }, null);
            var addCategory = typeof(RuleCatalog).GetMethod("AddCategory", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { typeof(string), typeof(Color), typeof(string), typeof(Func<bool>) }, null);

            addCategory.Invoke(null, new object[] { "Ally Editor", new Color(94 / 255, 82 / 255, 30 / 255, byte.MaxValue), "", new Func<bool>(() => false) });

            RuleDef allySpawnType = new RuleDef("FloodWarning.allySpawnType", "Guaranteed");


            RuleChoiceDef myRule = allySpawnType.AddChoice("0", 0, false);
            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
            myRule.tooltipNameToken = "Vanilla";
            myRule.tooltipBodyToken = "Allies do not spawn with any extra items";
            RuleChoiceDef myRule2 = allySpawnType.AddChoice("0", 1, false);
            myRule2.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
            myRule2.tooltipNameToken = "Item Ratio";
            myRule2.tooltipBodyToken = "Allies will gain extra items based on how many their owner has (Ratio editable below)";
            allySpawnType.MakeNewestChoiceDefault();
            RuleChoiceDef myRule3 = allySpawnType.AddChoice("0", 2, false);
            myRule3.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
            myRule3.tooltipNameToken = "Direct Copy";
            myRule3.tooltipBodyToken = "Allies will have a direct copy of their owner's items, not including equipment";
            RuleChoiceDef myRule4 = allySpawnType.AddChoice("0", 2, false);
            myRule4.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
            myRule4.tooltipNameToken = "Direct Copy+";
            myRule4.tooltipBodyToken = "Allies will have a direct copy of their owner's items, Including equipment";

            addRule.Invoke(null, new object[] { allySpawnType });

            RuleDef allyItemRatio = new RuleDef("FloodWarning.allyItemRatio", "Guaranteed");

            for (float o = 0; o <= 20; o++)
            {
                float myNum = o / 10;
                RuleChoiceDef myRule5 = allyItemRatio.AddChoice("0", myNum, false);
                myRule5.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                myRule5.tooltipNameToken = "" + myNum * 10 + " to 10";
                myRule5.tooltipBodyToken = "Allies will spawn with " + myNum + "x the items their owner has";
                if (myNum == 0.4f) { allyItemRatio.MakeNewestChoiceDefault(); }
            }
            addRule.Invoke(null, new object[] { allyItemRatio });

            On.RoR2.SummonMasterBehavior.OpenSummon += (orig, self, activator) =>
            {
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(self.masterPrefab, self.transform.position, self.transform.rotation);
                CharacterBody component = activator.GetComponent<CharacterBody>();
                CharacterMaster master = component.master;
                CharacterMaster component2 = gameObject.GetComponent<CharacterMaster>();
                component2.teamIndex = TeamComponent.GetObjectTeam(component.gameObject);
                Inventory component3 = gameObject.GetComponent<Inventory>();
                giveItems(component3, master.inventory, (int)(Run.instance.ruleBook.GetRuleChoice(RuleCatalog.FindRuleDef("FloodWarning.allySpawnType")).extraData));
                NetworkServer.Spawn(gameObject);
                component2.SpawnBody(component2.bodyPrefab, self.transform.position + Vector3.up * 0.8f, self.transform.rotation);
                AIOwnership component4 = gameObject.GetComponent<AIOwnership>();
                if (component4 && component && master)
                {
                    component4.ownerMaster = master;
                }
                BaseAI component5 = gameObject.GetComponent<BaseAI>();
                if (component5)
                {
                    component5.leader.gameObject = activator.gameObject;
                }
                UnityEngine.Object.Destroy(self.gameObject);
            };

            On.RoR2.EquipmentSlot.SummonMaster += (orig, self, masterObjectPrefab, position) =>
            {
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(masterObjectPrefab, position, self.transform.rotation);
                CharacterBody component = self.GetComponent<CharacterBody>();
                CharacterMaster master = component.master;
                CharacterMaster component2 = gameObject.GetComponent<CharacterMaster>();
                component2.teamIndex = TeamComponent.GetObjectTeam(component.gameObject);
                Inventory component3 = gameObject.GetComponent<Inventory>();
                giveItems(component3, master.inventory, (int)(Run.instance.ruleBook.GetRuleChoice(RuleCatalog.FindRuleDef("FloodWarning.allySpawnType")).extraData));
                NetworkServer.Spawn(gameObject);
                component2.SpawnBody(component2.bodyPrefab, position, self.transform.rotation);
                AIOwnership component4 = gameObject.GetComponent<AIOwnership>();
                if (component4 && component && master)
                {
                    component4.ownerMaster = master;
                }
                BaseAI component5 = gameObject.GetComponent<BaseAI>();
                if (component5)
                {
                    component5.leader.gameObject = self.gameObject;
                }
                return component2;
            };

            //    On.RoR2.CharacterBody.UpdateBeetleGuardAllies += (orig, self) =>
            //    {
            //        if (NetworkServer.active)
            //        {
            //            int num = self.inventory ? self.inventory.GetItemCount(ItemIndex.BeetleGland) : 0;
            //            if (num > 0 && self.master.GetDeployableCount(DeployableSlot.BeetleGuardAlly) < num)
            //            {
            //                self.SetFieldValue("guardResummonCooldown", self.GetFieldValue<float>("guardResummonCooldown") - Time.fixedDeltaTime);

            //                if (self.GetFieldValue<float>("guardResummonCooldown") <= 0f)
            //                {
            //                    self.SetFieldValue("guardResummonCooldown", 30f);
            //                    GameObject gameObject = DirectorCore.instance.TrySpawnObject((SpawnCard)Resources.Load("SpawnCards/CharacterSpawnCards/cscBeetleGuardAlly"), new DirectorPlacementRule
            //                    {
            //                        placementMode = DirectorPlacementRule.PlacementMode.Approximate,
            //                        minDistance = 3f,
            //                        maxDistance = 40f,
            //                        spawnOnTarget = self.transform
            //                    }, RoR2Application.rng);
            //                    if (gameObject)
            //                    {
            //                        CharacterMaster component = gameObject.GetComponent<CharacterMaster>();

            //                        Inventory inventory = gameObject.GetComponent<Inventory>();

            //                        inventory.CopyItemsFrom(self.inventory);
            //                        giveItems(inventory, self.inventory, (int)(Run.instance.ruleBook.GetRuleChoice(RuleCatalog.FindRuleDef("FloodWarning.allySpawnType")).extraData));

            //                        AIOwnership component2 = gameObject.GetComponent<AIOwnership>();
            //                        BaseAI component3 = gameObject.GetComponent<BaseAI>();
            //                        if (component)
            //                        {
            //                            component.teamIndex = TeamComponent.GetObjectTeam(self.gameObject);
            //                            component.inventory.GiveItem(ItemIndex.BoostDamage, 30);
            //                            component.inventory.GiveItem(ItemIndex.BoostHp, 10);
            //                            GameObject bodyObject = component.GetBodyObject();
            //                            if (bodyObject)
            //                            {
            //                                Deployable component4 = bodyObject.GetComponent<Deployable>();
            //                                self.master.AddDeployable(component4, DeployableSlot.BeetleGuardAlly);
            //                            }
            //                        }
            //                        if (component2)
            //                        {
            //                            component2.ownerMaster = self.master;
            //                        }
            //                        if (component3)
            //                        {
            //                            component3.leader.gameObject = self.gameObject;
            //                        }
            //                    }
            //                }
            //            }
            //        }

            //    };
        }

        public static void giveItems(Inventory summonInventory, Inventory playerInventory, int GiveRule)
        {
            if (GiveRule == 1)
            {
                float tier1Count = 0;
                float tier2Count = 0;
                float tier3Count = 0;
                float tier4Count = 0;
                bool hasEquipment = false;
                foreach (ItemIndex index in ItemCatalog.tier1ItemList)
                {
                    tier1Count += playerInventory.GetItemCount(index);
                }
                foreach (ItemIndex index in ItemCatalog.tier2ItemList)
                {
                    tier2Count += playerInventory.GetItemCount(index);
                }
                foreach (ItemIndex index in ItemCatalog.tier3ItemList)
                {
                    tier3Count += playerInventory.GetItemCount(index);
                }
                foreach (ItemIndex index in ItemCatalog.lunarItemList)
                {
                    tier4Count += playerInventory.GetItemCount(index);
                }

                tier1Count *= (float)(Run.instance.ruleBook.GetRuleChoice(RuleCatalog.FindRuleDef("FloodWarning.allyItemRatio")).extraData);
                tier2Count *= (float)(Run.instance.ruleBook.GetRuleChoice(RuleCatalog.FindRuleDef("FloodWarning.allyItemRatio")).extraData);
                tier3Count *= (float)(Run.instance.ruleBook.GetRuleChoice(RuleCatalog.FindRuleDef("FloodWarning.allyItemRatio")).extraData);
                tier4Count *= (float)(Run.instance.ruleBook.GetRuleChoice(RuleCatalog.FindRuleDef("FloodWarning.allyItemRatio")).extraData);

                hasEquipment = (new System.Random().Next(0, 100) < 100 * (float)(Run.instance.ruleBook.GetRuleChoice(RuleCatalog.FindRuleDef("FloodWarning.allyItemRatio")).extraData) && (playerInventory.GetEquipmentIndex() != EquipmentIndex.None));

                for (int i = 0; i < tier1Count; i++)
                {
                    summonInventory.GiveItem(ItemCatalog.tier1ItemList[Run.instance.runRNG.RangeInt(0, ItemCatalog.tier1ItemList.Count)], 1);
                }
                for (int i = 0; i < tier2Count; i++)
                {
                    summonInventory.GiveItem(ItemCatalog.tier2ItemList[Run.instance.runRNG.RangeInt(0, ItemCatalog.tier2ItemList.Count)], 1);

                }
                for (int i = 0; i < tier3Count; i++)
                {
                    summonInventory.GiveItem(ItemCatalog.tier3ItemList[Run.instance.runRNG.RangeInt(0, ItemCatalog.tier3ItemList.Count)], 1);

                }
                for (int i = 0; i < tier4Count; i++)
                {
                    summonInventory.GiveItem(ItemCatalog.lunarItemList[Run.instance.runRNG.RangeInt(0, ItemCatalog.lunarItemList.Count)], 1);

                }
                if (hasEquipment)
                {
                    summonInventory.SetEquipmentIndex(Run.instance.availableEquipmentDropList[Run.instance.runRNG.RangeInt(0, Run.instance.availableEquipmentDropList.Count)].equipmentIndex);
                    summonInventory.GiveItem(ItemIndex.AutoCastEquipment, 1);
                }

            }
            if (GiveRule == 2)
            {
                summonInventory.CopyItemsFrom(playerInventory);
            }
            if (GiveRule == 3)
            {
                if (playerInventory.GetEquipmentIndex() != EquipmentIndex.DroneBackup)
                {
                    summonInventory.CopyEquipmentFrom(playerInventory);
                    summonInventory.GiveItem(ItemIndex.AutoCastEquipment, 1);
                }
            }
            summonInventory.ResetItem(ItemIndex.TreasureCache);
            summonInventory.ResetItem(ItemIndex.Feather);
            summonInventory.ResetItem(ItemIndex.Firework);
            summonInventory.ResetItem(ItemIndex.Talisman);
            summonInventory.ResetItem(ItemIndex.SprintArmor);
            summonInventory.ResetItem(ItemIndex.JumpBoost);
            summonInventory.ResetItem(ItemIndex.GoldOnHit);
            summonInventory.ResetItem(ItemIndex.WardOnLevel);
            summonInventory.ResetItem(ItemIndex.BeetleGland);
            summonInventory.ResetItem(ItemIndex.CrippleWardOnLevel);
            summonInventory.ResetItem(ItemIndex.ExtraLife);
        }
    }
}
