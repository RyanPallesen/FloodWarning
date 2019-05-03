using BepInEx;
using RoR2;
using UnityEngine;
using System.Collections.Generic;
using System;
using BepInEx.Configuration;
using System.Reflection;
using MonoMod.Cil;
using KinematicCharacterController;
using Flood_Warning;

namespace Flood_Warning
{
    [BepInDependency("com.bepis.r2api")]

    [BepInPlugin("com.PallesenProductions.EnemyEdits", "EnemyEdits", "1.0.0")]

    public class EnemyEdits : BaseUnityPlugin
    {

        public void Awake()//Code that runs when the game starts
        {
           
            R2API.ConfigController.addConfigCategory("Enemy Editor", new Color(0, 0, 0));
            R2API.ConfigController.insertRule("FloodWarning", "EliteCost");

            for (float o = 0; o <= 10; o++)
            {
                float myNum = o;
                R2API.ConfigController.insertRuleChoice<float>(myNum, "", "" + myNum + "x elite cost", "If the game is able to spawn " + myNum + " Non-Elite enemies this tick, it will try to spawn 1 elite instead", new Color(0, 0, 0), new Color(0, 0, 0));
            }
            R2API.ConfigController.pushRuleToGame();

            On.RoR2.CombatDirector.AttemptSpawnOnTarget += (orig, self, spawnTarget) =>
            {
                self.skipSpawnIfTooCheap = false;
                self.targetPlayers = false;
                return orig(self, spawnTarget);
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

            };

            On.RoR2.CombatDirector.Awake += (orig, self) =>
            {
                Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.CombatDirector"), "eliteMultiplierCost").SetValue(self, R2API.ConfigController.GetVar<float>("FloodWarning","EliteCost"));
                orig(self);
            };


            //On.RoR2.CombatDirector.SetNextSpawnAsBoss += (orig, self) =>
            //{
            //    Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.CombatDirector"), "currentActiveEliteIndex").SetValue(self, (EliteIndex)Run.instance.spawnRng.RangeInt(0, (int)EliteIndex.Count));
            //    orig(self);
            //};
            //On.RoR2.CombatDirector.OverrideCurrentMonsterCard += (orig, self,card) =>
            //{
            //    Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.CombatDirector"), "currentActiveEliteIndex").SetValue(self, (EliteIndex)Run.instance.spawnRng.RangeInt(0, (int)EliteIndex.Count));
            //    orig(self,card);
            //};
        }

    }
    
}