using System;
using System.Collections.Generic;

namespace NicoFramework.Modules.Bag
{
    public interface IBag<T> where T : IBagItem
    {
        // Action
        event Action OnItemAdd;
        event Action OnItemRemove;

        event Action OnItemTakeOut;

        // ...
        int Count { get; }
        int MaxCapacity { get; set; }
        void Add(T item);
        void TakeOut(int index, int takeoutCount);
        void RemoveById(string id);
        void RemoveByIndex(int index);
        void Clear();
        T GetItemByIndex(int index);
        List<T> GetItemsById(string id);

        /// <summary>
        /// 返回第一个 相同 ID 未满 StackLimit 的可堆叠物品，如果都满则任意返回
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T GetCanStackItemById(string id);

        List<T> GetAllItems();
        void Sort(Func<T, object> sortByProperty);
        Bag<T> Select(Func<T, bool> filter);
        bool CheckItemPropertyLegal(IBagItem item, Func<IBagItem, IComparable> checkByProperty);
    }
}