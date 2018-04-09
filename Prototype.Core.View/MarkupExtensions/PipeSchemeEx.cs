using System;
using System.Windows;
using System.Windows.Media;
using Prototype.Core.Controls;
using Prototype.Core.Interfaces;
using Prototype.Core.Interfaces.GasPanel;
using Prototype.Core.Models;
using Prototype.Core.Models.GasPanel;

namespace Prototype.Core.MarkupExtensions
{
    public static class PipeSchemeEx
    {
        #region StartVertex

        public static readonly DependencyProperty StartVertexProperty = DependencyProperty.RegisterAttached(
            "StartVertex", typeof(IVertex), typeof(PipeSchemeEx), new PropertyMetadata(default(IVertex), StartVertexPropertyChangedCallback));
        
        public static void SetStartVertex(DependencyObject element, IVertex value)
        {
            element.SetValue(StartVertexProperty, value);
        }

        public static IVertex GetStartVertex(DependencyObject element)
        {
            return (IVertex) element.GetValue(StartVertexProperty);
        }

        #endregion

        #region EndVertex

        public static readonly DependencyProperty EndVertexProperty = DependencyProperty.RegisterAttached(
            "EndVertex", typeof(IVertex), typeof(PipeSchemeEx), new PropertyMetadata(default(IVertex), EndVertexPropertyChangedCallback));

        public static void SetEndVertex(DependencyObject element, IVertex value)
        {
            element.SetValue(EndVertexProperty, value);
        }

        public static IVertex GetEndVertex(DependencyObject element)
        {
            return (IVertex) element.GetValue(EndVertexProperty);
        }

        #endregion

        #region Methods

        private static void StartVertexPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var pipe = dependencyObject as Pipe;
            if (pipe == null)
            {
                return;
            }

            UpdatePipeModel(pipe);
        }

        private static void EndVertexPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var pipe = dependencyObject as Pipe;
            if (pipe == null)
            {
                return;
            }

            UpdatePipeModel(pipe);
        }

        private static void UpdatePipeModel(Pipe pipe)
        {
            pipe.PipeVm = DeterminePipeModel(pipe);
        }

        private static IPipeVm DeterminePipeModel(Pipe pipe)
        {
            var startVertex = pipe.GetValue(StartVertexProperty) as IVertex;
            var endVertex = pipe.GetValue(EndVertexProperty) as IVertex;
            if (startVertex == null || endVertex == null)
            {
                return null;
            }

            if (startVertex.Owner == null || startVertex.Owner != endVertex.Owner)
            {
                throw new Exception("!!!");
            }

            return startVertex.Owner.FindPipeVm(startVertex, endVertex);
        }

        #endregion
    }
}
