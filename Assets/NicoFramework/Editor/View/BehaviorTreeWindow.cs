using System;
using System.Collections.Generic;
using System.Linq;
using NicoFramework.Editor.EditorToolEx;
using NicoFramework.Modules.BehaviorTree;
using Sirenix.Utilities;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace NicoFramework.Editor.View
{
    public class BehaviorTreeWindow : EditorWindow
    {
        public static BehaviorTreeWindow Instance { get; private set; }

        public TreeView treeView;
        public InspectorView inspectorView;

        [MenuItem("Window/UI Toolkit/BehaviorTreeWindow")]
        public static void ShowExample()
        {
            BehaviorTreeWindow wnd = GetWindow<BehaviorTreeWindow>("BehaviorTreeWindow");
        }

        private void OnDestroy()
        {
            Save();
        }

        public void CreateGUI()
        {
            InitWindow();
        }

        public void InitWindow()
        {
            var id = BtSettingSO.GetSetting().TreeID;
            var iGetBehaviorTree = EditorUtility.InstanceIDToObject(id) as IGetBehaviorTree;

            Instance = this;
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;
            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/NicoFramework/Editor/View/BehaviorTreeWindow.uxml");
            var styleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/NicoFramework/Editor/View/BehaviorTreeWindow.uss");

            visualTree.CloneTree(root);

            // 获取 TreeView 界面
            treeView = root.Q<TreeView>();
            // 获取 InspectorView 界面
            inspectorView = root.Q<InspectorView>();

            if (iGetBehaviorTree == null || iGetBehaviorTree.GetTreeData() == null ||
                iGetBehaviorTree.GetTreeData().RootNode == null) {
                return;
            }

            // 创建行为树视图
            CreateRoot(iGetBehaviorTree.GetTreeData().RootNode);
            // 点与点之间连线
            treeView.nodes.OfType<NodeView>().ForEach(node => node.LinkLines());

            // 加载 treeView 视图定位信息
            LoadTreeViewTransform();
        }

        /// <summary>
        /// 保存当前行为树界面
        /// </summary>
        public void Save()
        {
            // 保存当前在行为树窗口中的位置
            var viewTransform = BtSettingSO.GetSetting().GetTree().GetTreeData().ViewTransform;
            viewTransform.position = treeView.viewTransform.position;
            viewTransform.scale = treeView.viewTransform.scale;

            // 保存当前的场景即可
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        }

        /// <summary>
        /// 加载保存的视图位置信息，如果没有则新建
        /// </summary>
        public void LoadTreeViewTransform()
        {
            var treeData = BtSettingSO.GetSetting().GetTree().GetTreeData();
            if (treeData.ViewTransform == null) {
                treeData.ViewTransform = new GraphViewTransform();
            } else {
                treeView.viewTransform.position = treeData.ViewTransform.position;
                treeView.viewTransform.scale = treeData.ViewTransform.scale;
            }
        }

        /// <summary>
        /// 通过根节点创建树
        /// </summary>
        /// <param name="rootNode"></param>
        public void CreateRoot(BtNodeBase rootNode)
        {
            if (rootNode == null) {
                return;
            }

            NodeView nodeView = new NodeView(rootNode);
            if (rootNode.Guid == null) {
                rootNode.Guid = System.Guid.NewGuid().ToString();
            }

            treeView.GuidMapNodeView.Add(nodeView.NodeData.Guid, nodeView);
            nodeView.SetPosition(new Rect(rootNode.Position, Vector2.one));
            treeView.AddElement(nodeView);

            // 此处根节点需要特殊处理


            switch (rootNode) {
                case BtCompositeNode compositeNode:
                    compositeNode.ChildNodes.ForEach(CreateChild);
                    break;
                case BtDecoratorNode decoratorNode:
                    CreateChild(decoratorNode.ChildNode);
                    break;
            }
        }

        public void CreateChild(BtNodeBase nodeData)
        {
            if (nodeData == null) {
                return;
            }

            NodeView nodeView = new NodeView(nodeData);
            if (nodeData.Guid == null) {
                nodeData.Guid = System.Guid.NewGuid().ToString();
            }

            treeView.GuidMapNodeView.Add(nodeView.NodeData.Guid, nodeView);
            nodeView.SetPosition(new Rect(nodeData.Position, Vector2.one));
            treeView.AddElement(nodeView);

            switch (nodeData) {
                case BtCompositeNode compositeNode:
                    compositeNode.ChildNodes.ForEach(CreateChild);
                    break;
                case BtDecoratorNode decoratorNode:
                    CreateChild(decoratorNode.ChildNode);
                    break;
            }
        }
    }

    public class RightClickMenu : ScriptableObject, ISearchWindowProvider
    {
        public delegate bool SelectEntryDelegate(SearchTreeEntry searchTreeEntry, SearchWindowContext context);

        public SelectEntryDelegate OnSelectEntryHandler;

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var entries = new List<SearchTreeEntry>(); // 搜索树条目
            entries.Add(new SearchTreeGroupEntry(new GUIContent("Create Node")));
            entries = AddNodeType<BtCompositeNode>(entries, "组合节点");
            entries = AddNodeType<BtDecoratorNode>(entries, "修饰节点");
            entries = AddNodeType<BtActionNode>(entries, "行为节点");

            return entries;
        }

        /// <summary>
        /// 通过反射添加节点数据到菜单
        /// </summary>
        /// <param name="entries"></param>
        /// <param name="pathName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<SearchTreeEntry> AddNodeType<T>(List<SearchTreeEntry> entries, string pathName)
        {
            entries.Add(new SearchTreeGroupEntry(new GUIContent(pathName)) { level = 1 });
            List<Type> rootNodeTypes = typeof(T).GetDerivedClasses();
            foreach (var rootType in rootNodeTypes) {
                var menuName = rootType.Name;
                entries.Add(new SearchTreeEntry(new GUIContent(menuName)) { level = 2, userData = rootType });
            }

            return entries;
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            if (OnSelectEntryHandler == null) {
                return false;
            }

            return OnSelectEntryHandler(searchTreeEntry, context);
        }
    }
}