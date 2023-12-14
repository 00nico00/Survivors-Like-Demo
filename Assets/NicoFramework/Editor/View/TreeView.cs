using System;
using System.Collections.Generic;
using System.Linq;
using NicoFramework.Modules.BehaviorTree;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace NicoFramework.Editor.View
{
    public class TreeView : GraphView
    {
        // Guid 映射到节点
        public Dictionary<string, NodeView> GuidMapNodeView = new Dictionary<string, NodeView>();

        // 存储当前被复制的结点
        private List<BtNodeBase> copyNodes = new List<BtNodeBase>();

        public new class UxmlFactory : UxmlFactory<TreeView, UxmlTraits>
        {
        }

        public TreeView()
        {
            Insert(0, new GridBackground());

            // 关联操纵器
            this.AddManipulator(new ContentZoomer()); // 缩放
            this.AddManipulator(new ContentDragger()); // 拖拽背景
            this.AddManipulator(new SelectionDragger()); // 允许鼠标选择拖动一个或多个元素
            this.AddManipulator(new RectangleSelector()); // 处理矩形选择框

            styleSheets.Add(
                AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/NicoFramework/Editor/View/BehaviorTreeWindow.uss"));
            GraphViewMenu();

            // 添加可视化节点，线改变的回调
            graphViewChanged += OnGraphViewChanged;
            // 鼠标点击的回调
            RegisterCallback<MouseEnterEvent>(MouseEventControl);
            // 键盘事件的回调
            RegisterCallback<KeyDownEvent>(KeyDownEventCallback);
        }

        private void KeyDownEventCallback(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.Tab) {
                evt.StopPropagation();
            }

            if (!evt.ctrlKey) {
                return;
            }

            switch (evt.keyCode) {
                case KeyCode.S:
                    BehaviorTreeWindow.Instance.Save();
                    evt.StopPropagation(); // 防止消息向下传递
                    break;
                case KeyCode.E:
                    // OnStartMove();
                    evt.StopPropagation();
                    break;
                case KeyCode.X:
                    evt.StopPropagation();
                    break;
                case KeyCode.C:
                    Copy();
                    evt.StopPropagation();
                    break;
                case KeyCode.V:
                    Paste();
                    evt.StopPropagation();
                    break;
            }
        }

        private void Copy()
        {
            copyNodes = selection
                .OfType<NodeView>()
                .Select(node => node.NodeData)
                .ToList()
                .CloneData();
        }

        private void Paste()
        {
            var pasteNodes = new List<NodeView>();
            for (int i = 0; i < copyNodes.Count; i++) {
                var nodeView = new NodeView(copyNodes[i]);
                nodeView.SetPosition(new Rect(copyNodes[i].Position, Vector2.one));
                GuidMapNodeView.Add(nodeView.NodeData.Guid, nodeView);
                AddElement(nodeView);
                pasteNodes.Add(nodeView);
            }

            pasteNodes.ForEach(node => node.LinkLines());
            // 刷新复制的节点数据
            copyNodes = copyNodes.CloneData();
        }

        private void MouseEventControl(MouseEnterEvent evt)
        {
            BehaviorTreeWindow.Instance.inspectorView.UpdateViewData();
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange change)
        {
            // log
            foreach (var (key, _) in GuidMapNodeView) {
                UnityEngine.Debug.Log(key);
            }

            // 在 treeView 中添加或删除边时添加对应的数据
            if (change.edgesToCreate != null) {
                change.edgesToCreate.ForEach(edge => { edge.AddDataOnLinkLine(); });
            }

            if (change.elementsToRemove != null) {
                change.elementsToRemove.ForEach(elem =>
                {
                    if (elem is Edge edge) {
                        edge.RemoveDataOnUnLinkLine();
                    }

                    // 删除节点之后也在 GuidMapNodeView 中清除相应的 Guid
                    if (elem is NodeView nodeView) {
                        GuidMapNodeView.Remove(nodeView.NodeData.Guid);
                    }
                });
            }

            return change;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);
            // evt.menu.AppendAction("Create Group", CreatNode);
        }

        private void CreatNode(Type type, Vector2 position)
        {
            BtNodeBase nodeData = Activator.CreateInstance(type) as BtNodeBase;
            nodeData.Guid = System.Guid.NewGuid().ToString(); // 生成唯一标识
            nodeData.NodeName = type.Name;

            NodeView node = new NodeView(nodeData);
            GuidMapNodeView.Add(node.NodeData.Guid, node);
            node.SetPosition(new Rect(position, Vector2.one));
            AddElement(node);
        }

        #region 右键菜单

        public void GraphViewMenu()
        {
            var menuWindowProvider = ScriptableObject.CreateInstance<RightClickMenu>();
            menuWindowProvider.OnSelectEntryHandler = OnMenuSelectEntry;

            nodeCreationRequest += context =>
            {
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), menuWindowProvider);
            };
        }

        private bool OnMenuSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            var windowRoot = BehaviorTreeWindow.Instance.rootVisualElement;
            // context.screenMousePosition - BehaviorTreeWindow.Instance.position.position
            // 此处是为了使得这个鼠标的位置是相对于 BehaviorTreeWindow 左上角的
            var windowMousePosition = windowRoot.ChangeCoordinatesTo(windowRoot.parent,
                context.screenMousePosition - BehaviorTreeWindow.Instance.position.position);
            var graphMousePosition = contentViewContainer.WorldToLocal(windowMousePosition);

            CreatNode((Type)searchTreeEntry.userData, graphMousePosition);

            return true;
        }

        #endregion

        // 定义顶点连接规则
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            // 两连接点方向不能相同，不能为同一个顶点
            return ports.Where(endPorts =>
                endPorts.direction != startPort.direction &&
                endPorts.node != startPort.node).ToList();
        }
    }
}