using BepInEx;
using RoR2;
using UnityEngine;
using System.Collections.Generic;
using System;
using BepInEx.Configuration;
using System.Reflection;
using MonoMod.Cil;
using KinematicCharacterController;
using System.Linq;
using EntityStates;
using R2API;

namespace Flood_Warning
{
    [BepInDependency("com.bepis.r2api")]

    [BepInPlugin("com.PallesenProductions.FloodWarning", "Flood Warning", "2.0.0")]

    public class CharacterEdits : BaseUnityPlugin
    {

        public void Awake()//Code that runs when the game starts
        {
            SurvivorAPI.SurvivorCatalogReady += delegate (object s, EventArgs e)
            {
                SurvivorDef commando = new SurvivorDef
                {
                    bodyPrefab = BodyCatalog.FindBodyPrefab("CommandoBody"),
                    displayPrefab = Resources.Load<GameObject>("Prefabs/CharacterDisplays/CommandoDisplay"),
                    descriptionToken = "COMMANDO_DESCRIPTION",
                    primaryColor = new Color(0.929411769f, 0.5882353f, 0.07058824f),
                    unlockableName = ""
                };

                SurvivorDef huntress = new SurvivorDef
                {
                    bodyPrefab = BodyCatalog.FindBodyPrefab("HuntressBody"),
                    displayPrefab = Resources.Load<GameObject>("Prefabs/CharacterDisplays/HuntressDisplay"),
                    primaryColor = new Color(0.8352941f, 0.235294119f, 0.235294119f),
                    descriptionToken = "HUNTRESS_DESCRIPTION",
                    unlockableName = "Characters.Huntress"
                };
                SurvivorAPI.SurvivorDefinitions.Insert(1, huntress);

                SurvivorDef toolbot = new SurvivorDef
                {
                    bodyPrefab = BodyCatalog.FindBodyPrefab("ToolbotBody"),
                    displayPrefab = Resources.Load<GameObject>("Prefabs/CharacterDisplays/ToolbotDisplay"),
                    descriptionToken = "TOOLBOT_DESCRIPTION",
                    primaryColor = new Color(0.827451f, 0.768627465f, 0.3137255f),
                    unlockableName = "Characters.Toolbot"
                };
                SurvivorAPI.SurvivorDefinitions.Insert(2, toolbot);

                SurvivorDef engi = new SurvivorDef
                {
                    bodyPrefab = BodyCatalog.FindBodyPrefab("EngiBody"),
                    displayPrefab = Resources.Load<GameObject>("Prefabs/CharacterDisplays/EngiDisplay"),
                    descriptionToken = "ENGI_DESCRIPTION",
                    primaryColor = new Color(0.372549027f, 0.8862745f, 0.5254902f),
                    unlockableName = "Characters.Engineer"
                };
                SurvivorAPI.SurvivorDefinitions.Insert(3, engi);

                SurvivorDef mage = new SurvivorDef
                {
                    bodyPrefab = BodyCatalog.FindBodyPrefab("MageBody"),
                    displayPrefab = Resources.Load<GameObject>("Prefabs/CharacterDisplays/MageDisplay"),
                    descriptionToken = "MAGE_DESCRIPTION",
                    primaryColor = new Color(0.968627453f, 0.75686276f, 0.992156863f),
                    unlockableName = "Characters.Mage"
                };
              
                GameObject gameObject = BodyCatalog.FindBodyPrefab("MageBody");
                GenericSkill magePrimary = gameObject.GetComponent<SkillLocator>().primary;

                object box = magePrimary.activationState;
                var field = typeof(EntityStates.SerializableEntityStateType)?.GetField("_typeName", BindingFlags.NonPublic | BindingFlags.Instance);
                field?.SetValue(box, typeof(EntityStates.Commando.CommandoWeapon.Reload).Assembly.GetType("EntityStates.Mage.Weapon.Flamethrower")?.AssemblyQualifiedName);
                magePrimary.activationState = (EntityStates.SerializableEntityStateType)box;

                //magePrimary.activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Commando.CommandoWeapon.Reload).Assembly.GetType("EntityStates.Mage.Weapon.Flamethrower"));
                //magePrimary.icon = BodyCatalog.FindBodyPrefab("MageBody").GetComponent<SkillLocator>().special.icon;
                //magePrimary.skillNameToken = BodyCatalog.FindBodyPrefab("MageBody").GetComponent<SkillLocator>().special.skillNameToken;
                //magePrimary.skillDescriptionToken = BodyCatalog.FindBodyPrefab("MageBody").GetComponent<SkillLocator>().special.skillDescriptionToken;
                magePrimary.baseMaxStock = 1;
                magePrimary.mustKeyPress = true;
                magePrimary.beginSkillCooldownOnSkillEnd = false;
                magePrimary.canceledFromSprinting = false;
                magePrimary.noSprint = false;
                magePrimary.stockToConsume = 1;
                magePrimary.baseRechargeInterval = 2f;
                magePrimary.interruptPriority = InterruptPriority.Death;
                


                //mageSkills.passiveSkill.skillNameToken = "Trusty Jetpack";
                //mageSkills.passiveSkill.skillDescriptionToken = "Increased jump, movespeed and 1 extra starting jump";
                //mageSkills.passiveSkill.enabled = true;

                //mageBody.baseAcceleration *= 4;
                //mageBody.baseMoveSpeed *= 1.3f;
                //mageBody.baseJumpPower *= 1.3f;
                //mageBody.baseMaxShield = 50f;
                //mageBody.levelMaxShield = 1.2f;
                //mageBody.baseMaxHealth *= 0.5f;
                //mageBody.baseJumpCount = 2;
                //mageBody.baseDamage *= 0.8f;
                //mageBody.levelMaxHealth *= 0.75f;



                //mageSkills.utility.skillNameToken = "Portable Porter";
                //mageSkills.utility.skillDescriptionToken = "Teleport forwards a short distance";
                //mageSkills.utility.activationState.stateType = typeof(EntityStates.Huntress.BlinkState);//EntityStates.Commando.CommandoWeapon.Reload).Assembly.GetType("EntityStates.Huntress.BackflipState");
                //mageSkills.utility.baseRechargeInterval = 4f;

                //mageSkills.special.activationState.stateType = typeof(EntityStates.Commando.CommandoWeapon.Reload).Assembly.GetType("EntityStates.LemurianBruiserMonster.ChargeMegaFireball");
                //mageSkills.special.skillNameToken = "Fireball blaster";
                //mageSkills.special.skillDescriptionToken = "Fire a spread of 5 fireballs";
                //mageSkills.special.baseRechargeInterval = 10f;


                SurvivorAPI.SurvivorDefinitions.Insert(4, mage);

                SurvivorDef merc = new SurvivorDef
                {
                    bodyPrefab = BodyCatalog.FindBodyPrefab("MercBody"),
                    displayPrefab = Resources.Load<GameObject>("Prefabs/CharacterDisplays/MercDisplay"),
                    descriptionToken = "MERC_DESCRIPTION",
                    primaryColor = new Color(0.423529416f, 0.819607854f, 0.917647064f),
                    unlockableName = "Characters.Mercenary"
                };
                SurvivorAPI.SurvivorDefinitions.Insert(5, merc);

                SurvivorDef bandit = new SurvivorDef
                {
                    bodyPrefab = BodyCatalog.FindBodyPrefab("BanditBody"),
                    descriptionToken = "test",
                    displayPrefab = Resources.Load<GameObject>("Prefabs/Characters/BanditDisplay"),
                    primaryColor = new Color(0.8039216f, 0.482352942f, 0.843137264f),
                    unlockableName = ""
                };
               
                SurvivorAPI.SurvivorDefinitions.Insert(6, bandit);

            };

           
            On.RoR2.CharacterMaster.OnBodyStart += (orig, self, body) =>
            {
                orig(self, body);
                if (body.baseNameToken == "MAGE_BODY_NAME")
                {
                    body.inventory.SetEquipmentIndex(EquipmentIndex.AffixBlue);
                    body.inventory.GiveItem(ItemIndex.Clover, 8);
                }
            };

        }
    }
}
namespace EntityStates.Bandit
{
    public class Cannon : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            Chat.AddMessage("IT'S ALIVE");
        }

        public override void OnExit()
        {
            SmallHop(characterMotor, 100f);
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= this.totalDuration && isAuthority)
            {
                outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Any;
        }

        private float totalDuration = 10f;
    }
}
namespace EntityStates.CustomStates
{
    public class LightningExplosion : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            Chat.AddMessage("IT'S ALIVE");
        }

        public override void OnExit()
        {
            SmallHop(characterMotor, 1f);
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= this.totalDuration && isAuthority)
            {
                outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Any;
        }

        private float totalDuration = 10f;
    }
}