using BepInEx;
using RoR2;
using UnityEngine;
using System.Collections.Generic;
using System;
using BepInEx.Configuration;
using System.Reflection;
using MonoMod.Cil;
using KinematicCharacterController;

namespace R2API
{
    [BepInDependency("com.bepis.r2api")]

    [BepInPlugin("com.PallesenProductions.ConfigController", "ConfigController", "1.0.0")]

    public class ConfigController : BaseUnityPlugin
    {
        static MethodInfo addRule = typeof(RuleCatalog).GetMethod("AddRule", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { typeof(RuleDef) }, null);
        static MethodInfo addCategory = typeof(RuleCatalog).GetMethod("AddCategory", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { typeof(string), typeof(Color), typeof(string), typeof(Func<bool>) }, null);

        static RuleDef currentRule = new RuleDef("FloodWarning.EliteCost", "Guaranteed");


        Color defaultColour = new Color(0, 0, 0);

        //Adds a category, like "Difficulty"
        public static void addConfigCategory(string categoryName, Color categoryColor)
        {
            addCategory.Invoke(null, new object[] { categoryName, categoryColor, "", new Func<bool>(() => false) });
        }
        //Adds a category with a tooltip, such as the 'We're working on artifacts' tip
        public static void addConfigTooltip(string categoryName, Color categoryColor ,string tooltip)
        {
            addCategory.Invoke(null, new object[] { categoryName, categoryColor, tooltip, new Func<bool>(() => false) });
        }
        //Adds a rule to the most recent category
        public static void insertRule(string modName, string ruleName )
        {
            currentRule = new RuleDef(modName + "." + ruleName, "ruleName");
        }
        //adds a choice to the most recent rule, used as insertRuleChoice<variable type>
        public static void insertRuleChoice<T>(T variable, string choiceName, string choiceTitle, string choiceDescription, Color titleColor, Color bodyColor , bool isDefault = false, string spritePath = "" )
        {
            RuleChoiceDef myRule = currentRule.AddChoice(choiceName, variable, false);
            myRule.tooltipNameToken = choiceTitle;
            myRule.tooltipBodyToken = choiceDescription;
            myRule.tooltipNameColor = titleColor;
            myRule.tooltipBodyColor = bodyColor;
            myRule.spritePath = spritePath;
            if (isDefault) { currentRule.MakeNewestChoiceDefault(); }

        }
        //Pushes the rule and it's choices to the game.
        public static void pushRuleToGame()
        {
            addRule.Invoke(null, new object[] { currentRule });
        }
        //Returns the chosen value of the given rule, cast to type T, used as GetVar<T>
        public static T GetVar<T>(string modName, string ruleName)
        {
            return (T)Run.instance.ruleBook.GetRuleChoice(RuleCatalog.FindRuleDef(modName + "." + ruleName)).extraData;
        }

        public void Awake()//Code that runs when the game starts
        {

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

            On.RoR2.UI.RuleCategoryController.Awake += (orig, self) =>
            {
                self.SetCollapsed(true);
                orig(self);
            };

            On.RoR2.UI.RuleChoiceController.OnClick += (orig, self) =>
            {
                RuleChoiceDef myChoiceDef = (RuleChoiceDef)Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.UI.RuleChoiceController"), "currentChoiceDef").GetValue(self);
                myChoiceDef.ruleDef.defaultChoiceIndex = myChoiceDef.localIndex;
                orig(self);
            };

          
            Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RuleCatalog"), "allRuleDefs").SetValue(null, allRuleDefs);
            Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RuleCatalog"), "allChoicesDefs").SetValue(null, allChoicesDefs);
            RuleCatalog.availability.MakeAvailable();
        }
    }
}