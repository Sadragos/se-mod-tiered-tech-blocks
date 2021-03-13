using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI;
using Sandbox.Game;
using Sandbox.ModAPI.Interfaces;
using Sandbox.ModAPI.Interfaces.Terminal;
using VRage.Game;
using VRage.Game.Entity;
using VRage.Game.Components;
using VRage.Game.ModAPI;
using VRage.Game.ModAPI.Ingame;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using VRage.Utils;
using VRage;
using Sandbox.Definitions;

namespace TieredTechBlocks
{
	[MyEntityComponentDescriptor(typeof(MyObjectBuilder_CargoContainer), false)]
	public class CargoUpgradeHandler : UpgradeHandler<IMyCargoContainer, MyCargoContainerDefinition>
	{
		public CargoUpgradeHandler() : base(new Dictionary<string, string>()
        {
/*			{ "LargeBlockSmallContainer", "LargeBlockSmallContainer2x" },
			{ "LargeBlockSmallContainer2x", "LargeBlockSmallContainer4x" },
			{ "LargeBlockSmallContainer4x", "LargeBlockSmallContainer8x" }*/
		}) {}

		protected static bool init = false;
		public override void UpdateOnceBeforeFrame()
		{
			if (!init)
			{
				Buttons();
				init = true;
			}
		}

        public override void TransferCustomSettings(IMyCargoContainer oldBlock, IMyCargoContainer newBlock)
        {
            base.TransferCustomSettings(oldBlock, newBlock);
        }
    }
}