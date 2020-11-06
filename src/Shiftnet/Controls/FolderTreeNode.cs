using AlkalineThunder.Pandemic.Gui;
using AlkalineThunder.Pandemic.Gui.Controls;
using AlkalineThunder.Pandemic.Input;
using AlkalineThunder.Pandemic.Rendering;
using Microsoft.Xna.Framework;

namespace Shiftnet.Controls
{
    public class FolderTreeNode : Control
    {
        private StackPanel _outerStack = new StackPanel();
        private StackPanel _innerStack = new StackPanel();
        private Button _node = new Button();

        public bool IsExpanded
        {
            get => _innerStack.Visible;
            set => _innerStack.Visible = true;
        }
        
        public FolderTreeNode()
        {
            _outerStack.AddChild(_node);
            _outerStack.AddChild(_innerStack);

            _outerStack.Spacing = 5;
            _innerStack.Spacing = 5;

            _innerStack.Padding = new Padding(15, 0, 0, 0);
            
            _node.Click += NodeOnClick;
            
            InternalChildren.Add(_outerStack);
        }

        private void NodeOnClick(object? sender, MouseButtonEventArgs e)
        {
            IsExpanded = !IsExpanded;
        }

        protected override Vector2 MeasureOverride(Vector2 alottedSize)
        {
            return _outerStack.Measure(null, alottedSize);
        }

        protected override void Arrange(Rectangle bounds)
        {
            _outerStack.Layout(bounds);
        }

        public void Expand()
        {
            _innerStack.Visible = true;
        }

        public void Collapse()
        {
            _innerStack.Visible = false;
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            _node.IsActive = _innerStack.Visible;
        }
    }
}