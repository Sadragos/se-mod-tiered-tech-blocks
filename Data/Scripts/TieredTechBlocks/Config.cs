using System;
using System.IO;
using System.Text;
using Sandbox.Game;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Weapons;
using System.Collections.Generic;
using System.Linq;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRage.Game;
using VRage.Utils;

namespace TieredTechBlocks
{

    public class Config
    {

        public static MyConfig Instance;

        public static void Load()
        {
            // Load config xml
            if (MyAPIGateway.Utilities.FileExistsInWorldStorage("TieredTechBlocksConfig.xml", typeof(MyConfig)))
            {
                try
                {
                    TextReader reader = MyAPIGateway.Utilities.ReadFileInWorldStorage("TieredTechBlocksConfig.xml", typeof(MyConfig));
                    var xmlData = reader.ReadToEnd();
                    Instance = MyAPIGateway.Utilities.SerializeFromXML<MyConfig>(xmlData);
                    reader.Dispose();
                    MyLog.Default.WriteLine("TieredTechBlocks: found and loaded");
                }
                catch (Exception e)
                {
                    MyLog.Default.WriteLine("TieredTechBlocks: loading failed, generating new Config");
                }
            }

            if (Instance == null)
            {
                MyLog.Default.WriteLine("TieredTechBlocks: No Loot Config found, creating New");
                // Create default values
                Instance = new MyConfig()
                {
                    SmallGridCommon = new Item() { Chance = 0.2f, MinAmount = 3, MaxAmount = 9 },
                    LargeGridCommon = new Item() { Chance = 0.15f, MinAmount = 6, MaxAmount = 40 },
                    SmallGridRare = new Item() { Chance = 0.1f, MinAmount = 2, MaxAmount = 6 },
                    LargeGridRare = new Item() { Chance = 0.07f, MinAmount = 3, MaxAmount = 20 },
                    SmallGridExotic = new Item() { Chance = 0.05f, MinAmount = 1, MaxAmount = 3 },
                    LargeGridExotic = new Item() { Chance = 0.04f, MinAmount = 2, MaxAmount = 10 },
                    ExcludeGrids = new List<string>() { "respawn" }
                };
            }


            // Updates
            if(Instance.ExcludeGrids == null)
            {
                Instance.ExcludeGrids = new List<string>() { "respawn" };
            }

            Write();
        }


        public static void Write()
        {
            if (Instance == null) return;

            try
            {
                MyLog.Default.WriteLine("TieredTechBlocks: Serializing to XML... ");
                string xml = MyAPIGateway.Utilities.SerializeToXML<MyConfig>(Instance);
                MyLog.Default.WriteLine("TieredTechBlocks: Writing to disk... ");
                TextWriter writer = MyAPIGateway.Utilities.WriteFileInWorldStorage("TieredTechBlocksConfig.xml", typeof(MyConfig));
                writer.Write(xml);
                writer.Flush();
                writer.Close();
            }
            catch (Exception e)
            {
                MyLog.Default.WriteLine("TieredTechBlocks: Error saving XML!" + e.StackTrace);
            }
        }
    }
}