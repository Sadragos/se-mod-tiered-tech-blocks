using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.Game.Components;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRage.Utils;

namespace TieredTechBlocks
{
    [MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation)]
    class Core : MySessionComponentBase
    {
        private bool _initialized;
        IMyEntity Entity;

        public void BeforeDamage(object target, ref MyDamageInformation info)
        {
            var slim = target as IMySlimBlock;
            if (slim == null || slim.CubeGrid == null) return;
            
            if (!MyAPIGateway.Entities.TryGetEntityById(info.AttackerId, out Entity)) return;

            var attacker = Entity as MyCubeBlock;
            if (attacker != null && attacker.CubeGrid.IsSameConstructAs(slim.CubeGrid))
                info.Amount = 0;
        }

        public override void UpdateBeforeSimulation()
        {
            try
            {
                if (MyAPIGateway.Session == null)
                    return;

                // Run the init
                if (!_initialized)
                {
                    _initialized = true;
                    Initialize();
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        private void Initialize()
        {
            if (MyAPIGateway.Multiplayer.IsServer)
            {
                MyLog.Default.WriteLine("Registered Damage Handler!");
                MyAPIGateway.Session.DamageSystem.RegisterBeforeDamageHandler(0, BeforeDamage);
            }
        }
    }
}
