using System;
using NicoFramework.Tools.Timer;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NicoFramework.Modules.BehaviorTree
{
    /// <summary>
    /// 顺序节点
    /// </summary>
    public class SequenceNode : BtCompositeNode
    {
        private int _index;

        public override BehaviorState Tick()
        {
            var state = ChildNodes[_index].Tick();
            switch (state) {
                case BehaviorState.Success:
                    _index++;
                    if (_index >= ChildNodes.Count) {
                        // 所有子节点执行成功
                        _index = 0;
                        return BehaviorState.Success;
                    }

                    return BehaviorState.InProgress;
                case BehaviorState.Failure:
                    // 从头开始
                    _index = 0;
                    return BehaviorState.Failure;
                case BehaviorState.InProgress:
                    return state;
            }

            return BehaviorState.NotExecuted;
        }
    }

    /// <summary>
    /// 选择节点
    /// </summary>
    public class SelectorNode : BtCompositeNode
    {
        private int _index;

        public override BehaviorState Tick()
        {
            var state = ChildNodes[_index].Tick();

            switch (state) {
                case BehaviorState.Success:
                    _index = 0;
                    return state;
                case BehaviorState.Failure:
                    _index++;
                    if (_index >= ChildNodes.Count) {
                        // 如果没有子节点能执行成功
                        _index = 0;
                        return BehaviorState.Failure;
                    }

                    return BehaviorState.InProgress;
                default:
                    return state;
            }
        }
    }

    /// <summary>
    /// 延时节点，为了方便观察
    /// </summary>
    public class DelayNode : BtDecoratorNode
    {
        [LabelText("延时时间"), SerializeField, FoldoutGroup("@NodeName")]
        private float _timer;

        private float _currentTimer;

        public override BehaviorState Tick()
        {
            _currentTimer += Time.deltaTime;
            if (_currentTimer >= _timer) {
                _currentTimer = 0f;
                ChildNode.Tick();
                return BehaviorState.Success;
            }

            return BehaviorState.InProgress;
        }
    }

    public class SetObjectActionNode : BtActionNode
    {
        [LabelText("是否启用"), SerializeField, FoldoutGroup("@NodeName")]
        private bool _isActive;

        [LabelText("启用对象"), SerializeField, FoldoutGroup("@NodeName")]
        private GameObject _particle;

        public override BehaviorState Tick()
        {
            _particle.SetActive(_isActive);
            Debug.Log($"{NodeName} 节点" + (_isActive ? "启用了" : "禁用了"));
            return BehaviorState.Success;
        }
    }

    /// <summary>
    /// 一个判断 bool 值的装饰节点
    /// </summary>
    public class SoNode : BtDecoratorNode
    {
        [LabelText("是否活动"), SerializeField, FoldoutGroup("@NodeName")]
        private bool _isActive;

        public override BehaviorState Tick()
        {
            if (_isActive) {
                return ChildNode.Tick();
            }

            return BehaviorState.Failure;
        }
    }
}