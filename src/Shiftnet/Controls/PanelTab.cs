using System;
using AlkalineThunder.Pandemic;
using AlkalineThunder.Pandemic.Gui;
using AlkalineThunder.Pandemic.Gui.Controls;
using AlkalineThunder.Pandemic.Input;
using Microsoft.Xna.Framework;

namespace Shiftnet.Controls
{
    public class PanelTab : Control
    {
        private Button _button;
        private TextBlock _titleText;
        private Icon _windowIcon;
        private StackPanel _buttonContent;
        private Button _closeButton;

        
        public string Title
        {
            get => _titleText.Text;
            set => _titleText.Text = value;
        }

        public bool IsActive
        {
            get => _button.IsActive;
            set => _button.IsActive = value;
        }

        public PanelTab(bool closeable)
        {
            _button = new Button();
            _titleText = new TextBlock();
            _windowIcon = new Icon();
            _closeButton = new Button();

            _closeButton.StyleProperties.SetValue("bgColor", "#00000000");

            _buttonContent = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 3,
                Padding = 3
            };

            _button.Content = _buttonContent;
            _buttonContent.AddChild(_windowIcon);
            _buttonContent.AddChild(_titleText);

            if (closeable)
                _buttonContent.AddChild(_closeButton);
            
            _closeButton.Click += CloseButtonOnClick;
            
            InternalChildren.Add(_button);

            _closeButton.Content = new Icon
            {
                FixedWidth = 16,
                FixedHeight = 16,
                Image = GameLoop.LoadTexture("textures/close")
            };
        }

        private void CloseButtonOnClick(object? sender, MouseButtonEventArgs e)
        {
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }

        protected override Vector2 MeasureOverride(Vector2 alottedSize)
        {
            return _button.Measure(null, alottedSize);
        }

        protected override void Arrange(Rectangle bounds)
        {
            _button.Layout(bounds);
        }

        public event EventHandler CloseRequested;
    }
}