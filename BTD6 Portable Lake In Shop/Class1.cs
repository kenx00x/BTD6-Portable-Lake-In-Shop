﻿using Assets.Main.Scenes;
using Assets.Scripts.Models.Powers;
using Assets.Scripts.Models.Profile;
using Assets.Scripts.Models.TowerSets;
using Assets.Scripts.Simulation.Input;
using Assets.Scripts.Unity;
using Assets.Scripts.Unity.UI_New.Upgrade;
using Harmony;
using Il2CppSystem.Collections.Generic;
using MelonLoader;
using System.IO;
[assembly: MelonInfo(typeof(BTD6_Portable_Lake_In_Shop.Class1), "Portable Lake In Shop", "1.0.0", "kenx00x")]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]
namespace BTD6_Portable_Lake_In_Shop
{
    public class Class1 : MelonMod
    {
        public static string dir = $"{Directory.GetCurrentDirectory()}\\Mods\\PortableLakeInShop";
        public static string config = $"{dir}\\config.txt";
        public static int PortableLakeCost = 470;
        public override void OnApplicationStart()
        {
            MelonLogger.Log("Portable Lake In Shop mod loaded");
            Directory.CreateDirectory($"{dir}");
            if (File.Exists(config))
            {
                MelonLogger.Log("Reading config file");
                using (StreamReader sr = File.OpenText(config))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        PortableLakeCost = int.Parse(s.Substring(s.IndexOf(char.Parse("=")) + 1));
                    }
                }
                MelonLogger.Log("Done reading");
            }
            else
            {
                MelonLogger.Log("Creating config file");
                using (StreamWriter sw = File.CreateText(config))
                {
                    sw.WriteLine("PortableLakeCost=470");
                }
                MelonLogger.Log("Done Creating");
            }
        }
        [HarmonyPatch(typeof(ProfileModel), "Validate")]
        public class ProfileModel_Patch
        {
            [HarmonyPostfix]
            public static void Postfix(ProfileModel __instance)
            {
                List<string> unlockedTowers = __instance.unlockedTowers;
                if (unlockedTowers.Contains("PortableLake"))
                {
                    MelonLogger.Log("Portable Lake already unlocked");
                }
                else
                {
                    MelonLogger.Log("unlocking Portable Lake");
                    unlockedTowers.Add("PortableLake");
                }
            }
        }
        [HarmonyPatch(typeof(TitleScreen), "UpdateVersion")]
        public class TitleScreen_Patch
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                PowerModel powerModel = Game.instance.model.GetPowerWithName("PortableLake");
                if (powerModel.tower.icon == null)
                {
                    powerModel.tower.icon = powerModel.icon;
                }
                powerModel.tower.cost = PortableLakeCost;
                powerModel.tower.towerSet = "Support";
            }
        }
        [HarmonyPatch(typeof(TowerInventory), "Init")]
        public class TowerInventory_Patch
        {
            [HarmonyPrefix]
            public static bool Prefix(ref List<TowerDetailsModel> allTowersInTheGame)
            {
                ShopTowerDetailsModel powerDetails = new ShopTowerDetailsModel("PortableLake", 1, 0, 0, 0, -1, 0, null);
                allTowersInTheGame.Add(powerDetails);
                return true;
            }
        }
        [HarmonyPatch(typeof(UpgradeScreen), "UpdateUi")]
        public class UpgradeScreen_Patch
        {
            [HarmonyPrefix]
            public static bool Prefix(ref string towerId)
            {
                if (towerId.Contains("PortableLake"))
                {
                    towerId = "DartMonkey";
                }
                return true;
            }
        }
    }
}