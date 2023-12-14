using UnityEngine.UIElements;

namespace NicoFramework.Editor.View
{
    public class SplitView : TwoPaneSplitView
    {
        public new class UxmlFactory : UxmlFactory<SplitView, UxmlTraits>
        {
        }

        public SplitView()
        {
        }
    }
}