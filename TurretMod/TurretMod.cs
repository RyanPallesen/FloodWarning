using BepInEx;
using RoR2;
using UnityEngine;
using System.Collections.Generic;
using System;
using BepInEx.Configuration;
using BepInEx;
using RoR2;
using UnityEngine;
using System.Collections.Generic;
using System;
using BepInEx.Configuration;
using System.Reflection;
using RoR2.CharacterAI;

namespace Flood_Warning
{

    [BepInDependency("com.bepis.r2api")]

    [BepInPlugin("com.PallesenProductions.TurretMod", "Turretmod", "2.0.0")]

    public class TurretsShootRebar : BaseUnityPlugin
    {
        public static ConfigWrapper<bool> ConfigShouldRunMod { get; private set; }
        int currentRule;
        bool lastWasRebar = false;
        public void Awake()//Code that runs when the game starts
        {
            var addRule = typeof(RuleCatalog).GetMethod("AddRule", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { typeof(RuleDef) }, null);
            var addCategory = typeof(RuleCatalog).GetMethod("AddCategory", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { typeof(string), typeof(Color), typeof(string), typeof(Func<bool>) }, null);

            Color mycolor = new Color(0.3f, 0f, 0.5f);

            //Creates a header for the mod itself
            addCategory.Invoke(null, new object[] { "Ryan's Turrets", mycolor, "A Fun mod for custom turrets", new Func<bool>(() => false) });
            //Create a header for the category, such as 'Huntress edits'
            addCategory.Invoke(null, new object[] { "Turret Type Config", mycolor, "", new Func<bool>(() => false) });
            //Creates a rule and an accessible variable
            RuleDef ruleDef3 = new RuleDef("RyanTurrets.TurretMode", "TurretModes");
            //Adds Choice name, variable and "hidden", (set this to false if you want it to show up), then adds the string to appear on hover.
            ruleDef3.AddChoice("1", 1, false).tooltipBodyToken = "Makes all turrets default";
            ruleDef3.AddChoice("2", 2, false).tooltipBodyToken = "Makes all turrets shoot rebar";
            ruleDef3.AddChoice("3", 3, false).tooltipBodyToken = "Makes all turrets machine guns";
            ruleDef3.AddChoice("4", 4, false).tooltipBodyToken = "Switches between Rebar and Machine gun turrets";
            ruleDef3.MakeNewestChoiceDefault();
            //Adds the choices to the catalog
            addRule.Invoke(null, new object[] { ruleDef3 });

            //This has to run once after all the custom rules are entered, in order to reset the game into knowing what rules exist.
            List<RuleDef> allRuleDefs = (List<RuleDef>)Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RuleCatalog"), "allRuleDefs").GetValue(null);
            List<RuleChoiceDef> allChoicesDefs = (List<RuleChoiceDef>)Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RuleCatalog"), "allChoicesDefs").GetValue(null);
            for (int k = 0; k < allRuleDefs.Count; k++)
            {
                RuleDef ruleDef4 = allRuleDefs[k];
                ruleDef4.globalIndex = k;

                for (int j = 0; j < ruleDef4.choices.Count; j++)
                {
                    RuleChoiceDef ruleChoiceDef6 = ruleDef4.choices[j];
                    ruleChoiceDef6.localIndex = j;
                    ruleChoiceDef6.globalIndex = allChoicesDefs.Count;
                    ruleChoiceDef6.availableInMultiPlayer = true;
                    ruleChoiceDef6.excludeByDefault = false;
                    allChoicesDefs.Add(ruleChoiceDef6);
                }
            }
            Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RuleCatalog"), "allRuleDefs").SetValue(null, allRuleDefs);
            Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RuleCatalog"), "allChoicesDefs").SetValue(null, allChoicesDefs);
            RuleCatalog.availability.MakeAvailable();
            bool didReflect = false;

            currentRule = (int)Run.instance.ruleBook.GetRuleChoice(RuleCatalog.FindRuleDef("RyanTurrets.TurretMode")).extraData;

            On.EntityStates.Drone.DroneWeapon.FireMegaTurret.OnEnter += (orig, self) =>
            {
                if (!didReflect)
                {
                    Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("EntityStates.Drone.DroneWeapon.FireMegaTurret"), "force").SetValue(self, 0.1f);
                    didReflect = true;
                }
                orig(self);
            };
            bool gotDefault = false;
            float defaultspeed = 0f;
            float defaultleveldamage = 0f;
            float defaultdamage = 0f;
            On.RoR2.CharacterBody.HandleConstructTurret += (orig, netMsg) =>
            {
                if (!gotDefault)
                {
                    defaultspeed = BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().baseAttackSpeed;
                    defaultdamage = BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().baseDamage;
                    defaultleveldamage = BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().levelDamage;
                    gotDefault = true;
                }

                if (currentRule == 1)
                {
                    BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<SkillLocator>().primary.activationState.stateType = typeof(EntityStates.Commando.CommandoWeapon.Reload).Assembly.GetType("EntityStates.EngiTurret.EngiTurretWeapon.FireGauss");
                    BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().baseAttackSpeed = defaultspeed;
                    BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().baseDamage = defaultdamage;
                    BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().levelDamage = defaultleveldamage;
                    BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().baseNameToken = "Weak Boi Default";

                    for (int i = 0; i < MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>().Length; i++)
                    {
                        if (MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>()[i].customName == "FireAtEnemy")
                        {
                            MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>()[i].maxDistance = 60;
                            MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>()[i].selectionRequiresTargetLoS = false;
                            MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>()[i].activationRequiresTargetLoS = true;
                        }
                    }

                }
                else if (currentRule == 2)
                {
                    BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<SkillLocator>().primary.activationState.stateType = typeof(EntityStates.Toolbot.FireSpear);

                    BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().baseAttackSpeed = 0.5f;

                    BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().baseDamage = 13f;

                    BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().levelDamage = 2.12f;
                    BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().baseNameToken = "Portable Railgun";
                    for (int i = 0; i < MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>().Length; i++)
                    {
                        if (MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>()[i].customName == "FireAtEnemy")
                        {
                            MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>()[i].maxDistance = 1024;
                            MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>()[i].selectionRequiresTargetLoS = false;
                            MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>()[i].activationRequiresTargetLoS = false;
                        }
                    }
                }
                else if (currentRule == 3)
                {
                    BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<SkillLocator>().primary.activationState.stateType = typeof(EntityStates.Commando.CommandoWeapon.Reload).Assembly.GetType("EntityStates.Drone.DroneWeapon.FireMegaTurret");
                    BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().baseAttackSpeed = 1f;
                    BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().baseDamage = 2f;
                    BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().levelDamage = 0.4f;
                    BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().baseNameToken = ".50 Cal Maxim";
                    for (int i = 0; i < MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>().Length; i++)
                    {
                        if (MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>()[i].customName == "FireAtEnemy")
                        {
                            MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>()[i].maxDistance = 100;
                            MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>()[i].selectionRequiresTargetLoS = false;
                            MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>()[i].activationRequiresTargetLoS = false;
                        }
                    }

                }
                else
                {
                    if (lastWasRebar == false)
                    {
                        Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                        {
                            baseToken = "Next Turret will be: [Machine Gun]"
                        });
                        BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<SkillLocator>().primary.activationState.stateType = typeof(EntityStates.Toolbot.FireSpear);
                        BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().baseAttackSpeed = 0.5f;
                        BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().baseDamage = 13f;
                        BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().levelDamage = 2.12f;
                        BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().baseNameToken = "Portable Railgun";
                        for (int i = 0; i < MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>().Length; i++)
                        {
                            if (MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>()[i].customName == "FireAtEnemy")
                            {
                                MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>()[i].maxDistance = 1024;
                                MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>()[i].selectionRequiresTargetLoS = false;
                                MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>()[i].activationRequiresTargetLoS = false;
                            }
                        }
                        lastWasRebar = true;
                    }
                    else
                    {

                        Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                        {
                            baseToken = "Next Turret will be: [Railgun]"
                        });

                        BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<SkillLocator>().primary.activationState.stateType = typeof(EntityStates.Commando.CommandoWeapon.Reload).Assembly.GetType("EntityStates.Drone.DroneWeapon.FireMegaTurret");
                        BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().baseAttackSpeed = 1f;
                        BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().baseDamage = 2f;
                        BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().levelDamage = 0.4f;
                        BodyCatalog.FindBodyPrefab("EngiTurretBody").GetComponent<CharacterBody>().baseNameToken = ".50 Cal Maxim";
                        for (int i = 0; i < MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>().Length; i++)
                        {
                            if (MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>()[i].customName == "FireAtEnemy")
                            {
                                MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>()[i].maxDistance = 100;
                                MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>()[i].selectionRequiresTargetLoS = false;
                                MasterCatalog.FindMasterPrefab("EngiTurretMaster").GetComponents<AISkillDriver>()[i].activationRequiresTargetLoS = false;
                            }
                        }
                        lastWasRebar = false;
                    }
                }

