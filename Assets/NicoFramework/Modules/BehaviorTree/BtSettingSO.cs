using UnityEngine;

namespace NicoFramework.Modules.BehaviorTree
{
    [CreateAssetMenu(menuName = "BehaviorTree/BtSettingSO")]
    public class BtSettingSO : ScriptableObject
    {
        public int TreeID;

        public static BtSettingSO GetSetting()
        {
            return Resources.Load<BtSettingSO>("BtSettingSO");
        }

#if UNITY_EDITOR
        public IGetBehaviorTree GetTree() => UnityEditor.EditorUtility.InstanceIDToObject(TreeID) as IGetBehaviorTree;
        public void SetRoot(BtNodeBase rootData) => GetTree().SetRoot(rootData);
#endif
    }
}