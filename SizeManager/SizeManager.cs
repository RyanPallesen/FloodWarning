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

    [BepInPlugin("com.PallesenProductions.SizeManager", "SizeManager", "1.0.0")]

    public class SizeManager : BaseUnityPlugin
    {

        public void Awake()//Code that runs when the game starts
        {
            var addRule = typeof(RuleCatalog).GetMethod("AddRule", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { typeof(RuleDef) }, null);
            var addCategory = typeof(RuleCatalog).GetMethod("AddCategory", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { typeof(string), typeof(Color), typeof(string), typeof(Func<bool>) }, null);

            var myvar = addCategory.Invoke(null, new object[] { "Entity Sizes", new Color(94 / 255, 82 / 255, 30 / 255, byte.MaxValue), "", new Func<bool>(() => false) });

            RuleDef SpawnPlayerRule = new RuleDef("FloodWarning.PlayerSize", "Guaranteed");
            SpawnPlayerRule.defaultChoiceIndex = 8;
            for (float o = 1; o <= 50; o++)
            {
                float myNum = o / 10;
                RuleChoiceDef myRule = SpawnPlayerRule.AddChoice("0", myNum, false);
                myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                myRule.tooltipNameToken = "" + myNum + "x size for Players";
                myRule.tooltipBodyToken = "When a Player spawns in your world, they will be " + myNum + "x the size";
            }
            addRule.Invoke(null, new object[] { SpawnPlayerRule });
            RuleDef SpawnAllyRule = new RuleDef("FloodWarning.AllySize", "Guaranteed");
            SpawnAllyRule.defaultChoiceIndex = 6;
            for (float o = 1; o <= 50; o++)
            {
                float myNum = o / 10;
                RuleChoiceDef myRule = SpawnAllyRule.AddChoice("0", myNum, false);
                myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                myRule.tooltipNameToken = "" + myNum + "x size for Allies";
                myRule.tooltipBodyToken = "When an Ally (Drone, Turret) spawns in your world, they will be " + myNum + "x the size";
            }
            addRule.Invoke(null, new object[] { SpawnAllyRule });
            RuleDef SpawnNeutralRule = new RuleDef("FloodWarning.NeutralSize", "Guaranteed");
            SpawnNeutralRule.defaultChoiceIndex = 10;
            for (float o = 1; o <= 50; o++)
            {
                float myNum = o / 10;
                RuleChoiceDef myRule = SpawnNeutralRule.AddChoice("0", myNum, false);
                myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                myRule.tooltipNameToken = "" + myNum + "x size for Neutral Entities";
                myRule.tooltipBodyToken = "When a Neutral Entity (Newt) spawns in your world, they will be " + myNum + "x the size";
            }
            addRule.Invoke(null, new object[] { SpawnNeutralRule });

            RuleDef SpawnBossRule = new RuleDef("FloodWarning.BossSize", "Guaranteed");
            SpawnBossRule.defaultChoiceIndex = 11;
            for (float o = 1; o <= 50; o++)
            {
                float myNum = o / 10;
                RuleChoiceDef myRule = SpawnBossRule.AddChoice("0", myNum, false);
                myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                myRule.tooltipNameToken = "" + myNum + "x size for Boss";
                myRule.tooltipBodyToken = "When a Boss spawns in your world, they will be " + myNum + "x the size";
            }
            addRule.Invoke(null, new object[] { SpawnBossRule });

            RuleDef SpawnChampionRule = new RuleDef("FloodWarning.ChampionSize", "Guaranteed");
            SpawnChampionRule.defaultChoiceIndex = 15;
            for (float o = 1; o <= 50; o++)
            {
                float myNum = o / 10;
                RuleChoiceDef myRule = SpawnChampionRule.AddChoice("0", myNum, false);
                myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                myRule.tooltipNameToken = "" + myNum + "x size for Teleporter Bosses (Champions)";
                myRule.tooltipBodyToken = "When a Teleporter Boss (Champion) spawns in your world, they will be " + myNum + "x the size";
            }
            addRule.Invoke(null, new object[] { SpawnChampionRule });

            RuleDef SpawnEliteRule = new RuleDef("FloodWarning.EliteSize", "Guaranteed");
            SpawnEliteRule.defaultChoiceIndex = 13;
            for (float o = 1; o <= 50; o++)
            {
                float myNum = o / 10;
                RuleChoiceDef myRule = SpawnEliteRule.AddChoice("0", myNum, false);
                myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                myRule.tooltipNameToken = "" + myNum + "x size for Elites";
                myRule.tooltipBodyToken = "When an Elite spawns in your world, they will be " + myNum + "x the size";
            }
            addRule.Invoke(null, new object[] { SpawnEliteRule });

            RuleDef SpawnMonsterRule = new RuleDef("FloodWarning.MonsterSize", "Guaranteed");
            SpawnMonsterRule.defaultChoiceIndex = 9;
            for (float o = 1; o <= 50; o++)
            {
                float myNum = o / 10;
                RuleChoiceDef myRule = SpawnMonsterRule.AddChoice("0", myNum, false);
                myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                myRule.tooltipNameToken = "" + myNum + "x size for Non-Elite, Non-Boss Monsters";
                myRule.tooltipBodyToken = "Whenever ANY enemy spawns in your world, they will be " + myNum + "x the size";
            }
            addRule.Invoke(null, new object[] { SpawnMonsterRule });

            On.RoR2.CharacterMaster.OnBodyStart += (orig, self, body) =>
            {
                orig(self, body);
                var tf = body.modelLocator.modelTransform.localScale;
                float scaleFactor = 1f;
                if (body.isChampion)
                {
                    scaleFactor *= (float)Run.instance.ruleBook.GetRuleChoice(RuleCatalog.FindRuleDef("FloodWarning.ChampionSize")).extraData;
                }

                if (body.isBoss)
                {
                    scaleFactor *= (float)Run.instance.ruleBook.GetRuleChoice(RuleCatalog.FindRuleDef("FloodWarning.BossSize")).extraData;
                }

                if (body.isElite)
                {
                    scaleFactor *= (float)Run.instance.ruleBook.GetRuleChoice(RuleCatalog.FindRuleDef("FloodWarning.EliteSize")).extraData;

                }
                if (body.GetComponent<TeamComponent>().teamIndex == TeamIndex.Monster)
                {
                    scaleFactor *= (float)Run.instance.ruleBook.GetRuleChoice(RuleCatalog.FindRuleDef("FloodWarning.MonsterSize")).extraData;
                }
                if (body.GetComponent<TeamComponent>().teamIndex == TeamIndex.Player && !body.isPlayerControlled)
                {
                    scaleFactor *= (float)Run.instance.ruleBook.GetRuleChoice(RuleCatalog.FindRuleDef("FloodWarning.AllySize")).extraData;

                }
                if (body.GetComponent<TeamComponent>().teamIndex == TeamIndex.Neutral)
                {
                    scaleFactor *= (float)Run.instance.ruleBook.GetRuleChoice(RuleCatalog.FindRuleDef("FloodWarning.NeutralSize")).extraData;

                }

                if (body.isPlayerControlled)
                {
                    scaleFactor *= (float)Run.instance.ruleBook.GetRuleChoice(RuleCatalog.FindRuleDef("FloodWarning.PlayerSize")).extraData;

                }
                body.modelLocator.modelTransform.localScale = new Vector3(tf.x * scaleFactor, tf.y * scaleFactor, tf.z * scaleFactor);
            };
        }
    }
}