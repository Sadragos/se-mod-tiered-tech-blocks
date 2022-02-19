using System;
using System.Collections.Generic;
using Sandbox.Game;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.ModAPI;
using VRage.Utils;
using VRageMath;


namespace TieredTechBlocks
{
    [MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
    public class MoreLoot : MySessionComponentBase
    {

        IMyCubeGrid Grid = null;
        List<IMySlimBlock> GridBlocks = new List<IMySlimBlock>();
        List<IMyCargoContainer> Container = new List<IMyCargoContainer>();
        

        Item Tech2, Tech4, Tech8;
        int MaxContainers = 5;

        public override void Init(MyObjectBuilder_SessionComponent sessionComponent)
        {
            if (MyAPIGateway.Session.IsServer)
            {
                Config.Load();

                Tech2 = new Item
                {
                    builder = new MyObjectBuilder_Component() { SubtypeName = "Tech2x" },
                    chanceSmall = Config.Instance.SmallGridCommon.Chance,
                    chanceLarge = Config.Instance.LargeGridCommon.Chance,
                    minItemsSmall = Config.Instance.SmallGridCommon.MinAmount,
                    maxItemsSmall = Config.Instance.SmallGridCommon.MaxAmount,
                    minItemsLarge = Config.Instance.LargeGridCommon.MinAmount,
                    maxItemsLarge = Config.Instance.LargeGridCommon.MaxAmount
                };
                Tech4 = new Item
                {
                    builder = new MyObjectBuilder_Component() { SubtypeName = "Tech4x" },
                    chanceSmall = Config.Instance.SmallGridRare.Chance,
                    chanceLarge = Config.Instance.LargeGridRare.Chance,
                    minItemsSmall = Config.Instance.SmallGridRare.MinAmount,
                    maxItemsSmall = Config.Instance.SmallGridRare.MaxAmount,
                    minItemsLarge = Config.Instance.LargeGridRare.MinAmount,
                    maxItemsLarge = Config.Instance.LargeGridRare.MaxAmount
                };
                Tech8 = new Item
                {
                    builder = new MyObjectBuilder_Component() { SubtypeName = "Tech8x" },
                    chanceSmall = Config.Instance.SmallGridExotic.Chance,
                    chanceLarge = Config.Instance.LargeGridExotic.Chance,
                    minItemsSmall = Config.Instance.SmallGridExotic.MinAmount,
                    maxItemsSmall = Config.Instance.SmallGridExotic.MaxAmount,
                    minItemsLarge = Config.Instance.LargeGridExotic.MinAmount,
                    maxItemsLarge = Config.Instance.LargeGridExotic.MaxAmount
                };
                MyVisualScriptLogicProvider.PrefabSpawnedDetailed += NewSpawn;
            }
        }


        private bool AddLoot(IMyCargoContainer container)
        {
            bool added = false;

            bool isLarge = container.CubeGrid.GridSizeEnum == MyCubeSize.Large;
            IMyInventory inventory = container.GetInventory();

            try
            {
                if (MyUtils.GetRandomDouble(0, 1) <= (isLarge ? Tech2.chanceLarge : Tech2.chanceSmall))
                {
                    int amount = MyUtils.GetRandomInt((isLarge ? Tech2.minItemsLarge : Tech2.minItemsSmall), (isLarge ? Tech2.maxItemsLarge : Tech2.maxItemsSmall));
                    MyLog.Default.WriteLine("TieredTechBlocks: Added " + amount + "x Common Tech to " + container.CustomName);
                    inventory.AddItems(amount, Tech2.builder);
                    added = true;
                }
                if (MyUtils.GetRandomDouble(0, 1) <= (isLarge ? Tech4.chanceLarge : Tech4.chanceSmall))
                {
                    int amount = MyUtils.GetRandomInt((isLarge ? Tech4.minItemsLarge : Tech4.minItemsSmall), (isLarge ? Tech4.maxItemsLarge : Tech4.maxItemsSmall));
                    MyLog.Default.WriteLine("TieredTechBlocks: Added " + amount + "x Rare Tech to " + container.CustomName);
                    inventory.AddItems(amount, Tech4.builder);
                    added = true;
                }
                if (MyUtils.GetRandomDouble(0, 1) <= (isLarge ? Tech8.chanceLarge : Tech8.chanceSmall))
                {
                    int amount = MyUtils.GetRandomInt((isLarge ? Tech8.minItemsLarge : Tech8.minItemsSmall), (isLarge ? Tech8.maxItemsLarge : Tech8.maxItemsSmall));
                    MyLog.Default.WriteLine("TieredTechBlocks: Added " + amount + "x Exotic Tech to " + container.CustomName);
                    inventory.AddItems(amount, Tech8.builder);
                    added = true;
                }
            }
            catch (Exception e)
            {
                MyLog.Default.WriteLine("TieredTechBlocks:  FAILED " + e);
            }

            return added;


        }

        private void NewSpawn(long entityId, string prefabName)
        {
            try
            {
                Grid = null;
                Grid = MyAPIGateway.Entities.GetEntityById(entityId) as IMyCubeGrid;
                if (Grid != null && Grid.Physics != null)
                {
                    if(Config.Instance.ExcludeGrids.Contains(prefabName.ToLower()) || Config.Instance.ExcludeGrids.Contains(Grid.CustomName.ToLower()))
                    {
                        return;
                    }
                    Container.Clear();
                    GridBlocks.Clear();
                    Grid.GetBlocks(GridBlocks);

                    foreach (var block in GridBlocks)
                    {
                        if (block.FatBlock != null && block.FatBlock is IMyCargoContainer)
                        {
                            var cargo = block.FatBlock as IMyCargoContainer;
                            if (cargo != null && !cargo.MarkedForClose && cargo.IsWorking)
                            {
                                var inventory = cargo.GetInventory();
                                if (cargo.GetInventory() != null)
                                {
                                    Container.Add(cargo);
                                }
                            }
                        }
                    }

                    MyLog.Default.WriteLine("TieredTechBlocks: Valid Grid " + Grid.CustomName + " spawned with " + Container.Count + " possible Cargos");

                    Container.ShuffleList();
                    int addedLoot = 0;
                    foreach (IMyCargoContainer cargo in Container)
                    {
                        if (AddLoot(cargo) && ++addedLoot >= MaxContainers) break;
                    }

                }
            }
            catch (Exception e)
            {
                MyLog.Default.WriteLine("TieredTechBlocks: " + e);
            }
        }

        public static MyObjectBuilder_PhysicalObject GetBuilder(string category, string name)
        {
            switch (category)
            {
                case "MyObjectBuilder_Component":
                    return new MyObjectBuilder_Component() { SubtypeName = name };
                case "MyObjectBuilder_AmmoMagazine":
                    return new MyObjectBuilder_AmmoMagazine() { SubtypeName = name };
                case "MyObjectBuilder_Ingot":
                    return new MyObjectBuilder_Ingot() { SubtypeName = name };
                case "MyObjectBuilder_Ore":
                    return new MyObjectBuilder_Ore() { SubtypeName = name };
                case "MyObjectBuilder_ConsumableItem":
                    return new MyObjectBuilder_ConsumableItem() { SubtypeName = name };
                case "MyObjectBuilder_PhysicalGunObject":
                    return new MyObjectBuilder_PhysicalGunObject() { SubtypeName = name };
                default: return new MyObjectBuilder_PhysicalObject() { SubtypeName = name };
            }
        }

        protected override void UnloadData()
        {
            MyVisualScriptLogicProvider.PrefabSpawnedDetailed -= NewSpawn; //Make sure to unregister
        }

        protected struct Item
        {
            public MyObjectBuilder_Component builder;
            public int minItemsSmall;
            public int minItemsLarge;
            public int maxItemsSmall;
            public int maxItemsLarge;
            public double chanceSmall;
            public double chanceLarge;
        }
    }
}