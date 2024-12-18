using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace AllTheFactory
{
    internal static class ModInfo
    {
        internal const string Guid = "ba.atf";
        internal const string Name = "All The Factory";
        internal const string Version = "1.0";
    }

    [BepInPlugin(ModInfo.Guid, ModInfo.Name, ModInfo.Version)]
    internal class AllTheFactory : BaseUnityPlugin
    {
        Thing factory;
        void Awake()
        {
            Logger.LogInfo("My mod...loaded?!");
        }
        public void OnStartCore()
        {
            var dir = Path.GetDirectoryName(Info.Location);
            var excel = dir + "/AllTheFactory.xlsx";
            var sources = Core.Instance.sources;
            ModUtil.ImportExcel(excel, "Thing", sources.things);
        }

        void Start()
        {

            var sources = Core.Instance.sources.things.GetRow("");
            GetRecipe();
        }
        void GetRecipe()
        {
            // 遍历所有配方并打印其信息
            RecipeManager.BuildList(); // 确保配方列表已构建

            foreach (var recipe in RecipeManager.list)
            {
                string logHandler = $"配方ID: {recipe.id}, 类型: {recipe.type}, 是否桥梁: {recipe.isBridge}, 是否桥梁支柱: {recipe.isBridgePillar}";
                Logger.LogInfo(logHandler);
            }
        }

    }

    [HarmonyPatch(typeof(LayerCraft), nameof(LayerCraft.RefreshCategory))]
    class GetRecipe
    {
        static void Prefix(Thing factory)
        {
            UIDynamicList list = new UIDynamicList();
            object o2 = list.rows[0].objects[0];
            string key = ((factory.id == "test_ba") ? factory.id : "hand");  // 获取当前工厂ID，默认为"hand"
            if (key == "test_ba")
            {
                string text = ELayer.player.lastRecipes[key];
                foreach (object @object in list.objects)
                {
                    // 查找并选中上次使用的配方
                    if ((@object as Recipe).id == text)
                    {
                        o2 = @object;
                        break;
                    }
                }
            }
        }
        
    }

}
