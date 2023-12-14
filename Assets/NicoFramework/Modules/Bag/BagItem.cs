namespace NicoFramework.Modules.Bag
{
    public enum BagItemType
    {
        Weapon,
        Coin,
        Sword,
        Shield,
        Bow,
        Gun,
        Pao,
        Dao
    }

    public class BagItem : IBagItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Index { get; set; }
        public bool Stackable { get; set; }
        public int StackLimit { get; set; }
        public int StackCount { get; set; }
        public BagItemType BagItemType { get; set; }
        public int Grade { get; set; }
        public int Star { get; set; }

        public BagItem(string id, string name, BagItemType itemType, int stackCount, int grade, int star)
        {
            Id = id;
            Name = name;
            BagItemType = itemType;
            Grade = grade;
            Star = star;
            if (stackCount > 0) {
                Stackable = true;
                StackLimit = 99;
                StackCount = stackCount;
            }
        }

        public BagItem(BagItem item)
        {
            Id = item.Id;
            Name = item.Name;
            BagItemType = item.BagItemType;
            Stackable = item.Stackable;
            StackLimit = item.StackLimit;
            StackCount = item.StackCount;
            Grade = item.Grade;
            Star = item.Star;
        }


        public bool CheckLegal<T>(IBag<T> bag) where T : IBagItem
        {
            bool result = true;
            // 检查可堆叠属性是否相同
            result = bag.CheckItemPropertyLegal(this, item => item.Stackable) && result;
            // 检查其他属性是否相同
            result = bag.CheckItemPropertyLegal(this, item => item.Name) && result;
            result = bag.CheckItemPropertyLegal(this, item => ((BagItem)item).BagItemType) && result;
            result = bag.CheckItemPropertyLegal(this, item => ((BagItem)item).Grade) && result;
            result = bag.CheckItemPropertyLegal(this, item => ((BagItem)item).Star) && result;
            return result;
        }

        public bool Equals(IBagItem other)
        {
            return other != null && Id == other.Id;
        }
    }
}