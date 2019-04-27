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

    public class GlobalVars : BaseUnityPlugin
    {

        public static bool shouldRunFloodWarning;
        public static bool shouldRunEWI;
        public static bool shouldRunEDI;
        public static bool shouldRunSizeChanger;

        public void RunConfig()
        {
            shouldRunFloodWarning = Config.Wrap(
                "Settings",
                "myVariableMultiplier",
                "Custom value for myVariable in myMod",
                true).Value;
            shouldRunEWI = Config.Wrap(
                "Settings",
                "shouldRunEWI",
                "Whether or not Enemies With Items should run",
                true).Value;
            shouldRunEDI = Config.Wrap(
                "Settings",
                "shouldRunEDI",
                "Custom value for myVariable in myMod",
                true).Value;
            shouldRunSizeChanger = Config.Wrap(
                "Settings",
                "myVariableMultiplier",
                "Custom value for myVariable in myMod",
                true).Value;
        }

    }
}
