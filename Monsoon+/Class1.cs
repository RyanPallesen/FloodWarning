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

    [BepInPlugin("com.PallesenProductions.MonsoonPlus", "MonsoonPlus", "1.0.0")]

    public class MonsoonPlus : BaseUnityPlugin
    {

        public void Awake()//Code that runs when the game starts
        {

            Debug.Log("Called Awake");


            On.RoR2.DifficultyDef.ctor += (orig, self, scalingValue, nameToken, iconpath, descriptionToken, color) =>
            {
                Debug.Log("Called def");
                Debug.Log("Called def");
                Debug.Log("Called def");
                Debug.Log("Called def");
                Debug.Log("Called def");
                Debug.Log("Called def");
                Debug.Log("Called def");
                Debug.Log(self.scalingValue);


                if (self.scalingValue == 3f)//Monsoon
                {
                    Debug.Log("Called monsoon");

                    nameToken = "Flood Warning";
                    descriptionToken = "You're already dead. It's just a matter of time.";
                    orig(self, 6f, nameToken, iconpath, descriptionToken, color);
                }
            };

            //On.RoR2.DifficultyDef.GetIconSprite += (orig,self) =>
            //{

            //    Chat.AddMessage("Ran code");

            //    DifficultyDef[] difficultyDefs = (DifficultyDef[])Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("DifficultyCatalog"), "difficultyDefs").GetValue(self);

            //    difficultyDefs[0] = new DifficultyDef(1f, "DIFFICULTY_E_NAME", "Textures/DifficultyIcons/texDifficultyEasyIcon", "DIFFICULTY_EASY_DESCRIPTION", ColorCatalog.GetColor(ColorCatalog.ColorIndex.EasyDifficulty));
            //    difficultyDefs[1] = new DifficultyDef(2f, "DIFFICULTY_N_NAME", "Textures/DifficultyIcons/texDifficultyNormalIcon", "DIFFICULTY_NORMAL_DESCRIPTION", ColorCatalog.GetColor(ColorCatalog.ColorIndex.NormalDifficulty));
            //    difficultyDefs[2] = new DifficultyDef(3f, "DIFFICULTY_H_NAME", "Textures/DifficultyIcons/texDifficultyHardIcon", "DIFFICULTY_HARD_DESCRIPTION", ColorCatalog.GetColor(ColorCatalog.ColorIndex.HardDifficulty));

            //    Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("DifficultyCatalog"), "difficultyDefs").SetValue(self, difficultyDefs);

            //    return orig(self);
            //};



            On.RoR2.CombatDirector.FixedUpdate += (orig, self) =>
            {
                self.skipSpawnIfTooCheap = false;
                orig(self);
            };
        }
    }
}