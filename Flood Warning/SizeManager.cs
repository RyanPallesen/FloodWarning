using KinematicCharacterController;
using RoR2;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Flood_Warning
{
    class SizeManager
    {

        public static bool RunSizeManager()
        {
            var addRule = typeof(RuleCatalog).GetMethod("AddRule", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { typeof(RuleDef) }, null);
            var addCategory = typeof(RuleCatalog).GetMethod("AddCategory", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { typeof(string), typeof(Color), typeof(string), typeof(Func<bool>) }, null);



            addCategory.Invoke(null, new object[] { "Entity Sizes", new Color(94 / 255, 82 / 255, 30 / 255, byte.MaxValue), "", new Func<bool>(() => false) });

            RuleDef SpawnPlayerRule = new RuleDef("FloodWarning.PlayerSize", "Guaranteed");

            for (float o = 0; o <= 50; o++)
            {
                float myNum = o / 10;
                RuleChoiceDef myRule = SpawnPlayerRule.AddChoice("0", myNum, false);
                    if (myNum == 0.8f) { SpawnPlayerRule.MakeNewestChoiceDefault(); }
                    myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                    myRule.tooltipNameToken = "" + myNum + "x size for Players";
                    myRule.tooltipBodyToken = "When a Player spawns in your world, they will be " + myNum + "x the size";
                }
                addRule.Invoke(null, new object[] { SpawnPlayerRule });
            RuleDef SpawnAllyRule = new RuleDef("FloodWarning.AllySize", "Guaranteed");

            for (float o = 0; o <= 50; o++)
            {
                float myNum = o / 10;
                RuleChoiceDef myRule = SpawnAllyRule.AddChoice("0", myNum, false);
                if (myNum == 0.6f) { SpawnAllyRule.MakeNewestChoiceDefault(); }
                myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                myRule.tooltipNameToken = "" + myNum + "x size for Allies";
                myRule.tooltipBodyToken = "When an Ally (Drone, Turret) spawns in your world, they will be " + myNum + "x the size";
            }
            addRule.Invoke(null, new object[] { SpawnAllyRule });
            RuleDef SpawnNeutralRule = new RuleDef("FloodWarning.NeutralSize", "Guaranteed");

            for (float o = 0; o <= 50; o++)
            {
                float myNum = o / 10;
                RuleChoiceDef myRule = SpawnNeutralRule.AddChoice("0", myNum, false);
                if (myNum == 1f) { SpawnNeutralRule.MakeNewestChoiceDefault(); }
                myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                myRule.tooltipNameToken = "" + myNum + "x size for Neutral Entities";
                myRule.tooltipBodyToken = "When a Neutral Entity (Newt) spawns in your world, they will be " + myNum + "x the size";
            }
            addRule.Invoke(null, new object[] { SpawnNeutralRule });

            RuleDef SpawnBossRule = new RuleDef("FloodWarning.BossSize", "Guaranteed");
            for (float o = 0; o <= 50; o++)
            {
                float myNum = o / 10;
                RuleChoiceDef myRule = SpawnBossRule.AddChoice("0", myNum, false);
                if (myNum == 1.1f) { SpawnBossRule.MakeNewestChoiceDefault(); }
                myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                myRule.tooltipNameToken = "" + myNum + "x size for Boss";
                myRule.tooltipBodyToken = "When a Boss spawns in your world, they will be " + myNum + "x the size";
            }
            addRule.Invoke(null, new object[] { SpawnBossRule });

            RuleDef SpawnChampionRule = new RuleDef("FloodWarning.ChampionSize", "Guaranteed");
            for (float o = 0; o <= 50; o++)
            {
                float myNum = o / 10;
                RuleChoiceDef myRule = SpawnChampionRule.AddChoice("0", myNum, false);
                if (myNum == 1.2f) { SpawnChampionRule.MakeNewestChoiceDefault(); }
                myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                myRule.tooltipNameToken = "" + myNum + "x size for Teleporter Bosses (Champions)";
                myRule.tooltipBodyToken = "When a Teleporter Boss (Champion) spawns in your world, they will be " + myNum + "x the size";
            }
            addRule.Invoke(null, new object[] { SpawnChampionRule });

            RuleDef SpawnEliteRule = new RuleDef("FloodWarning.EliteSize", "Guaranteed");
            for (float o = 0; o <= 50; o++)
            {
                float myNum = o / 10;
                RuleChoiceDef myRule = SpawnEliteRule.AddChoice("0", myNum, false);
                if (myNum == 1.3f) { SpawnEliteRule.MakeNewestChoiceDefault(); }
                myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                myRule.tooltipNameToken = "" + myNum + "x size for Elites";
                myRule.tooltipBodyToken = "When an Elite spawns in your world, they will be " + myNum + "x the size";
            }
            addRule.Invoke(null, new object[] { SpawnEliteRule });

            RuleDef SpawnMonsterRule = new RuleDef("FloodWarning.MonsterSize", "Guaranteed");
            for (float o = 0; o <= 50; o++)
            {
                float myNum = o / 10;
                RuleChoiceDef myRule = SpawnMonsterRule.AddChoice("0", myNum, false);
                if (myNum == 1.1f) { SpawnMonsterRule.MakeNewestChoiceDefault(); }
                myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                myRule.tooltipNameToken = "" + myNum + "x size for Non-Elite, Non-Boss Monsters";
                myRule.tooltipBodyToken = "When an Elite spawns in your world, they will be " + myNum + "x the size";
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
                
                if(body.isElite)
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
            return true;
        }

    }
}