                orig(netMsg);
            };
        }

        public void Update()//Code that runs once a frame
        {

            if (Input.GetKeyDown(KeyCode.F1))//Code that runs once you have pressed 'F1'
            {
                Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                {
                    baseToken = "Changing Turret Placement Rule"
                });

                switch (currentRule)
                {
                    case 1:
                        currentRule = 2;
                        Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                        {
                            baseToken = "Turret Placement Rule is now: [Railgun Only]"
                        });
                        break;
                    case 2:
                        currentRule = 3;
                        Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                        {
                            baseToken = "Turret Placement Rule is now: [Railgun And Machinegun]"
                        });
                        break;
                    case 3:
                        currentRule = 1;
                        Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                        {
                            baseToken = "Turret Placement Rule is now: [Machinegun Only]"
                        });
                        break;
                }
            }

            if (Input.GetKeyDown(KeyCode.F2))//Code that runs once you have pressed 'F1'
            {

                if (currentRule == 3)
                {
                    switch (lastWasRebar)
                    {
                        case false:
                            lastWasRebar = true;
                            Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                            {
                                baseToken = "Next Turret will be: [Machine Gun]"
                            });
                            break;
                        case true:
                            lastWasRebar = false;
                            Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                            {
                                baseToken = "Next Turret will be: [Railgun]"
                            });
                            break;
                    }
                }
            }
            
        }
    }
}
