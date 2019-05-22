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
using UnityEngine.Networking;
using RoR2.CharacterAI;
using System.Linq;

namespace Flood_Warning
{
    [BepInDependency("com.bepis.r2api")]

    [BepInPlugin("com.PallesenProductions.ShadowClone", "ShadowClone", "1.0.1")]

    public class ShadowClone : BaseUnityPlugin
    {
        public void Awake()
        {

            SurvivorAPI.SurvivorCatalogReady += delegate (object s, EventArgs e)
            {
                
                BodyCatalog.FindBodyPrefab("MercBody").GetComponent<SkillLocator>().special.baseRechargeInterval = 20;
                BodyCatalog.FindBodyPrefab("MercBody").GetComponent<SkillLocator>().special.skillNameToken = "Shadow Clone";
                BodyCatalog.FindBodyPrefab("MercBody").GetComponent<SkillLocator>().special.skillDescriptionToken = "Summon two exact copies of yourself that <style=cIsUtility>inherit all your items</style> and live for 8 seconds ";

            };

            On.EntityStates.Merc.EvisDash.OnEnter += (orig, self) =>
            {
                self.outer.commonComponents.characterBody.AddTimedBuff(BuffIndex.Cloak, 2.5f);
                if (self.outer.commonComponents.characterBody.isPlayerControlled)
                {
                    var networkClient = NetworkClient.allClients.FirstOrDefault();
                    if (networkClient != null)
                        networkClient.RegisterHandlerSafe(HandleId, HandleDropItem);
                    var user = LocalUserManager.GetFirstLocalUser();

                    SendDropItem(user.cachedBody.gameObject, ItemIndex.Syringe);
                }
                else
                {
                    orig(self);
                }
                
            };
        }

            public const Int16 HandleId = 72;
        class SummonClone : MessageBase
        {
            public GameObject Player;
            public RoR2.ItemIndex ItemIndex;
            public override void Serialize(NetworkWriter writer)
            {
                writer.Write(Player);
                writer.Write((UInt16)ItemIndex);
            }

            public override void Deserialize(NetworkReader reader)
            {
                Player = reader.ReadGameObject();
                ItemIndex = (RoR2.ItemIndex)reader.ReadUInt16();
            }
        }
        static void SendDropItem(GameObject player, RoR2.ItemIndex itemIndex)
        {
            NetworkServer.SendToAll(HandleId, new SummonClone
            {
                Player = player,
                ItemIndex = itemIndex
            });

        }
        [RoR2.Networking.NetworkMessageHandler(msgType = HandleId, client = true)]
        static void HandleDropItem(NetworkMessage netMsg)
        {
            
                var dropItem = netMsg.ReadMessage<SummonClone>();
            var master = dropItem.Player.GetComponent<CharacterBody>().master;

            
                CharacterBody component = dropItem.Player.GetComponent<CharacterBody>();
            GameObject gameObject = MasterCatalog.FindMasterPrefab("MercMonsterMaster");
            GameObject bodyPrefab = BodyCatalog.FindBodyPrefab("MercBody");
            for (int i = 0; i < 2; i++)
            {

                if (master)
                {
                    
                    GameObject gameObject2 = Instantiate(gameObject, component.transform.position, component.transform.rotation);
                    CharacterMaster component2 = gameObject2.GetComponent<CharacterMaster>();
                    component2.gameObject.AddComponent<MasterSuicideOnTimer>().lifeTimer = 8f;

                    component2.teamIndex = TeamComponent.GetObjectTeam(component.gameObject);
                    AIOwnership component4 = gameObject2.GetComponent<AIOwnership>();
                    BaseAI component5 = gameObject2.GetComponent<BaseAI>();
                    if (component4)
                    {
                        component4.ownerMaster = master;

                    }
                    if (component5)
                    {
                        component5.leader.gameObject = master.gameObject;
                    }

                    Inventory component6 = gameObject2.GetComponent<Inventory>();
                    component6.CopyItemsFrom(master.inventory);
                    component6.ResetItem(ItemIndex.BeetleGland);
                    component6.ResetItem(ItemIndex.WardOnLevel);
                    component6.ResetItem(ItemIndex.AutoCastEquipment);
                    component6.ResetItem(ItemIndex.ExtraLife);

                    NetworkServer.Spawn(gameObject2);
                    CharacterBody body = component2.SpawnBody(bodyPrefab, component.transform.position, component.transform.rotation);
                }
                

            }


        }
    }

    
}