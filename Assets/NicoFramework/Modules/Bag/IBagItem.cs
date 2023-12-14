using System;

namespace NicoFramework.Modules.Bag
{
    public interface IBagItem : IEquatable<IBagItem>
    {
        string Id { get; set; }
        string Name { get; set; }
        int Index { get; set; }
        bool Stackable { get; set; }
        int StackLimit { get; set; }
        int StackCount { get; set; }

        /// <summary>
        /// 根据自身属性检查是否符合规范
        /// </summary>
        /// <param name="bag"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        bool CheckLegal<T>(IBag<T> bag) where T : IBagItem;
    }
}