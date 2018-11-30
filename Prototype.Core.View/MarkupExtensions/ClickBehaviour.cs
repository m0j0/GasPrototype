using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using MugenMvvmToolkit;

namespace Prototype.Core.Controls
{
    // inspired by https://referencesource.microsoft.com/#PresentationFramework/src/Framework/System/Windows/Controls/Primitives/ButtonBase.cs
    // NOTE Access keys aren't supported
    public static class ClickBehaviour
    {
        #region Fields

        private static readonly ConditionalWeakTable<FrameworkElement, EventHandler> CommandsHandlers =
            new ConditionalWeakTable<FrameworkElement, EventHandler>();

        private static readonly MethodInfo ReadControlFlagMethod;
        private static readonly MethodInfo WriteControlFlagMethod;
        private static readonly object IsSpaceKeyDownFlag = (ushort) 0x0002;

        #endregion

        #region Constructors

        static ClickBehaviour()
        {
            ReadControlFlagMethod = typeof(Control).GetMethod("ReadControlFlag", BindingFlags.Instance | BindingFlags.NonPublic);
            if (ReadControlFlagMethod == null)
            {
                throw new NullReferenceException("ReadControlFlagMethod");
            }

            WriteControlFlagMethod = typeof(Control).GetMethod("WriteControlFlag", BindingFlags.Instance | BindingFlags.NonPublic);
            if (WriteControlFlagMethod == null)
            {
                throw new NullReferenceException("ReadControlFlagMethod");
            }
        }

        #endregion

        #region Attached properties

        #region IsPressed

        private static readonly DependencyProperty IsPressedProperty = DependencyProperty.RegisterAttached(
            "IsPressed", typeof(bool), typeof(ClickBehaviour), new PropertyMetadata(default(bool)));

        private static void SetIsPressed(DependencyObject element, bool value)
        {
            element.SetValue(IsPressedProperty, value);
        }

        private static bool GetIsPressed(DependencyObject element)
        {
            return (bool) element.GetValue(IsPressedProperty);
        }

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
            var element = (FrameworkElement) d;
            VerifyElement(element);

            if (e.OldValue is ICommand oldCommand)
            {
                Unsubscribe(element);

                if (CommandsHandlers.TryGetValue(element, out var handler))
                {
                    oldCommand.CanExecuteChanged -= handler;
                    CommandsHandlers.Remove(element);
                }
            }

            if (e.NewValue is ICommand newCommand)
            {
                Subscribe(element);

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

        private static void UpdateCanExecute(FrameworkElement element)
        {
            var command = GetCommand(element);
            element.IsEnabled = command == null || command.CanExecute(GetCommandParameter(element));
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
            var element = (FrameworkElement) d;
            VerifyElement(element);

            if (e.OldValue != null)
            {
                Unsubscribe(element);
            }

            if (e.NewValue != null)
            {
                Subscribe(element);
            }
        }

        #endregion

        private static void VerifyElement(FrameworkElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (GetCommand(element) != null &&
                GetAction(element) != null)
            {
                throw new InvalidOperationException("Can't set both Command and Action at the same time!");
            }
        }

        #endregion

        #region Methods

        private static void Subscribe(FrameworkElement element)
        {
            element.MouseLeftButtonDown += OnMouseLeftButtonDown;
            element.MouseLeftButtonUp += OnMouseLeftButtonUp;
            element.MouseMove += OnMouseMove;
            element.LostMouseCapture += OnLostMouseCapture;
            element.KeyDown += OnKeyDown;
            element.KeyUp += OnKeyUp;
            element.LostKeyboardFocus += OnLostKeyboardFocus;
            element.SizeChanged += OnRenderSizeChanged;

            InputMethod.SetIsInputMethodEnabled(element, false);
            KeyboardNavigation.SetAcceptsReturn(element, true);
        }

        private static void Unsubscribe(FrameworkElement element)
        {
            element.MouseLeftButtonDown -= OnMouseLeftButtonDown;
            element.MouseLeftButtonUp -= OnMouseLeftButtonUp;
            element.MouseMove -= OnMouseMove;
            element.LostMouseCapture -= OnLostMouseCapture;
            element.KeyDown -= OnKeyDown;
            element.KeyUp -= OnKeyUp;
            element.LostKeyboardFocus -= OnLostKeyboardFocus;
            element.SizeChanged -= OnRenderSizeChanged;
        }

        private static void OnClick(FrameworkElement element)
        {
            var command = GetCommand(element);
            if (command != null)
            {
                if (command.CanExecute(GetCommandParameter(element)))
                {
                    command.Execute(GetCommandParameter(element));
                }
            }

            var action = GetAction(element);
            action?.Invoke();
        }

        private static void OnRenderSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
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

        private static bool GetIsSpaceKeyDown(FrameworkElement element)
        {
            return (bool) ReadControlFlagMethod.InvokeEx(element, IsSpaceKeyDownFlag);
        }

        private static void SetIsSpaceKeyDown(FrameworkElement element, bool value)
        {
            WriteControlFlagMethod.InvokeEx(element, IsSpaceKeyDownFlag, value);
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

        private static void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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

        private static void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
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

        private static void OnMouseMove(object sender, MouseEventArgs e)
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

        private static void OnLostMouseCapture(object sender, MouseEventArgs e)
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

        private static void OnKeyDown(object sender, KeyEventArgs e)
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

        private static void OnKeyUp(object sender, KeyEventArgs e)
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

        private static void OnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
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
}