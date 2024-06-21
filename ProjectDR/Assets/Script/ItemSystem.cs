using System;
using UnityEngine;

public class ItemSystem : MonoBehaviour
{
    public enum ItemSlotType { Equip, Inventory, Reward }


    [Serializable]
    private class ItemClass
    {
        public ItemData Data;

        //Ω∫≈»
        public ICreature.Stats Stat1;
        public int Stat1_Value;
        public int[] Stat1_Arr;
        public ICreature.Stats Stat2;
        public int Stat2_Value;
        public int[] Stat2_Arr;

        //«‡µø
        public ICreature.BtlActClass BtlAct1 = null;
        public ICreature.BtlActClass BtlAct2 = null;
    }
}
