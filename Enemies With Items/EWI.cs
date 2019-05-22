using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using System.Collections;
using UnityEngine;
using BepInEx;
using System.Collections.Generic;

namespace Enemies_With_Items
{
    
    enum itemRule
    {
RandomByDifficulty,
RandomByAverage,
RandomByTotal,
RatioOneToOne,
RatioHalf,
RatioThird,
RatioFourth,
LooseCopy,
ExactCopy,
None
    }

    [BepInDependency("com.bepis.r2api")]

    [BepInPlugin("com.PallesenProductions.EnemiesWithItems", "EnemiesWithItems", "2.0.0")]

    class EWI : BaseUnityPlugin
    {




        public void Awake()
        {
            R2API.ConfigController.addConfigCategory("Enemy Item Type Editor", new Color(0, 0, 0));
            R2API.ConfigController.insertRule("EWI", "EnemyItemType");


            R2API.ConfigController.insertRuleChoice<itemRule>(itemRule.RandomByDifficulty,"DifficultyScaled", "Difficulty Scaled Items","Enemies will gain items based on the game's current difficulty level", new Color(0, 0, 0), new Color(0, 0, 0));
            R2API.ConfigController.insertRuleChoice<itemRule>(itemRule.RandomByAverage, "DifficultyScaled", "Player-based (Average)", "Enemies will have items equal to the average amount of items between players", new Color(0, 0, 0), new Color(0, 0, 0));
            R2API.ConfigController.insertRuleChoice<itemRule>(itemRule.RandomByTotal, "DifficultyScaled", "Player-based (Total)", "Enemies will have items equal to the total amount of items between players", new Color(0, 0, 0), new Color(0, 0, 0));
            R2API.ConfigController.insertRuleChoice<itemRule>(itemRule.RatioOneToOne, "DifficultyScaled", "Tier-based (1:1)", "For each item you have of a specific tier, the enemy will have 1", new Color(0, 0, 0), new Color(0, 0, 0));
            R2API.ConfigController.insertRuleChoice<itemRule>(itemRule.RatioHalf, "DifficultyScaled", "Tier-based (1:2)", "For every 2 items you have of a specific tier, the enemy will have 1", new Color(0, 0, 0), new Color(0, 0, 0));
            R2API.ConfigController.insertRuleChoice<itemRule>(itemRule.RatioThird, "DifficultyScaled", "Tier-based (1:3)", "For every 3 items you have of a specific tier, the enemy will have 1", new Color(0, 0, 0), new Color(0, 0, 0));
            R2API.ConfigController.insertRuleChoice<itemRule>(itemRule.RatioFourth, "DifficultyScaled", "Tier-based (1:4)", "For every 4 items you have of a specific tier, the enemy will have 1", new Color(0, 0, 0), new Color(0, 0, 0));
            R2API.ConfigController.insertRuleChoice<itemRule>(itemRule.LooseCopy, "DifficultyScaled", "Loose Copy", "Enemies have a copy of a random player's items, Minus equipment", new Color(0, 0, 0), new Color(0, 0, 0));
            R2API.ConfigController.insertRuleChoice<itemRule>(itemRule.ExactCopy, "DifficultyScaled", "Exact Copy", "Enemies have an exact copy of a random player's items", new Color(0, 0, 0), new Color(0, 0, 0));
            R2API.ConfigController.insertRuleChoice<itemRule>(itemRule.None, "DifficultyScaled", "None", "Enemies will not have items", new Color(0, 0, 0), new Color(0, 0, 0));

            R2API.ConfigController.pushRuleToGame();


            IL.RoR2.CombatDirector.AttemptSpawnOnTarget += il =>
            {

                var cursor = il.At(0);

                cursor.GotoNext(x => x.MatchStloc(8));
                cursor.Emit(OpCodes.Ldarg_0);
                cursor.EmitDelegate<Func<CharacterMaster, CombatDirector, CharacterMaster>>((component, director) =>
                {
                StartCoroutine(giveItems(component, R2API.ConfigController.GetVar<itemRule>("EWI", "EnemyItemType")));

                    return component;
                });

            };
        }

        public static ItemIndex GenerateItem(float WhiteWeight =0.8f, float GreenWeight=0.2f, float RedWeight =0.05f, float BossWeight = 0f, float LunarWeight = 0f)
        {
            System.Random random = new System.Random();
            WeightedSelection<List<PickupIndex>> weightedSelection = new WeightedSelection<List<PickupIndex>>(8);

            weightedSelection.AddChoice(Run.instance.availableTier1DropList, WhiteWeight);
            weightedSelection.AddChoice(Run.instance.availableTier2DropList, GreenWeight);
            weightedSelection.AddChoice(Run.instance.availableTier3DropList, RedWeight);
            weightedSelection.AddChoice(Run.instance.availableLunarDropList, LunarWeight);

            List<PickupIndex> dropList = weightedSelection.Evaluate(Run.instance.spawnRng.nextNormalizedFloat);
            PickupIndex itemToGive = dropList[Run.instance.spawnRng.RangeInt(0, dropList.Count)];

            return itemToGive.itemIndex;
        }

