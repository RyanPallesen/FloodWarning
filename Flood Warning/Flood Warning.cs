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

        public void Awake()//Code that runs when the game starts
        {
          


            if (BetterInteractables.RunBetterInteractables()) { Chat.AddMessage("[FW] Better Interactables Loaded"); }
            if (InteractableManager.RunInteractableManager()) { Chat.AddMessage("[FW] Interactable Manager Loaded"); }
            if (SizeManager.RunSizeManager()) { Chat.AddMessage("[FW] Size Manager Loaded"); }
            if (CharacterEdits.RunCharacterEdits()) { Chat.AddMessage("[FW] Character Edits Loaded"); }
            if (EnemyEdits.RunEnemyEdits()) { Chat.AddMessage("[FW] Enemy Edits Loaded"); }
            if (AITeammates.RunAITeammates()) { Chat.AddMessage("[FW] Teammate editor loaded"); }

            List<RuleDef> allRuleDefs = (List<RuleDef>)Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RuleCatalog"), "allRuleDefs").GetValue(null);
            List<RuleChoiceDef> allChoicesDefs = (List<RuleChoiceDef>)Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RuleCatalog"), "allChoicesDefs").GetValue(null);

            for (int k = 0; k < allRuleDefs.Count; k++)
            {
                RuleDef ruleDef = allRuleDefs[k];
                ruleDef.globalIndex = k;
                for (int j = 0; j < ruleDef.choices.Count; j++)
                {
                    RuleChoiceDef ruleChoiceDef6 = ruleDef.choices[j];
                    ruleChoiceDef6.localIndex = j;
                    ruleChoiceDef6.globalIndex = allChoicesDefs.Count;
                    allChoicesDefs.Add(ruleChoiceDef6);
                }
            }

            Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RuleCatalog"), "allRuleDefs").SetValue(null, allRuleDefs);
            Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RuleCatalog"), "allChoicesDefs").SetValue(null, allChoicesDefs);
            RuleCatalog.availability.MakeAvailable();


        }

        public void Update()
        {

        }

        
    }
}
