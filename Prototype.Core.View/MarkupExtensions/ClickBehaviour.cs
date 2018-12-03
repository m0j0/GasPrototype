using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using MugenMvvmToolkit;

namespace Prototype.Core.Controls
{
    // inspired by https://referencesource.microsoft.com/#PresentationFramework/src/Framework/System/Windows/Controls/Primitives/ButtonBase.cs
    // NOTE Access keys aren't supported
    internal class ClickBehaviourCore
    {
        private readonly Action<ClickBehaviourCore, FrameworkElement> _subscribe;
        private readonly Action<ClickBehaviourCore, FrameworkElement> _unsubscribe;
        private readonly Func<DependencyObject, ICommand> _getCommand;
        private readonly Func<DependencyObject, object> _getCommandParameter;
        private readonly Func<DependencyObject, Action> _getAction;

        #region Fields

        private static readonly ConditionalWeakTable<FrameworkElement, EventHandler> CommandsHandlers =
            new ConditionalWeakTable<FrameworkElement, EventHandler>();

        #endregion

        public ClickBehaviourCore(Action<ClickBehaviourCore, FrameworkElement> subscribe, 
            Action<ClickBehaviourCore, FrameworkElement> unsubscribe,
            Func<DependencyObject, ICommand> getCommand,
            Func<DependencyObject, object> getCommandParameter,
            Func<DependencyObject, Action> getAction
            )
        {
            _subscribe = subscribe;
            _unsubscribe = unsubscribe;
            _getCommand = getCommand;
            _getCommandParameter = getCommandParameter;
            _getAction = getAction;
        }

        #region Attached properties

        #region ClickBehaviourCore

        public static readonly DependencyProperty ClickBehaviourProperty = DependencyProperty.RegisterAttached(
            "ClickBehaviour", typeof(ClickBehaviourCore), typeof(ClickBehaviourCore), new PropertyMetadata(null));

        public static void SetClickBehaviour(DependencyObject element, ClickBehaviourCore value)
        {
            element.SetValue(ClickBehaviourProperty, value);
        }

        public static ClickBehaviourCore GetClickBehaviour(DependencyObject element)
        {
            return (ClickBehaviourCore) element.GetValue(ClickBehaviourProperty);
        }
        
        #endregion

        #region IsPressed

        private static readonly DependencyProperty IsPressedProperty = DependencyProperty.RegisterAttached(
            "IsPressed", typeof(bool), typeof(ClickBehaviour), new PropertyMetadata(default(bool)));

        private static void SetIsPressed(DependencyObject element, bool value)
        {
            element.SetValue(IsPressedProperty, Empty.BooleanToObject(value));
        }

        private static bool GetIsPressed(DependencyObject element)
        {
            return (bool) element.GetValue(IsPressedProperty);
        }

        #endregion

        #region Command
        
        public void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = (FrameworkElement) d;
            VerifyElement(element);

            if (e.OldValue is ICommand oldCommand)
            {
                _unsubscribe(this, element);

                if (CommandsHandlers.TryGetValue(element, out var handler))
                {
                    oldCommand.CanExecuteChanged -= handler;
                    CommandsHandlers.Remove(element);
                }
            }

            if (e.NewValue is ICommand newCommand)
            {
                _subscribe(this, element);

                var handler = ReflectionExtensions.CreateWeakDelegate<FrameworkElement, EventArgs, EventHandler>(
                    element,
                    (el, o, arg3) => UpdateCanExecute(el),
                    (o, eventHandler) =>
                    {
                        if (o is ICommand command)
                        {
                            command.CanExecuteChanged -= eventHandler;
                        }
                    },
                    eventHandler => eventHandler.Handle);

                CommandsHandlers.Add(element, handler);
                newCommand.CanExecuteChanged += handler;
            }

            UpdateCanExecute(element);
        }

        private void UpdateCanExecute(FrameworkElement element)
        {
            var command = _getCommand(element);
            element.IsEnabled = command == null || command.CanExecute(_getCommandParameter(element));
        }

        #endregion

        #region Action

        public void OnActionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = (FrameworkElement) d;
            VerifyElement(element);

            if (e.OldValue != null)
            {
                _unsubscribe(this, element);
            }

