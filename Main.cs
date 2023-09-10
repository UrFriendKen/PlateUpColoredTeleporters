using HarmonyLib;
using KitchenData;
using KitchenMods;
using System.Reflection;
using UnityEngine;

// Namespace should have "Kitchen" in the beginning
namespace KitchenColoredTeleporters
{
    public class Main : IModInitializer
    {
        public const string MOD_GUID = $"IcedMilo.PlateUp.{MOD_NAME}";
        public const string MOD_NAME = "Colored Teleporters";
        public const string MOD_VERSION = "0.1.1";

        internal const int TELEPORTER_APPLIANCE_ID = 459840623;
        internal const int SHED_MAGIC_EVERYTHING_APPLIANCE_ID = -349733673;
        internal const int SHED_TELEPORT_TARGET_APPLIANCE_ID = 1836107598;

        public Main()
        {
            Harmony harmony = new Harmony(MOD_GUID);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public void PostActivate(KitchenMods.Mod mod)
        {

        }

        public void PreInject()
        {
            UpdateTeleporterAppliance();
            UpdateShedMagicEverythingAppliance();
            UpdateShedTeleportTarget();
        }

        void UpdateTeleporterAppliance()
        {
            if (!GameData.Main.TryGet(TELEPORTER_APPLIANCE_ID, out Appliance teleporter))
            {
                Main.LogError("Failed to get Teleporter GDO!");
                return;
            }
            if (teleporter.Prefab == null)
            {
                Main.LogError("Teleporter Prefab is null!");
                return;
            }
            Transform teleporterPrefabContainer = teleporter.Prefab?.transform.Find("Teleporter");
            if (teleporterPrefabContainer == null)
            {
                Main.LogError("Failed to find teleporter transform!");
                return;
            }

            MeshRenderer teleporterArrowsRenderer = teleporterPrefabContainer.Find("Arrows")?.gameObject?.GetComponent<MeshRenderer>();
            if (teleporterArrowsRenderer == null)
            {
                Main.LogError("Failed to get Teleporter Arrows renderer!");
                return;
            }
            MeshRenderer teleporterSurfaceRenderer = teleporterPrefabContainer.Find("Surface")?.gameObject?.GetComponent<MeshRenderer>();
            if (teleporterSurfaceRenderer == null)
            {
                Main.LogError("Failed to get Teleporter Surface renderer!");
                return;
            }

            TeleporterColorView teleporterColorView = teleporter.Prefab.AddComponent<TeleporterColorView>();
            teleporterColorView.ArrowsRenderer = teleporterArrowsRenderer;
            teleporterColorView.SurfaceRenderer = teleporterSurfaceRenderer;

            Main.LogInfo("Successfully added TeleporterColorView to teleporter.");
        }

        void UpdateShedMagicEverythingAppliance()
        {
            if (!GameData.Main.TryGet(SHED_MAGIC_EVERYTHING_APPLIANCE_ID, out Appliance shedMagicEverthing))
            {
                Main.LogError("Failed to get Shed Magic Everything GDO!");
                return;
            }
            if (shedMagicEverthing.Prefab == null)
            {
                Main.LogError("Shed Magic Everything Prefab is null!");
                return;
            }
            Transform shed = shedMagicEverthing.Prefab?.transform.Find("Shed");
            if (shed == null)
            {
                Main.LogError("Failed to find Shed transform!");
                return;
            }

            MeshRenderer roofRenderer = shed.Find("Roof")?.gameObject?.GetComponent<MeshRenderer>();
            if (roofRenderer == null)
            {
                Main.LogError("Failed to get Shed Roof renderer!");
                return;
            }

            TeleporterColorView teleporterColorView = shedMagicEverthing.Prefab.AddComponent<TeleporterColorView>();
            teleporterColorView.SurfaceRenderer = roofRenderer;

            Main.LogInfo("Successfully added TeleporterColorView to Shed Magic Everything.");
        }


        void UpdateShedTeleportTarget()
        {
            if (!GameData.Main.TryGet(SHED_TELEPORT_TARGET_APPLIANCE_ID, out Appliance shedTeleportTarget))
            {
                Main.LogError("Failed to get Shed Teleport Target GDO!");
                return;
            }
            if (shedTeleportTarget.Prefab == null)
            {
                Main.LogError("Shed Teleport Target Prefab is null!");
                return;
            }
            Transform shed = shedTeleportTarget.Prefab?.transform.Find("Shed");
            if (shed == null)
            {
                Main.LogError("Failed to find Shed transform!");
                return;
            }

            MeshRenderer roofRenderer = shed.Find("Roof")?.gameObject?.GetComponent<MeshRenderer>();
            if (roofRenderer == null)
            {
                Main.LogError("Failed to get Shed Roof renderer!");
                return;
            }

            TeleporterColorView teleporterColorView = shedTeleportTarget.Prefab.AddComponent<TeleporterColorView>();
            teleporterColorView.SurfaceRenderer = roofRenderer;

            Main.LogInfo("Successfully added TeleporterColorView to Shed Teleport Target.");
        }

        public void PostInject()
        {
        }

        #region Logging
        public static void LogInfo(string _log) { Debug.Log($"[{MOD_NAME}] " + _log); }
        public static void LogWarning(string _log) { Debug.LogWarning($"[{MOD_NAME}] " + _log); }
        public static void LogError(string _log) { Debug.LogError($"[{MOD_NAME}] " + _log); }
        public static void LogInfo(object _log) { LogInfo(_log.ToString()); }
        public static void LogWarning(object _log) { LogWarning(_log.ToString()); }
        public static void LogError(object _log) { LogError(_log.ToString()); }
        #endregion
    }
}
