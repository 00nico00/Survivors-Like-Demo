using UnityEngine;

namespace NicoFramework.Modules.BehaviorTree
{
    public interface IGetBehaviorTree
    {
        BehaviorTreeData GetTreeData();
        void SetRoot(BtNodeBase rootData);
    }
}