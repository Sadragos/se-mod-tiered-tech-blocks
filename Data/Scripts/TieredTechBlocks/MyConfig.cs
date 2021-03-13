using System;
using System.Collections.Generic;
using ProtoBuf;
using System.Xml.Serialization;
using VRageMath;
using VRage.Game;
using System.Text;

namespace TieredTechBlocks
{
    [ProtoContract]
    [Serializable]
    public class MyConfig
    {
        [ProtoMember(1)]
        public Item SmallGridCommon;
        [ProtoMember(2)]
        public Item LargeGridCommon;
        [ProtoMember(3)]
        public Item SmallGridRare;
        [ProtoMember(4)]
        public Item LargeGridRare;
        [ProtoMember(5)]
        public Item SmallGridExotic;
        [ProtoMember(6)]
        public Item LargeGridExotic;
        [ProtoMember(7)]
        public List<string> ExcludeGrids;

    }

    [ProtoContract]
    [Serializable]
    public class Item
    {
        [XmlAttribute]
        public float Chance;
        [XmlAttribute]
        public int MinAmount;
        [XmlAttribute]
        public int MaxAmount;
    }
}