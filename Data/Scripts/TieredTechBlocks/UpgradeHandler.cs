using Sandbox.Definitions;
using Sandbox.Game;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces.Terminal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.Entity;
using VRage.Game.ModAPI.Ingame;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using VRage.Utils;

namespace TieredTechBlocks
{
    public abstract class UpgradeHandler<BT, CBT> : MyGameLogicComponent where BT : IMyTerminalBlock where CBT : MyCubeBlockDefinition
	{
		protected Dictionary<string, string> Upgrades;

        public UpgradeHandler(Dictionary<string, string> upgrades)
        {
            Upgrades = upgrades;
        }

		public override void Init(MyObjectBuilder_EntityBase objectBuilder)
		{
			NeedsUpdate = MyEntityUpdateEnum.BEFORE_NEXT_FRAME;
		}

		public void Buttons()
		{
			var btn1 = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlButton, IMyCargoContainer>("terminalUpgrade");
			btn1.Title = MyStringId.GetOrCompute("Upgrade");
			btn1.Tooltip = MyStringId.GetOrCompute("Upgrades this Block to the next level");
			btn1.Action = UpgradeAction;
			btn1.Enabled = IsVisible;
			btn1.Visible = IsVisible;

			MyAPIGateway.TerminalControls.AddControl<IMyCargoContainer>(btn1);
		}

		public bool IsVisible(IMyTerminalBlock block)
		{
			return Upgrades.ContainsKey(block.BlockDefinition.SubtypeId);
		}

		public void UpgradeAction(IMyTerminalBlock block)
		{
			Dictionary<int, List<MyInventoryItem>> InventoryTransfer = new Dictionary<int, List<MyInventoryItem>>();

			MyEntity entity = MyVisualScriptLogicProvider.GetEntityById(block.EntityId);
			var blockSelf = entity as VRage.Game.ModAPI.IMyCubeBlock;
			var selfPos = blockSelf.Position;
			var selfGrid = blockSelf.CubeGrid;
			var selfOb = blockSelf.GetObjectBuilderCubeBlock(true);
			var ob = new MyObjectBuilder_CubeBlock()
			{
				SubtypeName = Upgrades[block.BlockDefinition.SubtypeId],
				BuildPercent = selfOb.BuildPercent,
				IntegrityPercent = selfOb.IntegrityPercent,
				Min = selfOb.Min,
				BlockOrientation = selfOb.BlockOrientation,
				SkinSubtypeId = selfOb.SkinSubtypeId,
				ColorMaskHSV = selfOb.ColorMaskHSV,
				BuiltBy = selfOb.BuiltBy,
				ShareMode = selfOb.ShareMode,
				Owner = selfOb.Owner
			};

			MyCubeBlockDefinition def = MyDefinitionManager.Static.GetDefinition<CBT>(Upgrades[block.BlockDefinition.SubtypeId]);

			if (block.InventoryCount > 0)
			{
				for (int i = 0; i < block.InventoryCount; i++)
				{
					List<MyInventoryItem> items = new List<MyInventoryItem>();
					block.GetInventory(i).GetItems(items);
					InventoryTransfer.Add(i, items);
					while (block.GetInventory(i).ItemCount > 0) block.GetInventory(i).RemoveItemsAt(0);
				}
			}

			MyAPIGateway.Utilities.InvokeOnGameThread(() =>
			{
				selfGrid.RazeBlock(selfPos);
				var newBlock = selfGrid.AddBlock(ob, false);

				VRage.Game.ModAPI.IMyCubeBlock fatNewBlock = newBlock.FatBlock;
				var terminal = fatNewBlock as IMyTerminalBlock;

				terminal.CustomName = block.CustomName;
				terminal.CustomData = block.CustomData;
				terminal.ShowInInventory = block.ShowInInventory;
				terminal.ShowInTerminal = block.ShowInTerminal;
				terminal.ShowInToolbarConfig = block.ShowInToolbarConfig;
				terminal.ShowOnHUD = block.ShowOnHUD;

				if (InventoryTransfer.Count > 0)
				{
					List<MyInventoryItem> items = new List<MyInventoryItem>();
					for (int i = 0; i < terminal.InventoryCount; i++)
					{
						foreach (MyInventoryItem item in InventoryTransfer[i])
						{
							terminal.GetInventory(i).AddItems(item.Amount, GetBuilder(item.Type.TypeId, item.Type.SubtypeId));
						}
					}
				}

				TransferCustomSettings((BT) block, (BT) fatNewBlock);
			});
		}

		public virtual void TransferCustomSettings(BT oldBlock, BT newBlock)
        {

        }

		public MyObjectBuilder_PhysicalObject GetBuilder(string category, string name)
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
	}
}
