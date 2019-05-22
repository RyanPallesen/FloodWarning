using BepInEx;
using RoR2;
using UnityEngine;
using System.Collections.Generic;
using System;
using BepInEx.Configuration;
using System.Reflection;
using KinematicCharacterController;
using Flood_Warning;
using RoR2.Projectile;
using R2API;

namespace Flood_Warning
{
    [BepInDependency("com.bepis.r2api")]

    [BepInPlugin("com.PallesenProductions.MineEdits", "MineEdits", "1.0.1")]

    public class MineEdits : BaseUnityPlugin
    {
        public void Awake()
        {
            On.RoR2.EngiMineController.Start += (orig,self) =>
            {
                self.explosionRadius = 25;//4.1x size
                self.GetComponent<ProjectileDamage>().damage *= 5;
                self.primingDelay = 1f;
                orig(self);
            };

            On.RoR2.EngiMineController.PrepForExplosion += (orig, self) =>
            {
                self.detatchForce *= 2;
                
                self.proximityDetector.gameObject.transform.localScale = Vector3.one * 16f;
                orig(self);
            };


            SurvivorAPI.SurvivorCatalogReady += delegate (object s, EventArgs e)
            {

                BodyCatalog.FindBodyPrefab("EngiBody").GetComponent<SkillLocator>().secondary.baseMaxStock = 2;
                BodyCatalog.FindBodyPrefab("EngiBody").GetComponent<SkillLocator>().secondary.baseRechargeInterval *= 4;
                BodyCatalog.FindBodyPrefab("EngiBody").GetComponent<SkillLocator>().secondary.skillDescriptionToken = "Place a mine that deals <style=cIsDamage>1500% damage</style> when an enemy walks nearby. Can place up to 10.";

            };



        }

    }
}