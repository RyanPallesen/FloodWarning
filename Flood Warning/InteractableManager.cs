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

    public class InteractableManager : BaseUnityPlugin
    {

        public static bool RunInteractableManager()
        {
            string[] interactables = { "iscBrokenDrone1", "iscBrokenDrone2", "iscBrokenMegaDrone", "iscBrokenMissileDrone", "iscBrokenTurret1", "iscBarrel1", "iscEquipmentBarrel", "iscLockbox", "iscChest1", "iscChest1Stealthed", "iscChest2", "iscGoldChest", "iscTripleShop", "iscTripleShopLarge", "iscDuplicator", "iscDuplicatorLarge", "iscDuplicatorMilitary", "iscShrineCombat", "iscShrineBoss", "iscShrineBlood", "iscShrineChance", "iscShrineHealing", "iscShrineRestack", "iscShrineGoldshoresAccess", "iscRadarTower", "iscTeleporter", "iscShopPortal", "iscMSPortal", "iscGoldshoresPortal", "iscGoldshoresBeacon" };
            string[] interactableNames = { "Gunner Drone", "Healing Drone", "Prototype Drone", "Broke Missile Drone", "Broken Turret", "Barrel", "Equipment Barrel", "Rusty Lockbox", "Chest", "Stealthed Chest", "Large Chest", "Legendary Chest", "Triple Shop", "Triple Shop (Red and Green)", "3D Printer", "Large Printer (Green)", "Mili-tech Printer (Red)", "Shrine of Combat", "Shrine of the Mountain", "Shrine of Blood", "Shrine of Chance", "Shrine of the Forest", "Shrine of Order", "Gold Shrine", "Radar", "Teleporter", "Blue Portal", "Celestial Portal", "Gold Portal", "Halycon Beacon" };

            var addRule = typeof(RuleCatalog).GetMethod("AddRule", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { typeof(RuleDef) }, null);
            var addCategory = typeof(RuleCatalog).GetMethod("AddCategory", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { typeof(string), typeof(Color), typeof(string), typeof(Func<bool>) }, null);


            addCategory.Invoke(null, new object[] { "Interactables", new Color(219 / 255, 182 / 255, 19 / 255, byte.MaxValue), "", new Func<bool>(() => false) });
            RuleDef iscruleDef3 = new RuleDef("FloodWarning.InteractableScale", "Total interactable scale");
            for (int i = 0; i < 8; i++)
            {
                float myNum = (float)(Math.Pow(2f, i)) / 4f;
                if (i == 0)
                {
                    RuleChoiceDef tempRule = iscruleDef3.AddChoice("0", 0f, false);
                    tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                    tempRule.tooltipNameToken = "" + 0f + "x Interactable spawns";
                    tempRule.tooltipBodyToken = "" + 0f + "x The amount of interactables (Shrines, Chests, Buyable drones and turrets) will appear in the world";

                }

                RuleChoiceDef myRule = iscruleDef3.AddChoice("0", myNum, false);
                if (myNum == 1) { iscruleDef3.MakeNewestChoiceDefault(); }
                myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                myRule.tooltipNameToken = "" + myNum + "x Interactable spawns";
                myRule.tooltipBodyToken = "" + myNum + "x The amount of interactables (Shrines, Chests, Buyable drones and turrets) will appear in the world";

            }
            addRule.Invoke(null, new object[] { iscruleDef3 });

            addCategory.Invoke(null, new object[] { "Weighted Selections", new Color(94 / 255, 82 / 255, 30 / 255, byte.MaxValue), "", new Func<bool>(() => false) });
            for (int i = 0; i < interactables.Length; i++)
            {
                RuleDef ChanceRule = new RuleDef("FloodWarning." + interactables[i] + "Chance", "Chance");

                for (int o = 0; o < 8; o++)
                {
                    float myNum = (float)(Math.Pow(2f, o)) / 4f;
                    if (o == 0)
                    {
                        RuleChoiceDef tempRule = ChanceRule.AddChoice("0", 0f, false);
                        tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                        tempRule.tooltipNameToken = "0x Chance of" + interactableNames[i];
                        tempRule.tooltipBodyToken = "0x Chance that a given spawned interactable will be of type '" + interactableNames[i] + "'";
                        switch (interactables[i])
                        {

                            case "iscBrokenDrone1":
                                tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                                break;
                            case "iscBrokenDrone2":
                                tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                                break;
                            case "iscBrokenMegaDrone":
                                tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                                break;
                            case "iscBrokenMissileDrone":
                                tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                                break;
                            case "iscBrokenTurret1":
                                tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                                break;
                            case "iscBarrel1":
                                tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                                break;
                            case "iscEquipmentBarrel":
                                tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                                break;
                            case "iscLockbox":
                                tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                                break;
                            case "iscChest1":
                                tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                                break;
                            case "iscChest1Stealthed":
                                tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                                break;
                            case "iscChest2":
                                tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                                break;
                            case "iscGoldChest":
                                tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                                break;
                            case "iscTripleShop":
                                tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                                break;
                            case "iscTripleShopLarge":
                                tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                                break;
                            case "iscDuplicator":
                                tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                                break;
                            case "iscDuplicatorLarge":
                                tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                                break;
                            case "iscDuplicatorMilitary":
                                tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                                break;
                            case "iscShrineCombat":
                                tempRule.spritePath = "Textures/shrinesymbols/texshrinecombatsymbol";
                                break;
                            case "iscShrineBoss":
                                tempRule.spritePath = "Textures/shrinesymbols/texshrinebosssymbole";
                                break;
                            case "iscShrineBlood":
                                tempRule.spritePath = "Textures/shrinesymbols/texshrinebloodsymbol";
                                break;
                            case "iscShrineChance":
                                tempRule.spritePath = "Textures/shrinesymbols/texshrinechancesymbol";
                                break;
                            case "iscShrineHealing":
                                tempRule.spritePath = "Textures/shrinesymbols/texshrinehealingsymbol";
                                break;
                            case "iscShrineRestack":
                                tempRule.spritePath = "Textures/shrinesymbols/texshrinerestacksymbol";
                                break;
                            case "iscShrineGoldshoresAccess":
                                tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                                break;
                            case "iscRadarTower":
                                tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                                break;
                            case "iscTeleporter":
                                tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                                break;
                            case "iscShopPortal":
                                tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                                break;
                            case "iscMSPortal":
                                tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                                break;
                            case "iscGoldshoresPortal":
                                tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                                break;
                            case "iscGoldshoresBeacon":
                                tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                                break;


                        }
                    }
                    RuleChoiceDef myRule = ChanceRule.AddChoice("0", myNum, false);
                    if (myNum == 1) { ChanceRule.MakeNewestChoiceDefault(); }

                    myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                    myRule.tooltipNameToken = "" + myNum + "x Chance of " + interactableNames[i];
                    myRule.tooltipBodyToken = "" + myNum + "x Chance that a given spawned interactable will be of type '" + interactableNames[i] + "'";
                    switch (interactables[i])
                    {

                        case "iscBrokenDrone1":
                            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                            break;
                        case "iscBrokenDrone2":
                            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                            break;
                        case "iscBrokenMegaDrone":
                            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                            break;
                        case "iscBrokenMissileDrone":
                            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                            break;
                        case "iscBrokenTurret1":
                            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                            break;
                        case "iscBarrel1":
                            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                            break;
                        case "iscEquipmentBarrel":
                            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                            break;
                        case "iscLockbox":
                            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                            break;
                        case "iscChest1":
                            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                            break;
                        case "iscChest1Stealthed":
                            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                            break;
                        case "iscChest2":
                            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                            break;
                        case "iscGoldChest":
                            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                            break;
                        case "iscTripleShop":
                            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                            break;
                        case "iscTripleShopLarge":
                            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                            break;
                        case "iscDuplicator":
                            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                            break;
                        case "iscDuplicatorLarge":
                            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                            break;
                        case "iscDuplicatorMilitary":
                            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                            break;
                        case "iscShrineCombat":
                            myRule.spritePath = "Textures/shrinesymbols/texshrinecombatsymbol";
                            break;
                        case "iscShrineBoss":
                            myRule.spritePath = "Textures/shrinesymbols/texshrinebosssymbole";
                            break;
                        case "iscShrineBlood":
                            myRule.spritePath = "Textures/shrinesymbols/texshrinebloodsymbol";
                            break;
                        case "iscShrineChance":
                            myRule.spritePath = "Textures/shrinesymbols/texshrinechancesymbol";
                            break;
                        case "iscShrineHealing":
                            myRule.spritePath = "Textures/shrinesymbols/texshrinehealingsymbol";
                            break;
                        case "iscShrineRestack":
                            myRule.spritePath = "Textures/shrinesymbols/texshrinerestacksymbol";
                            break;
                        case "iscShrineGoldshoresAccess":
                            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                            break;
                        case "iscRadarTower":
                            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                            break;
                        case "iscTeleporter":
                            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                            break;
                        case "iscShopPortal":
                            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                            break;
                        case "iscMSPortal":
                            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                            break;
                        case "iscGoldshoresPortal":
                            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                            break;
                        case "iscGoldshoresBeacon":
                            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                            break;


                    }
                }
                addRule.Invoke(null, new object[] { ChanceRule });
            }

            addCategory.Invoke(null, new object[] { "Guarantees", new Color(94 / 255, 82 / 255, 30 / 255, byte.MaxValue), "", new Func<bool>(() => false) });
            for (int i = 0; i < interactables.Length; i++)
            {
                RuleDef GuaranteeRule = new RuleDef("FloodWarning." + interactables[i] + "Guaranteed", "Guaranteed");

                for (int o = 0; o < 8; o++)
                {
                    float myNum = (float)Math.Pow(2f, o);
                    if (o == 0)
                    {
                        RuleChoiceDef tempRule = GuaranteeRule.AddChoice("0", 0f, false);
                        tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                        tempRule.tooltipNameToken = "0 Guaranteed " + interactableNames[i];
                        tempRule.tooltipBodyToken = "No Guaranteee that any " + interactableNames[i] + " will spawn";
                        switch (interactables[i])
                        {

                            case "iscBrokenDrone1":
                                tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                                break;
                            case "iscBrokenDrone2":
                                tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                                break;
                            case "iscBrokenMegaDrone":
                                tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                                break;
                            case "iscBrokenMissileDrone":
                                tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                                break;
                            case "iscBrokenTurret1":
                                tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                                break;
                            case "iscBarrel1":
                                tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                                break;
                            case "iscEquipmentBarrel":
                                tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                                break;
                            case "iscLockbox":
                                tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                                break;
                            case "iscChest1":
                                tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                                break;
                            case "iscChest1Stealthed":
                                tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                                break;
                            case "iscChest2":
                                tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                                break;
                            case "iscGoldChest":
                                tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                                break;
                            case "iscTripleShop":
                                tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                                break;
                            case "iscTripleShopLarge":
                                tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                                break;
                            case "iscDuplicator":
                                tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                                break;
                            case "iscDuplicatorLarge":
                                tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                                break;
                            case "iscDuplicatorMilitary":
                                tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                                break;
                            case "iscShrineCombat":
                                tempRule.spritePath = "Textures/shrinesymbols/texshrinecombatsymbol";
                                break;
                            case "iscShrineBoss":
                                tempRule.spritePath = "Textures/shrinesymbols/texshrinebosssymbole";
                                break;
                            case "iscShrineBlood":
                                tempRule.spritePath = "Textures/shrinesymbols/texshrinebloodsymbol";
                                break;
                            case "iscShrineChance":
                                tempRule.spritePath = "Textures/shrinesymbols/texshrinechancesymbol";
                                break;
                            case "iscShrineHealing":
                                tempRule.spritePath = "Textures/shrinesymbols/texshrinehealingsymbol";
                                break;
                            case "iscShrineRestack":
                                tempRule.spritePath = "Textures/shrinesymbols/texshrinerestacksymbol";
                                break;
                            case "iscShrineGoldshoresAccess":
                                tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                                break;
                            case "iscRadarTower":
                                tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                                break;
                            case "iscTeleporter":
                                tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                                break;
                            case "iscShopPortal":
                                tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                                break;
                            case "iscMSPortal":
                                tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                                break;
                            case "iscGoldshoresPortal":
                                tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                                break;
                            case "iscGoldshoresBeacon":
                                tempRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                                break;


                        }
                    }
                    RuleChoiceDef myRule = GuaranteeRule.AddChoice("0", myNum, false);
                    if (myNum == 0) { GuaranteeRule.MakeNewestChoiceDefault(); }
                    myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                    myRule.tooltipNameToken = "" + myNum + " Guaranteed " + interactableNames[i];
                    myRule.tooltipBodyToken = "" + myNum + " " + interactableNames[i] + "(s) Will always spawn in your world.";
                    switch (interactables[i])
                    {

                        case "iscBrokenDrone1":
                            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                            break;
                        case "iscBrokenDrone2":
                            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                            break;
                        case "iscBrokenMegaDrone":
                            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                            break;
                        case "iscBrokenMissileDrone":
                            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                            break;
                        case "iscBrokenTurret1":
                            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                            break;
                        case "iscBarrel1":
                            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                            break;
                        case "iscEquipmentBarrel":
                            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                            break;
                        case "iscLockbox":
                            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                            break;
                        case "iscChest1":
                            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                            break;
                        case "iscChest1Stealthed":
                            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                            break;
                        case "iscChest2":
                            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                            break;
                        case "iscGoldChest":
                            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                            break;
                        case "iscTripleShop":
                            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                            break;
                        case "iscTripleShopLarge":
                            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                            break;
                        case "iscDuplicator":
                            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                            break;
                        case "iscDuplicatorLarge":
                            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                            break;
                        case "iscDuplicatorMilitary":
                            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                            break;
                        case "iscShrineCombat":
                            myRule.spritePath = "Textures/shrinesymbols/texshrinecombatsymbol";
                            break;
                        case "iscShrineBoss":
                            myRule.spritePath = "Textures/shrinesymbols/texshrinebosssymbole";
                            break;
                        case "iscShrineBlood":
                            myRule.spritePath = "Textures/shrinesymbols/texshrinebloodsymbol";
                            break;
                        case "iscShrineChance":
                            myRule.spritePath = "Textures/shrinesymbols/texshrinechancesymbol";
                            break;
                        case "iscShrineHealing":
                            myRule.spritePath = "Textures/shrinesymbols/texshrinehealingsymbol";
                            break;
                        case "iscShrineRestack":
                            myRule.spritePath = "Textures/shrinesymbols/texshrinerestacksymbol";
                            break;
                        case "iscShrineGoldshoresAccess":
                            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                            break;
                        case "iscRadarTower":
                            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                            break;
                        case "iscTeleporter":
                            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                            break;
                        case "iscShopPortal":
                            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                            break;
                        case "iscMSPortal":
                            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                            break;
                        case "iscGoldshoresPortal":
                            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                            break;
                        case "iscGoldshoresBeacon":
                            myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                            break;


                    }
                }
                addRule.Invoke(null, new object[] { GuaranteeRule });
            }


            bool gotCategory = false;
            DirectorCardCategorySelection myCategorySelection = new DirectorCardCategorySelection();
            Debug.Log("Ignore the above warning, this is intended.");


            On.RoR2.ClassicStageInfo.GenerateDirectorCardWeightedSelection += (orig, self, categorySelection) =>
            {
                if (!gotCategory)
                {
                    myCategorySelection = categorySelection;
                    gotCategory = true;
                }
                return orig(self,categorySelection);
            };

            On.RoR2.SceneDirector.PlaceTeleporter += (orig, self) =>
        {

            int currentCredits = (int)Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.SceneDirector"), "interactableCredit").GetValue(self);
            Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.SceneDirector"), "interactableCredit").SetValue(self, (int)(currentCredits * (float)Run.instance.ruleBook.GetRuleChoice(RuleCatalog.FindRuleDef("FloodWarning.InteractableScale")).extraData));

            foreach (DirectorCardCategorySelection.Category category in myCategorySelection.categories)
            {
                Debug.Log(category.name);

                float num = myCategorySelection.SumAllWeightsInCategory(category);
                foreach (DirectorCard directorCard in category.cards)
                {
                    for (int i = 0; i < interactables.Length; i++)
                    {
                        if (directorCard.spawnCard.name.ToString() == interactables[i])
                        {
                            directorCard.selectionWeight = (int)((float)Run.instance.ruleBook.GetRuleChoice(RuleCatalog.FindRuleDef("FloodWarning." + interactables[i] + "Chance")).extraData);
                        }
                    }

                }
            }


            orig(self);

            Xoroshiro128Plus rng = new Xoroshiro128Plus((ulong)Run.instance.stageRng.nextUint); ;
            DirectorCore myDirector = (DirectorCore)Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.SceneDirector"), "directorCore").GetValue(self);
            for (int i = 0; i < interactables.Length; i++)
            {
                for (int o = 0; o < (float)Run.instance.ruleBook.GetRuleChoice(RuleCatalog.FindRuleDef("FloodWarning." + interactables[i] + "Guaranteed")).extraData; o++)
                {
                    GameObject myGameObject = myDirector.TrySpawnObject(Resources.Load<SpawnCard>((string)("SpawnCards/InteractableSpawnCard/" + interactables[i])), new DirectorPlacementRule
                    {
                        placementMode = DirectorPlacementRule.PlacementMode.Random
                    }, rng);

                    if (myGameObject.GetComponent<PurchaseInteraction>())
                    {
                        myGameObject.GetComponent<PurchaseInteraction>().cost = Run.instance.GetDifficultyScaledCost(myGameObject.GetComponent<PurchaseInteraction>().cost);
                    }
                }
            }
        };
        return true;
        }
    }
}