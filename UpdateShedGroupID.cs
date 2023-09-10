using Kitchen;
using KitchenMods;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;

namespace KitchenColoredTeleporters
{
    public class UpdateShedGroupID : NightSystem, IModSystem
    {
        private HashSet<int> TempAssigned = new HashSet<int>();
        EntityQuery Teleporters;

        protected override void Initialise()
        {
            base.Initialise();
            Teleporters = GetEntityQuery(typeof(CConveyTeleport), typeof(CAppliance));
        }

        protected override void OnUpdate()
        {
            using NativeArray<Entity> entities = Teleporters.ToEntityArray(Allocator.Temp);
            using NativeArray<CConveyTeleport> teleports = Teleporters.ToComponentDataArray<CConveyTeleport>(Allocator.Temp);
            using NativeArray<CAppliance> appliances = Teleporters.ToComponentDataArray<CAppliance>(Allocator.Temp);

            TempAssigned.Clear();
            foreach (CConveyTeleport item in teleports)
            {
                TempAssigned.Add(item.GroupID);
            }

            for (int i = 0; i < entities.Length; i++)
            {
                if (appliances[i].ID != Main.SHED_MAGIC_EVERYTHING_APPLIANCE_ID)
                    continue;

                CConveyTeleport teleport = teleports[i];

                if (teleport.GroupID != 0 || teleport.Target == default)
                    continue;

                int groupID = -1;
                for (int j = -1; j > -entities.Length - 1; j--)
                {
                    if (!TempAssigned.Contains(j))
                    {
                        TempAssigned.Add(j);
                        groupID = j;
                        break;
                    }
                }

                teleport.GroupID = groupID;
                Entity entity = entities[i];
                Set(entity, teleport);
            }
        }
    }
}
