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
        #region Nested types

        public class Owner
        {
            public Owner(Action<ClickBehaviourCore, FrameworkElement> subscribe,
                Action<ClickBehaviourCore, FrameworkElement> unsubscribe,
                Func<DependencyObject, ICommand> getCommand,
                Func<DependencyObject, object> getCommandParameter,
                Func<DependencyObject, Action> getAction,
                Func<MouseButtonState> getMouseButtonState)
            {
                Should.NotBeNull(subscribe, nameof(subscribe));
                Should.NotBeNull(unsubscribe, nameof(unsubscribe));
                Should.NotBeNull(getCommand, nameof(getCommand));
                Should.NotBeNull(getCommandParameter, nameof(getCommandParameter));
                Should.NotBeNull(getAction, nameof(getAction));
                Should.NotBeNull(getMouseButtonState, nameof(getMouseButtonState));

                Subscribe = subscribe;
                Unsubscribe = unsubscribe;
                GetCommand = getCommand;
                GetCommandParameter = getCommandParameter;
                GetAction = getAction;
                GetMouseButtonState = getMouseButtonState;
            }

            public Action<ClickBehaviourCore, FrameworkElement> Subscribe { get; }

            public Action<ClickBehaviourCore, FrameworkElement> Unsubscribe { get; }

            public Func<DependencyObject, ICommand> GetCommand { get; }

            public Func<DependencyObject, object> GetCommandParameter { get; }

            public Func<DependencyObject, Action> GetAction { get; }

            public Func<MouseButtonState> GetMouseButtonState { get; }
        }

        #endregion

        #region Fields

        private readonly Owner _owner;
        private EventHandler _handler;
        private bool _isPressed;
        private bool _isSpaceKeyDown;

        #endregion

        public ClickBehaviourCore(Owner owner)
        {
            Should.NotBeNull(owner, nameof(owner));
            _owner = owner;
        }

        #region Attached properties

        #region Command

        public void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = VerifyElement(d);

            if (e.OldValue is ICommand oldCommand)
            {
                _owner.Unsubscribe(this, element);

                if (_handler != null)
                {
                    oldCommand.CanExecuteChanged -= _handler;
                    _handler = null;
                }
            }

            if (e.NewValue is ICommand newCommand)
            {
                _owner.Subscribe(this, element);

                _handler = ReflectionExtensions.CreateWeakDelegate<FrameworkElement, EventArgs, EventHandler>(
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

                newCommand.CanExecuteChanged += _handler;
            }

            UpdateCanExecute(element);
        }

        private void UpdateCanExecute(FrameworkElement element)
        {
            var command = _owner.GetCommand(element);
            element.IsEnabled = command == null || command.CanExecute(_owner.GetCommandParameter(element));
        }

        #endregion

        #region Action

        public void OnActionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = VerifyElement(d);

            if (e.OldValue != null)
            {
                _owner.Unsubscribe(this, element);
            }

            if (e.NewValue != null)
            {
                _owner.Subscribe(this, element);
            }
        }

        #endregion

        private FrameworkElement VerifyElement(DependencyObject obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            var element = obj as FrameworkElement;
            if (element == null)
            {
                throw new NotSupportedException(nameof(element));
            }

            if (_owner.GetCommand(element) != null &&
                _owner.GetAction(element) != null)
            {
                throw new InvalidOperationException("Can't set both Command and Action at the same time!");
            }

            return element;
        }

        #endregion

        #region Methods

        private void OnClick(FrameworkElement element)
        {
            var command = _owner.GetCommand(element);
            if (command != null)
            {
                if (command.CanExecute(_owner.GetCommandParameter(element)))
                {
                    command.Execute(_owner.GetCommandParameter(element));
                }
            }

            var action = _owner.GetAction(element);
            action?.Invoke();
        }

        public void OnRenderSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            if (!(sender is FrameworkElement element))
            {
                return;
            }

            if (element.IsMouseCaptured && _owner.GetMouseButtonState() == MouseButtonState.Pressed && !_isSpaceKeyDown)
            {
                UpdateIsPressed(element);
            }
        }

        private bool GetMouseLeftButtonReleased()
        {
            return _owner.GetMouseButtonState() == MouseButtonState.Released;
        }

        private static bool GetIsInMainFocusScope(FrameworkElement element)
        {
            Visual focusScope = FocusManager.GetFocusScope(element) as Visual;
            return focusScope == null || VisualTreeHelper.GetParent(focusScope) == null;
        }

        private void UpdateIsPressed(FrameworkElement element)
        {
            Point pos = Mouse.PrimaryDevice.GetPosition(element);

            if (pos.X >= 0 && pos.X <= element.ActualWidth && pos.Y >= 0 && pos.Y <= element.ActualHeight)
            {
                _isPressed = true;
            }
            else if (_isPressed)
            {
                _isPressed = false;
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
                _isPressed = true;
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
            bool shouldClick = !_isSpaceKeyDown && _isPressed;

            if (element.IsMouseCaptured && !_isSpaceKeyDown)
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

            if (element.IsMouseCaptured && _owner.GetMouseButtonState() == MouseButtonState.Pressed && !_isSpaceKeyDown)
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

            if (e.OriginalSource != element || _isSpaceKeyDown)
            {
                return;
            }

            if (element.IsKeyboardFocused && !GetIsInMainFocusScope(element))
            {
                Keyboard.Focus(null);
            }

            _isPressed = false;
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
                    if (!element.IsMouseCaptured && e.OriginalSource == element)
                    {
                        _isSpaceKeyDown = true;
                        _isPressed = true;
                        element.CaptureMouse();

                        e.Handled = true;
                    }
                }
            }
            else if (e.Key == Key.Enter && (bool) element.GetValue(KeyboardNavigation.AcceptsReturnProperty))
            {
                if (e.OriginalSource == element)
                {
                    _isSpaceKeyDown = false;
                    _isPressed = false;
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
                if (_isSpaceKeyDown)
                {
                    _isPressed = false;
                    _isSpaceKeyDown = false;
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

            if (e.Key == Key.Space && _isSpaceKeyDown)
            {
                if ((Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Alt)) != ModifierKeys.Alt)
                {
                    _isSpaceKeyDown = false;
                    if (GetMouseLeftButtonReleased())
                    {
                        bool shouldClick = _isPressed;

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
                _isPressed = false;

                if (element.IsMouseCaptured)
                {
                    element.ReleaseMouseCapture();
                }

                _isSpaceKeyDown = false;
            }
        }

        #endregion
    }

    public static class ClickBehaviour
    {
        #region Fields

        private static readonly ClickBehaviourCore.Owner ClickBehaviourOwner =
            new ClickBehaviourCore.Owner(Subscribe, Unsubscribe, GetCommand, GetCommandParameter, GetAction, GetMouseButtonState);

        #endregion

        #region Command

        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
            "Command", typeof(ICommand), typeof(ClickBehaviour), new PropertyMetadata(null, OnCommandChanged));

        public static void SetCommand(DependencyObject element, ICommand value)
        {
            element.SetValue(CommandProperty, value);
        }

        public static ICommand GetCommand(DependencyObject element)
        {
            return (ICommand) element.GetValue(CommandProperty);
        }

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var clickBehaviour = GetClickBehaviourInternal(d);
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
            return (object) element.GetValue(CommandParameterProperty);
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
            return (Action) element.GetValue(ActionProperty);
        }

        private static void OnActionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var clickBehaviour = GetClickBehaviourInternal(d);
            clickBehaviour.OnActionChanged(d, e);
        }

        #endregion

        #region ClickBehaviourCore

        private static readonly DependencyProperty ClickBehaviourProperty = DependencyProperty.RegisterAttached(
            "ClickBehaviour", typeof(ClickBehaviourCore), typeof(ClickBehaviour), new PropertyMetadata(null));

        private static void SetClickBehaviour(DependencyObject element, ClickBehaviourCore value)
        {
            element.SetValue(ClickBehaviourProperty, value);
        }

        private static ClickBehaviourCore GetClickBehaviour(DependencyObject element)
        {
            return (ClickBehaviourCore) element.GetValue(ClickBehaviourProperty);
        }

        #endregion

        #region Methods

        private static ClickBehaviourCore GetClickBehaviourInternal(DependencyObject element)
        {
            var result = GetClickBehaviour(element);
            if (result == null)
            {
                result = new ClickBehaviourCore(ClickBehaviourOwner);
                SetClickBehaviour(element, result);
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

        private static MouseButtonState GetMouseButtonState()
        {
            return Mouse.PrimaryDevice.LeftButton;
        }

        #endregion
    }

    public static class RightClickBehaviour
    {
        #region Fields

        private static readonly ClickBehaviourCore.Owner ClickBehaviourOwner =
            new ClickBehaviourCore.Owner(Subscribe, Unsubscribe, GetCommand, GetCommandParameter, GetAction, GetMouseButtonState);

        #endregion

        #region Command

        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
            "Command", typeof(ICommand), typeof(RightClickBehaviour), new PropertyMetadata(null, OnCommandChanged));

        public static void SetCommand(DependencyObject element, ICommand value)
        {
            element.SetValue(CommandProperty, value);
        }

        public static ICommand GetCommand(DependencyObject element)
        {
            return (ICommand) element.GetValue(CommandProperty);
        }

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var clickBehaviour = GetClickBehaviourInternal(d);
            clickBehaviour.OnCommandChanged(d, e);
        }

        #endregion

        #region CommandParameter

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached(
            "CommandParameter", typeof(object), typeof(RightClickBehaviour), new PropertyMetadata(null));

        public static void SetCommandParameter(DependencyObject element, object value)
        {
            element.SetValue(CommandParameterProperty, value);
        }

        public static object GetCommandParameter(DependencyObject element)
        {
            return (object) element.GetValue(CommandParameterProperty);
        }

        #endregion

        #region Action

        public static readonly DependencyProperty ActionProperty = DependencyProperty.RegisterAttached(
            "Action", typeof(Action), typeof(RightClickBehaviour), new PropertyMetadata(null, OnActionChanged));

        public static void SetAction(DependencyObject element, Action value)
        {
            element.SetValue(ActionProperty, value);
        }

        public static Action GetAction(DependencyObject element)
        {
            return (Action) element.GetValue(ActionProperty);
        }

        private static void OnActionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var clickBehaviour = GetClickBehaviourInternal(d);
            clickBehaviour.OnActionChanged(d, e);
        }

        #endregion

        #region ClickBehaviourCore

        private static readonly DependencyProperty ClickBehaviourProperty = DependencyProperty.RegisterAttached(
            "ClickBehaviour", typeof(ClickBehaviourCore), typeof(RightClickBehaviour), new PropertyMetadata(null));

        private static void SetClickBehaviour(DependencyObject element, ClickBehaviourCore value)
        {
            element.SetValue(ClickBehaviourProperty, value);
        }

        private static ClickBehaviourCore GetClickBehaviour(DependencyObject element)
        {
            return (ClickBehaviourCore) element.GetValue(ClickBehaviourProperty);
        }

        #endregion

        #region Methods

        private static ClickBehaviourCore GetClickBehaviourInternal(DependencyObject element)
        {
            var result = GetClickBehaviour(element);
            if (result == null)
            {
                result = new ClickBehaviourCore(ClickBehaviourOwner);
                SetClickBehaviour(element, result);
            }

            return result;
        }

        private static void Subscribe(ClickBehaviourCore clickBehaviour, FrameworkElement element)
        {
            element.MouseRightButtonDown += clickBehaviour.OnMouseLeftButtonDown;
            element.MouseRightButtonUp += clickBehaviour.OnMouseLeftButtonUp;
            element.MouseMove += clickBehaviour.OnMouseMove;
            element.LostMouseCapture += clickBehaviour.OnLostMouseCapture;
            element.SizeChanged += clickBehaviour.OnRenderSizeChanged;
        }

        private static void Unsubscribe(ClickBehaviourCore clickBehaviour, FrameworkElement element)
        {
            element.MouseRightButtonDown -= clickBehaviour.OnMouseLeftButtonDown;
            element.MouseRightButtonUp -= clickBehaviour.OnMouseLeftButtonUp;
            element.MouseMove -= clickBehaviour.OnMouseMove;
            element.LostMouseCapture -= clickBehaviour.OnLostMouseCapture;
            element.SizeChanged -= clickBehaviour.OnRenderSizeChanged;
        }

        private static MouseButtonState GetMouseButtonState()
        {
            return Mouse.PrimaryDevice.RightButton;
        }

        #endregion
    }
}