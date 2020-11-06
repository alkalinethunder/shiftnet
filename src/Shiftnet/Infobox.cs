using System;
using System.Threading;
using AlkalineThunder.Pandemic.Gui.Controls;
using AlkalineThunder.Pandemic.Gui.Markup;
using AlkalineThunder.Pandemic.Input;
using AlkalineThunder.Pandemic.Windowing;

namespace Shiftnet
{
    public sealed class Infobox : Window
    {
        private TextBlock _message;
        private bool _closing;
        private bool _dismissed;
        private InfoboxButtons _buttons;
        private Button _ok;
        private Button _cancel;
        private Button _yes;
        private Button _no;
        private TextInput _textEntry;
        
        public string TextError { get; private set; }

        public bool TextPromptEnabled
        {
            get => _textEntry.Visible;
            set => _textEntry.Visible = value;
        }
        
        public InfoboxTextCallback TextCallback { get; set; }
        public InfoboxValidator[] TextValidators { get; set; }
        
        public InfoboxButtons Buttons
        {
            get => _buttons;
            set
            {
                if (_buttons != value)
                {
                    _buttons = value;
                    UpdateButtons();
                }
            }
        }
        
        public event EventHandler<InfoboxDismissedEventArgs> Dismissed;
        
        public string Message
        {
            get => _message.Text;
            set => _message.Text = value;
        }
        
        protected override void OnInitialize()
        {
            Gui.AddChild(GuiBuilder.Build(this, "layout/infobox.gui"));
            _message = Gui.FindById<TextBlock>("message");
            base.OnInitialize();
            Closed += OnClosed;

            _textEntry = Gui.FindById<TextInput>("textEntry");
            
            _ok = Gui.FindById<Button>("ok");
            _yes = Gui.FindById<Button>("yes");
            _no = Gui.FindById<Button>("no");
            _cancel = Gui.FindById<Button>("cancel");

            _ok.Click += OkOnClick;
            _yes.Click += YesOnClick;
            _no.Click += NoOnClick;
            _cancel.Click += CancelOnClick;

            _textEntry.HintText = "Enter text...";
            
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            _ok.Visible = false;
            _yes.Visible = false;
            _no.Visible = false;
            _cancel.Visible = false;

            switch (_buttons)
            {
                case InfoboxButtons.OK:
                    _ok.Visible = true;
                    break;
                case InfoboxButtons.YesNo:
                    _yes.Visible = true;
                    _no.Visible = true;
                    break;
                case InfoboxButtons.YesNoCancel:
                    goto case InfoboxButtons.YesNo;
                    _cancel.Visible = true;
                    break;
                case InfoboxButtons.OKCancel:
                    goto case InfoboxButtons.OK;
                    _cancel.Visible = true;
                    break;
            }
        }
        
        private void CancelOnClick(object? sender, MouseButtonEventArgs e)
        {
            Dismiss(InfoboxResult.Cancel);
        }

        private void NoOnClick(object? sender, MouseButtonEventArgs e)
        {
            Dismiss(InfoboxResult.No);
        }

        private void YesOnClick(object? sender, MouseButtonEventArgs e)
        {
            Dismiss(InfoboxResult.Yes);
        }

        private void OkOnClick(object sender, MouseButtonEventArgs e)
        {
            Dismiss(InfoboxResult.OK);
        }

        private void OnClosed(object? sender, EventArgs e)
        {
            _closing = true;
            Dismiss(InfoboxResult.Cancel);
        }

        
        private void Dismiss(InfoboxResult result)
        {
            if (!_dismissed)
            {
                if (TextPromptEnabled)
                {
                    if (result == InfoboxResult.OK)
                    {
                        var text = _textEntry.Text;
                        if (TextValidators != null)
                        {
                            foreach (var validator in TextValidators)
                            {
                                if (!validator(text, out var error))
                                {
                                    TextError = error;
                                    return;
                                }
                            }
                        }

                        TextCallback?.Invoke(result, text);
                    }
                    else
                    {
                        TextCallback?.Invoke(result, string.Empty);
                    }
                }
                
                _dismissed = true;
                Dismissed?.Invoke(this, new InfoboxDismissedEventArgs(result));
                if (!_closing)
                {
                    Close();
                }
            }
        }
    }
}
