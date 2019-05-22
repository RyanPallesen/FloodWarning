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

    [BepInPlugin("com.PallesenProductions.HomingFireballs", "HomingFireballs", "1.0.1")]

    public class HomingFireballs : BaseUnityPlugin
    {
        public void Awake()
        {
           var fireBolt = typeof((EntityStates.Commando.CommandoWeapon.Reload).Assembly.GetType("EntityStates.Mage.Weapon.FireBolt")?.AssemblyQualifiedName)
            EntityStates.Mage.Weapon.FireBolt.projectilePrefab;
            EntityStates.Drone.DroneWeapon.FireMissileBarrage.projectilePrefab

            //On.EntityStates.Mage.Weapon.FireBolt.FireGauntlet += (orig, self) =>
            //{



            //};
        }
    }
}