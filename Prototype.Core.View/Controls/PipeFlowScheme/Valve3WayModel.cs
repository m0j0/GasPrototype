﻿using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Prototype.Core.Controls.PipeFlowScheme
{
    internal interface IValve3Way : IValve
    {
        Valve3WayFlowPath PathWhenOpen { get; }

        Valve3WayFlowPath PathWhenClosed { get; }

        Rotation Rotation { get; }

        bool IsOpen { get; }
    }

    internal sealed class Valve3WayModel
    {
        private static readonly Dictionary<Rotation, Rect> PrimaryUpperPipeRects;
        private static readonly Dictionary<Rotation, Rect> PrimaryLowerPipeRects;
        private static readonly Dictionary<Rotation, Rect> AuxiliaryPipeRects;

        private readonly IValve3Way _valve;

        static Valve3WayModel()
        {
            PrimaryUpperPipeRects = new Dictionary<Rotation, Rect>
            {
                [Rotation.Rotate0] = new Rect(16, 0, 5, 24),
                [Rotation.Rotate90] = new Rect(19, 16, 24, 5),
                [Rotation.Rotate180] = new Rect(19, 19, 5, 24),
                [Rotation.Rotate270] = new Rect(0, 19, 24, 5)
            };
            PrimaryLowerPipeRects = new Dictionary<Rotation, Rect>
            {
                [Rotation.Rotate0] = new Rect(16, 19, 5, 24),
                [Rotation.Rotate90] = new Rect(0, 16, 24, 5),
                [Rotation.Rotate180] = new Rect(19, 0, 5, 24),
                [Rotation.Rotate270] = new Rect(19, 19, 24, 5)
            };
            AuxiliaryPipeRects = new Dictionary<Rotation, Rect>
            {
                [Rotation.Rotate0] = new Rect(16, 19, 24, 5),
                [Rotation.Rotate90] = new Rect(19, 16, 5, 24),
                [Rotation.Rotate180] = new Rect(0, 19, 24, 5),
                [Rotation.Rotate270] = new Rect(19, 0, 5, 24)
            };
        }

        public Valve3WayModel(IValve3Way valve)
        {
            _valve = valve;
        }

        public static Rect GetPrimaryUpperPipeRect(Rotation rotation)
        {
            return PrimaryUpperPipeRects[rotation];
        }

        public static Rect GetPrimaryLowerPipeRect(Rotation rotation)
        {
            return PrimaryLowerPipeRects[rotation];
        }

        public static Rect GetAuxiliaryPipeRect(Rotation rotation)
        {
            return AuxiliaryPipeRects[rotation];
        }

        public static bool CanPrimaryUpperPipePassFlow(IValve3Way valve)
        {
            if (valve.IsOpen)
            {
                return valve.PathWhenOpen == Valve3WayFlowPath.Direct ||
                       valve.PathWhenOpen == Valve3WayFlowPath.UpperAuxiliary;
            }

            return valve.PathWhenClosed == Valve3WayFlowPath.Direct ||
                   valve.PathWhenClosed == Valve3WayFlowPath.UpperAuxiliary;
        }

        public static bool CanPrimaryLowerPipePassFlow(IValve3Way valve)
        {
            if (valve.IsOpen)
            {
                return valve.PathWhenOpen == Valve3WayFlowPath.Direct ||
                       valve.PathWhenOpen == Valve3WayFlowPath.LowerAuxiliary;
            }

            return valve.PathWhenClosed == Valve3WayFlowPath.Direct ||
                   valve.PathWhenClosed == Valve3WayFlowPath.LowerAuxiliary;
        }

        public static bool CanAuxiliaryPipePassFlow(IValve3Way valve)
        {
            if (valve.IsOpen)
            {
                return valve.PathWhenOpen == Valve3WayFlowPath.UpperAuxiliary ||
                       valve.PathWhenOpen == Valve3WayFlowPath.LowerAuxiliary;
            }

            return valve.PathWhenClosed == Valve3WayFlowPath.UpperAuxiliary ||
                   valve.PathWhenClosed == Valve3WayFlowPath.LowerAuxiliary;
        }

        public static bool IsIntersection(Rect graphPipe, Rect standardRect)
        {
            var intersection = Common.FindIntersection(graphPipe, standardRect);
            return intersection == standardRect;
        }

        public bool CanPassFlow(IFlowGraph graph, IPipeSegment pipeSegment)
        {
            var pipe = graph.FindPipe(pipeSegment);

            var valveAbsoluteRect = graph.GetAbsoluteRect(_valve);
            var valveOffset = new Vector(-valveAbsoluteRect.TopLeft.X, -valveAbsoluteRect.TopLeft.Y);

            var intersectedPipeRect = graph.GetAbsoluteRect(pipe);
            intersectedPipeRect.Offset(valveOffset);

            if (IsIntersection(intersectedPipeRect, GetPrimaryUpperPipeRect(_valve.Rotation)))
            {
                return CanPrimaryUpperPipePassFlow(_valve);
            }
            if (IsIntersection(intersectedPipeRect, GetPrimaryLowerPipeRect(_valve.Rotation)))
            {
                return CanPrimaryLowerPipePassFlow(_valve);
            }
            if (IsIntersection(intersectedPipeRect, GetAuxiliaryPipeRect(_valve.Rotation)))
            {
                return CanAuxiliaryPipePassFlow(_valve);
            }

            return false;
        }
    }
}
