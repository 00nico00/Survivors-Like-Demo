using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NicoFramework.Modules.BehaviorTree
{
    [CreateAssetMenu(menuName = "BehaviorTree/InspectorViewDataSO")]
    public class InspectorViewDataSO : SerializedScriptableObject
    {
        // TODO: 之后使用 ReactiveProperty 加一个事件驱动刷新行为树页面修改名字
        public HashSet<BtNodeBase> DataView = new HashSet<BtNodeBase>();
    }
}