using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NicoFramework.Modules.Bag
{
    public class Bag<T> : IBag<T> where T : IBagItem
    {
        private List<T> _bagItems;
        private readonly Dictionary<string, List<T>> _bagItemMapById;
        private Bag<T> _selectedItems;
        public event Action OnItemAdd;
        public event Action OnItemRemove;
        public event Action OnItemTakeOut;
        public int Count => _bagItems.Count;
        public int MaxCapacity { get; set; }

        public Bag(int maxCapacity)
        {
            _bagItemMapById = new Dictionary<string, List<T>>();
            _bagItems = new List<T>();
            MaxCapacity = maxCapacity;
        }

        public Bag(List<T> bagItems)
        {
            _bagItemMapById = new Dictionary<string, List<T>>();
            _bagItems = bagItems;
            // 将每一个物品都加入 id 索引的字典
            foreach (var item in _bagItems) {
                List<T> list;
                var find = _bagItemMapById.TryGetValue(item.Id, out list);
                if (find) {
                    // 如果有相同物品直接存入拿到的 list
                    list.Add(item);
                } else {
                    // 没有相同物品就新建插入
                    _bagItemMapById.Add(item.Id, new List<T> { item });
                }
            }

            MaxCapacity = _bagItems.Count;
        }

        #region CRUD

        public void Add(T item)
        {
            if (Count >= MaxCapacity) {
                // 如果添加进的有相同 ID 且符合规范且最终没有超过 item.StackLimit
                var sameIdItem = GetCanStackItemById(item.Id);
                if (sameIdItem != null && item.CheckLegal(this) &&
                    sameIdItem.StackCount + item.StackCount <= item.StackLimit) {
                    sameIdItem.StackCount += item.StackCount;
                    OnItemAdd?.Invoke();
                } else {
                    Debug.LogError("背包容量不足");
                }

                return;
            }

            // 对应此 ID 的物品第一次加入背包
            if (!_bagItemMapById.ContainsKey(item.Id)) {
                RegisterBagItem(item);
                return;
            }

            // 检查新加入背包的物体是否符合规范，规范由第一批加入的物体决定
            if (!item.CheckLegal(this)) {
                Debug.Log("加入的物品不规范");
                return;
            }

            // 不可堆叠物品的添加
            if (!item.Stackable) {
                RegisterBagItem(item);
                return;
            }

            // 后面的物品在背包里面肯定有一个已有的同 Id 物品且可堆叠
            var canStackableItem = GetCanStackItemById(item.Id);
            var mergeCount = item.StackCount + canStackableItem.StackCount;
            var endCount = mergeCount - item.StackLimit > 0 ? mergeCount - item.StackLimit : mergeCount;
            if (endCount == mergeCount) {
                canStackableItem.StackCount = endCount;
            } else {
                canStackableItem.StackCount = canStackableItem.StackLimit;
                item.StackCount = endCount;
                RegisterBagItem(item);
            }
        }

        public void TakeOut(int index, int takeoutCount)
        {
            var item = GetItemByIndex(index);
            if (item.StackCount < takeoutCount) {
                Debug.LogError("此格子物品数量不足");
            } else if (item.StackCount == takeoutCount) {
                _bagItems.Remove(item);
                var items = GetItemsById(item.Id);
                items.Remove(item);
            } else {
                item.StackCount -= takeoutCount;
            }

            OnItemTakeOut?.Invoke();
        }

        public void RemoveById(string id)
        {
            // 清除字典里面的 Id
            _bagItemMapById.Remove(id);
            // 清除列表里面的 Id
            _bagItems.RemoveAll(item => item.Id == id);

            OnItemRemove?.Invoke();
        }

        public void RemoveByIndex(int index)
        {
            if (index < 0 || index >= Count) {
                return;
            }

            var item = _bagItems[index];
            _bagItemMapById[item.Id].Remove(item);
            _bagItems.Remove(item);

            OnItemRemove?.Invoke();
        }

        public void Clear()
        {
            _bagItems.Clear();
        }

        public T GetItemByIndex(int index)
        {
            if (index < 0 || index >= Count) {
                Debug.LogError($"Index: {index} 数组越界");
            }

            return _bagItems[index];
        }

        public List<T> GetItemsById(string id)
        {
            List<T> list;
            _bagItemMapById.TryGetValue(id, out list);
            return list;
        }

        public T GetCanStackItemById(string id)
        {
            var list = GetItemsById(id);
            if (list == null) {
                return default(T);
            }

            var findItem = list.Find(item => item.Stackable && item.StackCount < item.StackLimit);
            if (findItem != null) {
                return findItem;
            }

            return list.Find(item => item.Stackable && item.StackCount == item.StackLimit);
        }

        public List<T> GetAllItems()
        {
            return _bagItems;
        }

        #endregion

        public void Sort(Func<T, object> sortByProperty)
        {
            _bagItems = _bagItems.OrderBy(sortByProperty).ToList();
            RefreshItemIndex();
        }

        public List<T> SelectAndOutList(Func<T, bool> filter)
        {
            return _bagItems.Where(filter).ToList();
        }

        public Bag<T> Select(Func<T, bool> filter)
        {
            return new Bag<T>(SelectAndOutList(filter));
        }

        public bool CheckItemPropertyLegal(IBagItem item, Func<IBagItem, IComparable> checkByProperty)
        {
            var property = checkByProperty(item);
            var item2 = GetItemsById(item.Id)[0];
            var property2 = checkByProperty(item2);
            return property.Equals(property2);
        }

        private void RegisterBagItem(T item)
        {
            item.Index = _bagItems.Count;
            _bagItems.Add(item);
            if (_bagItemMapById.ContainsKey(item.Id)) {
                _bagItemMapById[item.Id].Add(item);
            } else {
                _bagItemMapById.Add(item.Id, new List<T> { item });
            }

            OnItemAdd?.Invoke();
        }

        public T this[int index]
        {
            get => GetItemByIndex(index);
            set
            {
                if (index < 0 || index >= Count) {
                    Debug.LogError($"Index: {index} 数组越界");
                } else {
                    _bagItems[index] = value;
                }
            }
        }

        private void RefreshItemIndex()
        {
            for (int i = 0; i < _bagItems.Count; i++) {
                _bagItems[i].Index = i;
            }
        }

        public override string ToString()
        {
            string info = "BagInfo: ";
            for (int i = 0; i < _bagItems.Count; i++) {
                var item = _bagItems[i];
                info += $"Index: {i}, Id: {item.Id}, Name: {item.Name}, Count: {item.StackCount}\n";
            }

            return info;
        }
    }
}