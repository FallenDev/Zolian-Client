using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoftKitty.InventoryEngine
{
    public class LootPack : MonoBehaviour
    {
        #region Variables
        public string UID = "LootPack0000001";
        public List<int> ItemPool=new List<int>();
        public List<int> CurrencyMin = new List<int>();
        public List<int> CurrencyMax = new List<int>();
        public int MaxiumItemCount = 5;
        public int MaxiumCountEachItem = 3;
        public float DropChanceMultiplier = 1F;
        public bool RandomLevel = false;
        public bool DestoryWhenPlayerCloseLootWindow = true;
        private LootUi lootWindow;
        private InventoryHolder holder;
        private bool inited = false;
        private bool windowWasOpen = false;
        #endregion

        #region Internal Methods
        public static void GetRandomLoot(InventoryHolder _holder, List<int> _itemIdPool, int[] _currencyMinArray, int[] _currencyMaxArray, int _maxiumItemCount = 5, float _dropChanceMultiplier = 1F, bool _randomLevel=false, int _maxiumCountEachItem = 3)
        {
            List<Item> _dropList = new List<Item>();
            Random.InitState(System.DateTime.Now.Millisecond + System.DateTime.Now.Hour * 10000 + System.DateTime.Now.Minute * 1000);
            foreach (int obj in _itemIdPool)
            {
                if (Random.Range(0, 100) < ItemManager.itemDic[obj].dropRates * _dropChanceMultiplier)
                {
                    Item _newItem = new Item(obj,true, true);
                    if (_randomLevel && ItemManager.instance.EnableEnhancing && _newItem.type == ItemManager.instance.EnhancingCategoryID)
                    {
                        int _level = Random.Range(0, ItemManager.instance.MaxiumEnhancingLevel);
                        if (Random.Range(0, 100) < ItemManager.instance.EnhancingSuccessCurve.Evaluate(_level*1F/ ItemManager.instance.MaxiumEnhancingLevel)) {
                            _newItem.upgradeLevel = _level;
                        }
                    }
                    _dropList.Add(_newItem);
                }
            }

            if (_dropList.Count <= 0)
            {
                Item _newItem = new Item(_itemIdPool[Random.Range(0, _itemIdPool.Count)], true, true);
                if (_randomLevel && ItemManager.instance.EnableEnhancing && _newItem.type == ItemManager.instance.EnhancingCategoryID)
                {
                    int _level = Random.Range(0, ItemManager.instance.MaxiumEnhancingLevel);
                    if (Random.Range(0, 100) < ItemManager.instance.EnhancingSuccessCurve.Evaluate(_level * 1F / ItemManager.instance.MaxiumEnhancingLevel))
                    {
                        _newItem.upgradeLevel = _level;
                    }
                }
                _dropList.Add(_newItem);
            }

            _dropList.Sort(SortByRandom);
            int _dropCount = Mathf.Min(_dropList.Count, Random.Range(1, _maxiumItemCount + 1));
            _holder.InventorySize = _dropCount;
            for (int i = 0; i < _dropCount; i++)
            {
                _holder.Stacks.Add(new InventoryStack());
            }
            for (int i = 0; i < _dropCount; i++)
            {
                _holder.AddItem(_dropList[i], Random.Range(1, Mathf.Min(_dropList[i].maxiumStack, _maxiumCountEachItem) + 1));
            }
            for (int i = 0; i < Mathf.Min(_currencyMinArray.Length, _currencyMaxArray.Length, ItemManager.instance.currencies.Count); i++)
            {
                _holder.AddCurrency(i, Random.Range(_currencyMinArray[i], _currencyMaxArray[i]));
            }
        }

        private static int SortByRandom(Item a, Item b)
        {
            return Random.Range(0, 100).CompareTo(Random.Range(0, 100));
        }

        private void Update()
        {
            if (lootWindow != null)
            {
                windowWasOpen = true;
                if (lootWindow.isEmpty()) DestoryWhenPlayerCloseLootWindow = true;
            }
            else
            {
                if (windowWasOpen && DestoryWhenPlayerCloseLootWindow) DestroyPack();
            }
        }
        public void UpdatePrefab()
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }


        private void Init()
        {
            holder = gameObject.AddComponent<InventoryHolder>();
            holder.name = UID;
            GetRandomLoot(holder, ItemPool, CurrencyMin.ToArray(), CurrencyMax.ToArray(), MaxiumItemCount, DropChanceMultiplier,RandomLevel,  MaxiumCountEachItem);
            inited = true;
        }
        #endregion



        //Call this to open the interface of this loot pack.
        public void OpenPack()
        {
            if (!inited) Init();
            if (lootWindow == null)
            {
                lootWindow = LootUi.ShowLoot(holder);
            }
            
        }

        //Destroy this loot pack
        public void DestroyPack()
        {
            Destroy(gameObject);
        }

       

    }
}
