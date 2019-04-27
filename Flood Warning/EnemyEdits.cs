using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using BepInEx;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace Flood_Warning
{
    class EnemyEdits : BaseUnityPlugin
    {
        public static bool RunEnemyEdits()
        {
            var addRule = typeof(RuleCatalog).GetMethod("AddRule", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { typeof(RuleDef) }, null);
            var addCategory = typeof(RuleCatalog).GetMethod("AddCategory", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { typeof(string), typeof(Color), typeof(string), typeof(Func<bool>) }, null);



            addCategory.Invoke(null, new object[] { "Enemy Editor", new Color(94 / 255, 82 / 255, 30 / 255, byte.MaxValue), "", new Func<bool>(() => false) });

            RuleDef eliteSpawnCost = new RuleDef("FloodWarning.EliteCost", "Guaranteed");

            for (float o = 0; o <= 10; o++)
            {
                float myNum = o;
                RuleChoiceDef myRule = eliteSpawnCost.AddChoice("0", myNum, false);
                myRule.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
                myRule.tooltipNameToken = "" + myNum + "x elite cost";
                myRule.tooltipBodyToken = "Spawning an elite costs the system " + myNum + "x more than a normal enemy";
                if (myNum == 5) { eliteSpawnCost.MakeNewestChoiceDefault(); }

            }
            addRule.Invoke(null, new object[] { eliteSpawnCost });

            //On.RoR2.CombatDirector.AttemptSpawnOnTarget += (orig, self, spawnTarget) =>
            //{
            //    if (orig(self, spawnTarget) == false && Run.instance.difficultyCoefficient > 15f)
            //    {
            //    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(self.lastAttemptedMonsterCard.spawnCard.prefab, self.transform.position, self.transform.rotation);
            //        CharacterMaster component2 = gameObject.GetComponent<CharacterMaster>();
            //        component2.teamIndex = TeamIndex.Monster;
            //        Inventory component3 = gameObject.GetComponent<Inventory>();
            //        NetworkServer.Spawn(gameObject);
            //        component2.SpawnBody(component2.bodyPrefab, self.transform.position + Vector3.up * 0.8f, self.transform.rotation);
            //        for (int i = 0; i < Run.instance.difficultyCoefficient - 15f;i++)
            //        {
            //            component3.GiveItem(Run.instance.availableTier1DropList[Run.instance.spawnRng.RangeInt(0, Run.instance.availableTier1DropList.Count)].itemIndex, 1);
            //        }
            //    }
            //    return true;

            //};

            On.RoR2.CombatDirector.Awake += (orig, self) =>
            {
                Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.CombatDirector"), "eliteMultiplierCost").SetValue(self, (float)(Run.instance.ruleBook.GetRuleChoice(RuleCatalog.FindRuleDef("FloodWarning.EliteCost")).extraData));
                orig(self);
            };


            On.RoR2.CombatDirector.SetNextSpawnAsBoss += (orig, self) =>
            {
                Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.CombatDirector"), "currentActiveEliteIndex").SetValue(self, (EliteIndex)Run.instance.spawnRng.RangeInt(0, (int)EliteIndex.Count));
                orig(self);
            };
            On.RoR2.CombatDirector.OverrideCurrentMonsterCard += (orig, self,card) =>
            {
                Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.CombatDirector"), "currentActiveEliteIndex").SetValue(self, (EliteIndex)Run.instance.spawnRng.RangeInt(0, (int)EliteIndex.Count));
                orig(self,card);
            };
            return true;
        }

    }
}
