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

    [BepInPlugin("com.PallesenProductions.FloodWarning", "Flood Warning", "2.0.0")]

    public class Master : BaseUnityPlugin
    {
        public static bool shouldRunFloodWarning;
        public static bool shouldRunSizeChanger;

        public void Awake()//Code that runs when the game starts
        {
            RunConfig();

            if (shouldRunFloodWarning)
            {
                //if (shouldRunSizeChanger) { SizeChanger(); };
            }
        }

        public void Update()
        {

        }

        public void RunConfig()
        {
            shouldRunFloodWarning = Config.Wrap(
                "Settings",
                "myVariableMultiplier",
                "Custom value for myVariable in myMod",
                true).Value;

            shouldRunSizeChanger = Config.Wrap(
                "Settings",
                "myVariableMultiplier",
                "Custom value for myVariable in myMod",
                false).Value;
        }

        public void SizeChanger()
        {
            IL.RoR2.CharacterMaster.SpawnBody += il =>
            {
                var cursor = new ILCursor(il).Goto(0);
                cursor.GotoNext(MoveType.After, x => x.MatchCallvirt<GameObject>("GetComponent"));
                cursor.EmitDelegate<Func<GameObject, GameObject>>((gameObject) =>
                {
                    gameObject.transform.localScale = Vector3.one * 0.1f;
                    KinematicCharacterMotor component2 = gameObject.GetComponent<KinematicCharacterMotor>();
                    CapsuleCollider capsule = component2.Capsule;
                    component2.SetCapsuleDimensions(capsule.radius, capsule.height, 0.85f);
                    return gameObject;
                });
            };
        }
    }
}
