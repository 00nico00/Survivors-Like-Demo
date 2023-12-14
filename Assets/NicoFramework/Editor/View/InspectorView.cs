using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using NicoFramework.Modules.BehaviorTree;
using UnityEngine;
using UnityEngine.UIElements;

namespace NicoFramework.Editor.View
{
    public class InspectorView : VisualElement
    {
        public IMGUIContainer InspectorBar;
        public InspectorViewDataSO ViewData;

        public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits>
        {
        }

        public InspectorView()
        {
            InspectorBar = new IMGUIContainer() { name = "inspectorBar" };
            InspectorBar.style.flexGrow = 1;
            CreateInspectorView();
            Add(InspectorBar);
        }

        private async void CreateInspectorView()
        {
            var data = await Resources.LoadAsync<InspectorViewDataSO>("InspectorViewDataSO");
            ViewData = (InspectorViewDataSO)data;

            var odinEditor = UnityEditor.Editor.CreateEditor(ViewData);

            InspectorBar.onGUIHandler += () => { odinEditor.OnInspectorGUI(); };
        }

        public void UpdateViewData()
        {
            HashSet<BtNodeBase> data = BehaviorTreeWindow.Instance.treeView.selection
                .OfType<NodeView>()
                .Select(nodeView => nodeView.NodeData)
                .ToHashSet();

            if (ViewData.DataView == null) {
                ViewData.DataView = new HashSet<BtNodeBase>();
            }

            ViewData.DataView.Clear();
            foreach (var node in data) {
                ViewData.DataView.Add(node);
            }
        }
    }
}