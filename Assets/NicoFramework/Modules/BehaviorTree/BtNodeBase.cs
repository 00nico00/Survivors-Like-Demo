using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

namespace NicoFramework.Modules.BehaviorTree
{
    /// <summary>
    /// 保存行为树窗口上一次打开的位置
    /// </summary>
    public class GraphViewTransform : ITransform
    {
        public Vector3 position { get; set; }
        public Quaternion rotation { get; set; }
        public Vector3 scale { get; set; }
        public Matrix4x4 matrix { get; }
    }

    [BoxGroup]
    [HideReferenceObjectPicker]
    public class BehaviorTreeData
    {
        public BtNodeBase RootNode;
        public GraphViewTransform ViewTransform = new GraphViewTransform();
    }

    public enum BehaviorState
    {
        NotExecuted, // 未执行
        Success, // 成功
        Failure, // 失败
        InProgress // 执行中
    }

    [BoxGroup]
    [HideReferenceObjectPicker]
    public abstract class BtNodeBase
    {
        [FoldoutGroup("@NodeName"), LabelText("唯一标识")]
        public string Guid;

        [FoldoutGroup("@NodeName"), LabelText("节点位置")]
        public Vector2 Position;

        [FoldoutGroup("@NodeName"), LabelText("名称")]
        public string NodeName;

        public abstract BehaviorState Tick();
    }

    /// <summary>
    /// 组合节点
    /// </summary>
    public abstract class BtCompositeNode : BtNodeBase
    {
        [FoldoutGroup("@NodeName"), LabelText("子节点")]
        public List<BtNodeBase> ChildNodes = new List<BtNodeBase>();
    }

    /// <summary>
    /// 条件节点
    /// </summary>
    public abstract class BtDecoratorNode : BtNodeBase
    {
        [FoldoutGroup("@NodeName"), LabelText("子节点")]
        public BtNodeBase ChildNode;
    }

    /// <summary>
    /// 行为节点
    /// </summary>
    public abstract class BtActionNode : BtNodeBase
    {
    }

    public abstract class BtConditionNode : BtNodeBase
    {
    }
}