            if (e.NewValue != null)
            {
                _subscribe(this, element);
            }
        }

        #endregion

        #region IsSpaceKeyDown

        private static readonly DependencyProperty IsSpaceKeyDownProperty = DependencyProperty.RegisterAttached(
            "IsSpaceKeyDown", typeof(bool), typeof(ClickBehaviour), new PropertyMetadata(false));

        private static void SetIsSpaceKeyDown(DependencyObject element, bool value)
        {
            element.SetValue(IsSpaceKeyDownProperty, Empty.BooleanToObject(value));
        }

        private static bool GetIsSpaceKeyDown(DependencyObject element)
        {
            return (bool) element.GetValue(IsSpaceKeyDownProperty);
        }

        #endregion

        private void VerifyElement(FrameworkElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (_getCommand(element) != null &&
                _getAction(element) != null)
            {
                throw new InvalidOperationException("Can't set both Command and Action at the same time!");
            }
        }

        #endregion

        #region Methods



        private void OnClick(FrameworkElement element)
        {
            var command = _getCommand(element);
            if (command != null)
            {
                if (command.CanExecute(_getCommandParameter(element)))
                {
                    command.Execute(_getCommandParameter(element));
                }
            }

            var action = _getAction(element);
            action?.Invoke();
        }

        public void OnRenderSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            if (!(sender is FrameworkElement element))
            {
                return;
            }

            if (element.IsMouseCaptured && (Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed) && !GetIsSpaceKeyDown(element))
            {
                UpdateIsPressed(element);
            }
        }

        private static bool GetMouseLeftButtonReleased()
        {
            return InputManager.Current.PrimaryMouseDevice.LeftButton == MouseButtonState.Released;
        }

        private static bool GetIsInMainFocusScope(FrameworkElement element)
        {
            Visual focusScope = FocusManager.GetFocusScope(element) as Visual;
            return focusScope == null || VisualTreeHelper.GetParent(focusScope) == null;
        }

        private static void UpdateIsPressed(FrameworkElement element)
        {
            Point pos = Mouse.PrimaryDevice.GetPosition(element);

            if (pos.X >= 0 && (pos.X <= element.ActualWidth) && pos.Y >= 0 && pos.Y <= element.ActualHeight)
            {
                if (!GetIsPressed(element))
                {
                    SetIsPressed(element, true);
                }
            }
            else if (GetIsPressed(element))
            {
                SetIsPressed(element, false);
            }
        }

        public void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is FrameworkElement element))
            {
                return;
            }

            e.Handled = true;

            element.Focus();
            if (e.ButtonState != MouseButtonState.Pressed)
            {
                return;
            }

            element.CaptureMouse();
            if (!element.IsMouseCaptured)
            {
                return;
            }

            if (e.ButtonState == MouseButtonState.Pressed)
            {
                if (!GetIsPressed(element))
                {
                    SetIsPressed(element, true);
                }
            }
            else
            {
                element.ReleaseMouseCapture();
            }
        }

        public void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is FrameworkElement element))
            {
                return;
            }

            e.Handled = true;
            bool shouldClick = !GetIsSpaceKeyDown(element) && GetIsPressed(element);

            if (element.IsMouseCaptured && !GetIsSpaceKeyDown(element))
            {
                element.ReleaseMouseCapture();
            }

            if (shouldClick)
            {
                OnClick(element);
            }
        }

        public void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!(sender is FrameworkElement element))
            {
                return;
            }

            if (element.IsMouseCaptured && Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed && !GetIsSpaceKeyDown(element))
            {
                UpdateIsPressed(element);

                e.Handled = true;
            }
        }

        public void OnLostMouseCapture(object sender, MouseEventArgs e)
        {
            if (!(sender is FrameworkElement element))
            {
                return;
            }

            if ((e.OriginalSource != element) || GetIsSpaceKeyDown(element))
            {
                return;
            }

            if (element.IsKeyboardFocused && !GetIsInMainFocusScope(element))
            {
                Keyboard.Focus(null);
            }

            SetIsPressed(element, false);
        }

        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!(sender is FrameworkElement element))
            {
                return;
            }

            if (e.Key == Key.Space)
            {
                if ((Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Alt)) != ModifierKeys.Alt)
                {
                    if ((!element.IsMouseCaptured) && (e.OriginalSource == element))
                    {
                        SetIsSpaceKeyDown(element, true);
                        SetIsPressed(element, true);
                        element.CaptureMouse();

                        e.Handled = true;
                    }
                }
            }
            else if (e.Key == Key.Enter && (bool) element.GetValue(KeyboardNavigation.AcceptsReturnProperty))
            {
                if (e.OriginalSource == element)
                {
                    SetIsSpaceKeyDown(element, false);
                    SetIsPressed(element, false);
                    if (element.IsMouseCaptured)
                    {
                        element.ReleaseMouseCapture();
                    }

                    OnClick(element);
                    e.Handled = true;
                }
            }
            else
            {
                if (GetIsSpaceKeyDown(element))
                {
                    SetIsPressed(element, false);
                    SetIsSpaceKeyDown(element, false);
                    if (element.IsMouseCaptured)
                    {
                        element.ReleaseMouseCapture();
                    }
                }
            }
        }

        public void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (!(sender is FrameworkElement element))
            {
                return;
            }

            if (e.Key == Key.Space && GetIsSpaceKeyDown(element))
            {
                if ((Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Alt)) != ModifierKeys.Alt)
                {
                    SetIsSpaceKeyDown(element, false);
                    if (GetMouseLeftButtonReleased())
                    {
                        bool shouldClick = GetIsPressed(element);

                        if (element.IsMouseCaptured)
                        {
                            element.ReleaseMouseCapture();
                        }

                        if (shouldClick)
                        {
                            OnClick(element);
                        }
                    }
                    else
                    {
                        if (element.IsMouseCaptured)
                        {
                            UpdateIsPressed(element);
                        }
                    }

                    e.Handled = true;
                }
            }
        }

        public void OnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!(sender is FrameworkElement element))
            {
                return;
            }

            if (e.OriginalSource == element)
            {
                if (GetIsPressed(element))
                {
                    SetIsPressed(element, false);
                }

                if (element.IsMouseCaptured)
                {
                    element.ReleaseMouseCapture();
                }

                SetIsSpaceKeyDown(element, false);
            }
        }

        #endregion
    }

    public static class ClickBehaviour
    {
        #region Command

        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
            "Command", typeof(ICommand), typeof(ClickBehaviour), new PropertyMetadata(null, OnCommandChanged));

        public static void SetCommand(DependencyObject element, ICommand value)
        {
            element.SetValue(CommandProperty, value);
        }

        public static ICommand GetCommand(DependencyObject element)
        {
            return (ICommand)element.GetValue(CommandProperty);
        }

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var clickBehaviour = GetClickBehaviour(d);
            clickBehaviour.OnCommandChanged(d, e);
        }

        #endregion

        #region CommandParameter

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached(
            "CommandParameter", typeof(object), typeof(ClickBehaviour), new PropertyMetadata(null));

        public static void SetCommandParameter(DependencyObject element, object value)
        {
            element.SetValue(CommandParameterProperty, value);
        }

        public static object GetCommandParameter(DependencyObject element)
        {
            return (object)element.GetValue(CommandParameterProperty);
        }

        #endregion

        #region Action

        public static readonly DependencyProperty ActionProperty = DependencyProperty.RegisterAttached(
            "Action", typeof(Action), typeof(ClickBehaviour), new PropertyMetadata(null, OnActionChanged));

        public static void SetAction(DependencyObject element, Action value)
        {
            element.SetValue(ActionProperty, value);
        }

        public static Action GetAction(DependencyObject element)
        {
            return (Action)element.GetValue(ActionProperty);
        }

        private static void OnActionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var clickBehaviour = GetClickBehaviour(d);
            clickBehaviour.OnActionChanged(d, e);
        }

        #endregion

        #region Methods

        private static ClickBehaviourCore GetClickBehaviour(DependencyObject element)
        {
            var result = ClickBehaviourCore.GetClickBehaviour(element);
            if (result == null)
            {
                result = new ClickBehaviourCore(Subscribe, Unsubscribe, GetCommand, GetCommandParameter, GetAction);
                ClickBehaviourCore.SetClickBehaviour(element, result);
            }

            return result;
        }

        private static void Subscribe(ClickBehaviourCore clickBehaviour, FrameworkElement element)
        {
            element.MouseLeftButtonDown += clickBehaviour.OnMouseLeftButtonDown;
            element.MouseLeftButtonUp += clickBehaviour.OnMouseLeftButtonUp;
            element.MouseMove += clickBehaviour.OnMouseMove;
            element.LostMouseCapture += clickBehaviour.OnLostMouseCapture;
            element.KeyDown += clickBehaviour.OnKeyDown;
            element.KeyUp += clickBehaviour.OnKeyUp;
            element.LostKeyboardFocus += clickBehaviour.OnLostKeyboardFocus;
            element.SizeChanged += clickBehaviour.OnRenderSizeChanged;

            InputMethod.SetIsInputMethodEnabled(element, false);
            KeyboardNavigation.SetAcceptsReturn(element, true);
        }

        private static void Unsubscribe(ClickBehaviourCore clickBehaviour, FrameworkElement element)
        {
            element.MouseLeftButtonDown -= clickBehaviour.OnMouseLeftButtonDown;
            element.MouseLeftButtonUp -= clickBehaviour.OnMouseLeftButtonUp;
            element.MouseMove -= clickBehaviour.OnMouseMove;
            element.LostMouseCapture -= clickBehaviour.OnLostMouseCapture;
            element.KeyDown -= clickBehaviour.OnKeyDown;
            element.KeyUp -= clickBehaviour.OnKeyUp;
            element.LostKeyboardFocus -= clickBehaviour.OnLostKeyboardFocus;
            element.SizeChanged -= clickBehaviour.OnRenderSizeChanged;
        }

        #endregion
    }
}