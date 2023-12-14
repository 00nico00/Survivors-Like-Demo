using System.Collections.Generic;
using System.Linq;
using NicoFramework.Editor.View;
using NicoFramework.Modules.BehaviorTree;
using Sirenix.Serialization;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public static class BtExtension
{
    /// <summary>
    /// 在结点连线的时候添加进输入到 NodeView
    /// </summary>
    /// <param name="edge"></param>
    public static void AddDataOnLinkLine(this Edge edge)
    {
        NodeView inputNode = edge.input.node as NodeView;
        NodeView outputNode = edge.output.node as NodeView;
        switch (outputNode.NodeData) {
            case BtCompositeNode compositeNode:
                compositeNode.ChildNodes.Add(inputNode.NodeData);
                break;
            case BtDecoratorNode decoratorNode:
                decoratorNode.ChildNode = inputNode.NodeData;
                break;
        }
    }

    /// <summary>
    /// 在结点断开的时候删除数据从 NodeView
    /// </summary>
    /// <param name="edge"></param>
    public static void RemoveDataOnUnLinkLine(this Edge edge)
    {
        NodeView inputNode = edge.input.node as NodeView;
        NodeView outputNode = edge.output.node as NodeView;
        switch (outputNode.NodeData) {
            case BtCompositeNode compositeNode:
                compositeNode.ChildNodes.Remove(inputNode.NodeData);
                break;
            case BtDecoratorNode decoratorNode:
                decoratorNode.ChildNode = null;
                break;
        }
    }

    /// <summary>
    /// 用 odin 序列化去克隆结点
    /// </summary>
    /// <param name="nodes"></param>
    /// <returns></returns>
    public static List<BtNodeBase> CloneData(this List<BtNodeBase> nodes)
    {
        byte[] nodeBytes = SerializationUtility.SerializeValue(nodes, DataFormat.Binary);
        var toNodes = SerializationUtility.DeserializeValue<List<BtNodeBase>>(nodeBytes, DataFormat.Binary);

        //  删掉未复制的子节点数据，并随机产生新的 Guid
        for (int i = 0; i < toNodes.Count; i++) {
            toNodes[i].Guid = System.Guid.NewGuid().ToString();
            switch (toNodes[i]) {
                case BtCompositeNode compositeNode:
                    // 排除 toNodes 没有但是节点子集却含有的子节点
                    compositeNode.ChildNodes = compositeNode.ChildNodes.Intersect(toNodes).ToList();
                    break;
                case BtDecoratorNode decoratorNode:
                    if (decoratorNode.ChildNode != null && !toNodes.Contains(decoratorNode.ChildNode)) {
                        decoratorNode.ChildNode = null;
                    }

                    break;
            }

            // 位置向右下偏移
            toNodes[i].Position += Vector2.one * 30;
        }

        return toNodes;
    }
}