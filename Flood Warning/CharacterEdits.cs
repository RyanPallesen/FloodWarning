using BepInEx;
using KinematicCharacterController;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Flood_Warning
{
    class CharacterEdits : BaseUnityPlugin
    {

        public static bool RunCharacterEdits()
        {

            On.RoR2.SurvivorCatalog.Init += (orig) =>
            {
                orig();
                CharacterBody commandoBody = BodyCatalog.FindBodyPrefab("CommandoBody").GetComponent<CharacterBody>();
                SkillLocator commandoSkills = BodyCatalog.FindBodyPrefab("CommandoBody").GetComponent<SkillLocator>();

                CharacterBody huntressBody = BodyCatalog.FindBodyPrefab("HuntressBody").GetComponent<CharacterBody>();
                SkillLocator huntressSkills = BodyCatalog.FindBodyPrefab("HuntressBody").GetComponent<SkillLocator>();

                CharacterBody engiBody = BodyCatalog.FindBodyPrefab("EngiBody").GetComponent<CharacterBody>();
                SkillLocator engiSkills = BodyCatalog.FindBodyPrefab("EngiBody").GetComponent<SkillLocator>();

                CharacterBody toolbotBody = BodyCatalog.FindBodyPrefab("ToolbotBody").GetComponent<CharacterBody>();
                SkillLocator toolbotSkills = BodyCatalog.FindBodyPrefab("ToolbotBody").GetComponent<SkillLocator>();

                CharacterBody mageBody = BodyCatalog.FindBodyPrefab("MageBody").GetComponent<CharacterBody>();
                SkillLocator mageSkills = BodyCatalog.FindBodyPrefab("MageBody").GetComponent<SkillLocator>();

                CharacterBody mercBody = BodyCatalog.FindBodyPrefab("MercBody").GetComponent<CharacterBody>();
                SkillLocator mercSkills = BodyCatalog.FindBodyPrefab("MercBody").GetComponent<SkillLocator>();


                mageSkills.passiveSkill.skillNameToken = "Trusty Jetpack";
                mageSkills.passiveSkill.skillDescriptionToken = "Increased jump, movespeed and 1 extra starting jump";
                mageSkills.passiveSkill.enabled = true;

                mageBody.baseAcceleration *= 4;
                mageBody.baseMoveSpeed *= 1.3f;
                mageBody.baseJumpPower *= 1.3f;
                mageBody.baseMaxShield = 50f;
                mageBody.levelMaxShield = 1.2f;
                mageBody.baseMaxHealth *= 0.5f;
                mageBody.baseJumpCount = 2;
                mageBody.baseDamage *= 0.8f;
                mageBody.levelMaxHealth *= 0.75f;

                mageSkills.primary.icon = mageSkills.special.icon;
                mageSkills.primary.skillNameToken = mageSkills.special.skillNameToken;
                mageSkills.primary.skillDescriptionToken = mageSkills.special.skillDescriptionToken;
                mageSkills.primary.activationState = mageSkills.special.activationState;
                mageSkills.primary.baseMaxStock =  1;
                mageSkills.primary.beginSkillCooldownOnSkillEnd = true;
                mageSkills.primary.canceledFromSprinting = false;
                mageSkills.primary.noSprint = false;
                mageSkills.primary.stockToConsume = 1;
                mageSkills.primary.baseRechargeInterval = 2f;

                mageSkills.utility.skillNameToken = "Portable Porter";
                mageSkills.utility.skillDescriptionToken = "Teleport forwards a short distance";
                mageSkills.utility.activationState.stateType = typeof(EntityStates.Huntress.BlinkState);//EntityStates.Commando.CommandoWeapon.Reload).Assembly.GetType("EntityStates.Huntress.BackflipState");
                mageSkills.utility.baseRechargeInterval = 4f;

                mageSkills.special.activationState.stateType = typeof(EntityStates.Commando.CommandoWeapon.Reload).Assembly.GetType("EntityStates.LemurianBruiserMonster.ChargeMegaFireball");
                mageSkills.special.skillNameToken = "Fireball blaster";
                mageSkills.special.skillDescriptionToken = "Fire a spread of 5 fireballs";
                mageSkills.special.baseRechargeInterval = 10f;

                toolbotBody.baseAcceleration *= 2;

                commandoSkills.passiveSkill.skillNameToken = "Hardened Body";
                commandoSkills.passiveSkill.skillDescriptionToken = "Increased Passive Regeneration";
                commandoSkills.passiveSkill.enabled = true;
                commandoBody.baseRegen *= 2;


                engiSkills.passiveSkill.skillNameToken = "Auto-Targetting";
                engiSkills.passiveSkill.skillDescriptionToken = "Increased base crit rate";
                engiSkills.passiveSkill.enabled = true;
                engiBody.baseCrit = 30f;

                engiSkills.primary.noSprint = true;
                engiSkills.primary.canceledFromSprinting = true;

                engiSkills.secondary.mustKeyPress = false;
                engiSkills.secondary.shootDelay = 2f;
                engiSkills.secondary.baseRechargeInterval /= 1.5f;
                engiSkills.secondary.noSprint = false;
                engiSkills.secondary.canceledFromSprinting = false;
                engiSkills.secondary.isCombatSkill = false;

            };

            On.RoR2.CharacterMaster.OnBodyStart += (orig, self, body) =>
            {
                orig(self, body);
                if (body.baseNameToken == "MAGE_BODY_NAME")
                {
                    body.inventory.SetEquipmentIndex(EquipmentIndex.AffixRed);
                }
                if (body.baseNameToken == "ENGINEER_BODY_NAME")
                {
                    body.inventory.SetEquipmentIndex(EquipmentIndex.DroneBackup);
                }
            };
         

            On.EntityStates.Huntress.BackflipState.OnEnter += (orig, self) =>
            {
                Util.PlaySound(EntityStates.Huntress.BeginArrowRain.blinkSoundString, self.outer.gameObject);
                orig(self);
            };

                IL.RoR2.CharacterMaster.AddDeployable += il =>
                {
                    var c = new ILCursor(il).Goto(0);

                    c.GotoNext(x => x.MatchStloc(1) && x.Next.MatchLdarg(0));
                    c.Index += 1;
                    c.Next.OpCode = OpCodes.Nop;
                    c.Index += 1;
                    c.Emit(OpCodes.Ldloc_1);
                    c.Emit(OpCodes.Ldarg_0);
                    c.Emit(OpCodes.Ldarg_2);

                    c.EmitDelegate<Func<int, CharacterMaster, DeployableSlot, int>>((num2, self, slot) =>
                    {
                        
                            switch (slot)
                            {
                                case DeployableSlot.EngiMine:
                                    num2 += (self.inventory.GetItemCount(ItemIndex.SecondarySkillMagazine) * 5);
                                    break;
                                case DeployableSlot.EngiTurret:
                                    num2 = 2;
                                    break;
                                case DeployableSlot.BeetleGuardAlly:
                                    num2 = self.inventory.GetItemCount(ItemIndex.BeetleGland);
                                    break;
                                case DeployableSlot.EngiBubbleShield:
                                    num2 += self.inventory.GetItemCount(ItemIndex.UtilitySkillMagazine);
                                    break;
                            }
                        
                        return num2;
                    });

                    c.Emit(OpCodes.Stloc_1);
                    c.Emit(OpCodes.Ldarg_0);

                };

            return true;
        }
    }
}