        public static IEnumerator giveItems(CharacterMaster summon, itemRule GiveRule)
        {
            int itemsToGive = 0;
            float itemRatio = 0f;
            float tier1Count = 0f;
            float tier2Count = 0f;
            float tier3Count = 0f;
            float tier4Count = 0f;

            yield return new WaitForSeconds(0.1f);

            switch(GiveRule)
            {
                case itemRule.RandomByDifficulty:
                    itemsToGive = (int)Run.instance.difficultyCoefficient * 3;
                    if (summon.GetBody().isBoss) { itemsToGive *= 3/2; }
                    if (summon.GetBody().isElite) { itemsToGive *= 3/2; }
                    if (summon.GetBody().isChampion) { itemsToGive *= 2; }
                    for (int i=0;i<itemsToGive;i++)
                    {
                        summon.inventory.GiveItem(GenerateItem(), 1);
                        yield return new WaitForSeconds(0.01f);
                    }
                    break;
                case itemRule.RandomByAverage:
                    int numPlayers = 0;
                    foreach (CharacterMaster characterMasterlist1 in CharacterMaster.readOnlyInstancesList)
                    {
                        numPlayers += 1;
                        itemsToGive += characterMasterlist1.inventory.GetTotalItemCountOfTier(ItemTier.Tier1);
                        itemsToGive += characterMasterlist1.inventory.GetTotalItemCountOfTier(ItemTier.Tier2);
                        itemsToGive += characterMasterlist1.inventory.GetTotalItemCountOfTier(ItemTier.Tier3);
                        itemsToGive += characterMasterlist1.inventory.GetTotalItemCountOfTier(ItemTier.Lunar);
                        itemsToGive += characterMasterlist1.inventory.GetTotalItemCountOfTier(ItemTier.Boss);
                    }
                    itemsToGive /= numPlayers;
                    if (summon.GetBody().isBoss) { itemsToGive *= 2; }
                    if (summon.GetBody().isElite) { itemsToGive *= 2; }
                    if (summon.GetBody().isChampion) { itemsToGive *= 2; }
                    for (int i = 0; i < itemsToGive; i++)
                    {
                        summon.inventory.GiveItem(GenerateItem(), 1);
                        yield return new WaitForSeconds(0.01f);
                    }
                    break;
                case itemRule.RandomByTotal:
                    foreach (CharacterMaster characterMasterlist2 in CharacterMaster.readOnlyInstancesList)
                    {
                        itemsToGive += characterMasterlist2.inventory.GetTotalItemCountOfTier(ItemTier.Tier1);
                        itemsToGive += characterMasterlist2.inventory.GetTotalItemCountOfTier(ItemTier.Tier2);
                        itemsToGive += characterMasterlist2.inventory.GetTotalItemCountOfTier(ItemTier.Tier3);
                        itemsToGive += characterMasterlist2.inventory.GetTotalItemCountOfTier(ItemTier.Lunar);
                        itemsToGive += characterMasterlist2.inventory.GetTotalItemCountOfTier(ItemTier.Boss);
                    }
                    if (summon.GetBody().isBoss) { itemsToGive *= 2; }
                    if (summon.GetBody().isElite) { itemsToGive *= 2; }
                    if (summon.GetBody().isChampion) { itemsToGive *= 2; }
                    for (int i = 0; i < itemsToGive; i++)
                    {
                        summon.inventory.GiveItem(GenerateItem(), 1);
                        yield return new WaitForSeconds(0.01f);
                    }
                    break;
                case itemRule.RatioOneToOne:
                    itemRatio = 1f;
                    break;
                case itemRule.RatioHalf:
                    itemRatio = 0.5f;
                    break;
                case itemRule.RatioThird:
                    itemRatio = 0.33f;
                    break;
                case itemRule.RatioFourth:
                    itemRatio = 0.25f;
                    break;
                case itemRule.LooseCopy:
                    CharacterMaster characterMaster = CharacterMaster.readOnlyInstancesList[new System.Random().Next(0, CharacterMaster.readOnlyInstancesList.Count - 1)];
                    summon.inventory.CopyItemsFrom(characterMaster.inventory);
                    break;
                case itemRule.ExactCopy:
                    CharacterMaster characterMaster2 = CharacterMaster.readOnlyInstancesList[new System.Random().Next(0, CharacterMaster.readOnlyInstancesList.Count - 1)];
                    summon.inventory.CopyItemsFrom(characterMaster2.inventory);
                    summon.inventory.CopyEquipmentFrom(characterMaster2.inventory);
                    summon.inventory.GiveItem(ItemIndex.AutoCastEquipment, 1);
                    break;
            }

            if(itemRatio!=0)
            {

                CharacterMaster characterMaster3 = CharacterMaster.readOnlyInstancesList[new System.Random().Next(0, CharacterMaster.readOnlyInstancesList.Count - 1)];//Choose a random player
                
                tier1Count = characterMaster3.inventory.GetTotalItemCountOfTier(ItemTier.Tier1);
                tier2Count = characterMaster3.inventory.GetTotalItemCountOfTier(ItemTier.Tier2);
                tier3Count = characterMaster3.inventory.GetTotalItemCountOfTier(ItemTier.Tier3);
                tier4Count = characterMaster3.inventory.GetTotalItemCountOfTier(ItemTier.Lunar);

                for (int i = 3; i < tier1Count * itemRatio; i++) 
                {
                    summon.inventory.GiveItem(GenerateItem(1, 0, 0, 0, 0), 1);
                    yield return new WaitForSeconds(0.01f);
                }
                for (int i = 2; i < tier2Count * itemRatio; i++) 
                {
                    summon.inventory.GiveItem(GenerateItem(0, 1, 0, 0, 0), 1);
                    yield return new WaitForSeconds(0.01f);
                }
                for (int i = 1; i < tier3Count * itemRatio; i++) 
                {
                    summon.inventory.GiveItem(GenerateItem(0, 0, 1, 0, 0), 1);
                    yield return new WaitForSeconds(0.01f);
                }
                for (int i = 0; i < tier4Count * itemRatio; i++) 
                {
                    summon.inventory.GiveItem(GenerateItem(0, 0, 0, 0, 1), 1);
                    yield return new WaitForSeconds(0.01f);
                }
            }
        }
    }
}


