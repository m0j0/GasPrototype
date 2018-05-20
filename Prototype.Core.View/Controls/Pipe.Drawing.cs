using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Prototype.Core.Controls
{
    [Flags]
    internal enum Side
    {
        None = 0,
        Left = 1 << 0,
        Top = 1 << 1,
        Right = 1 << 2,
        Bottom = 1 << 3,
        LeftRight = Left | Right,
        TopBottom = Top | Bottom,
        All = Left | Right | Top | Bottom
    }

    internal static class PipeEx
    {
        public static Rect Intersect(Rect a, Rect b)
        {
            var x = Math.Max(a.X, b.X);
            var num1 = Math.Min(a.X + a.Width, b.X + b.Width);
            var y = Math.Max(a.Y, b.Y);
            var num2 = Math.Min(a.Y + a.Height, b.Y + b.Height);
            if (num1 >= x && num2 >= y)
            {
                return new Rect(x, y, num1 - x, num2 - y);
            }

            return Rect.Empty;
        }

        private static bool IsIntersect(Rect a, Rect b)
        {
            return a.Left <= b.Right &&
                   b.Left <= a.Right &&
                   a.Top <= b.Bottom &&
                   b.Top <= a.Bottom;
        }

        public static IReadOnlyCollection<IPipeSegment> SplitPipeToSegments(Pipe pipe, IReadOnlyCollection<Pipe> allPipes)
        {
            var processPipes = allPipes.Select(p => new ProcessPipe(p)).ToArray();
            var currentProcessPipe = processPipes.First(processPipe => processPipe.Pipe == pipe);
            var connectors = new HashSet<IConnector>();
            var result = new List<IPipeSegment>();

            foreach (var pipe1 in processPipes)
            {
                foreach (var pipe2 in processPipes)
                {
                    if (pipe1 == pipe2)
                    {
                        continue;
                    }

                    var intersectionRect = Intersect(
                        pipe1.Rect,
                        pipe2.Rect
                    );

                    if (intersectionRect.Width != Pipe.PipeWidth ||
                        intersectionRect.Height != Pipe.PipeWidth)
                    {
                        continue;
                    }

                    var sortedPipes = SortPipes(pipe1, pipe2);

                    if (pipe1.Rect.Left < intersectionRect.Left &&
                        pipe1.Rect.Right > intersectionRect.Right &&

                        pipe2.Rect.Top < intersectionRect.Top &&
                        pipe2.Rect.Bottom > intersectionRect.Bottom)
                    {
                        var bridgeConnector = connectors.OfType<BridgeConnector>().FirstOrDefault(cnn => cnn.Rect == intersectionRect) ??
                                              new BridgeConnector(intersectionRect);
                        connectors.Add(bridgeConnector);
                        bridgeConnector.AddPipes(pipe1, pipe2);
                        continue;
                    }

                    var cornerConnector = connectors.OfType<CornerConnector>().FirstOrDefault(cnn => cnn.Rect == intersectionRect) ??
                                          new CornerConnector(intersectionRect);
                    cornerConnector.AddPipe(pipe1);
                    cornerConnector.AddPipe(pipe2);
                    connectors.Add(cornerConnector);
                }
            }

            foreach (var connector in currentProcessPipe.Connectors)
            {
                currentProcessPipe.Segments.Add(new ConnectorSegment(
                    new Point(connector.Rect.Left - currentProcessPipe.Rect.Left, connector.Rect.Top - currentProcessPipe.Rect.Top),
                    currentProcessPipe.Orientation,
                    Side.All
                ));
            }

            var orderedSegments = currentProcessPipe.Segments.OrderBy(s => s.StartPoint.X).ThenBy(s => s.StartPoint.Y).ToList();
            
            var first = orderedSegments.FirstOrDefault();
            if (first == null ||
                first.StartPoint.X != 0 ||
                first.StartPoint.Y != 0)
            {
                orderedSegments.Insert(0,
                    new LinePipeSegment(new Point(0, 0),
                        0,
                        currentProcessPipe.Orientation)
                );
            }

            var last = orderedSegments.LastOrDefault();
            if (last == null ||
                last.StartPoint.X + Pipe.PipeWidth != currentProcessPipe.Rect.Right ||
                last.StartPoint.Y + Pipe.PipeWidth != currentProcessPipe.Rect.Bottom)
            {
                if (currentProcessPipe.Orientation == Orientation.Horizontal)
                {
                    orderedSegments.Add(new LinePipeSegment(new Point(currentProcessPipe.Rect.Width, 0),
                        0,
                        currentProcessPipe.Orientation));
                }
                else
                {
                    orderedSegments.Add(new LinePipeSegment(new Point(0, currentProcessPipe.Rect.Height),
                        0,
                        currentProcessPipe.Orientation));
                }
            }


            List<IPipeSegment> allSegments = new List<IPipeSegment>();
            for (int i = 0; i < orderedSegments.Count - 1; i++)
            {
                var s1 = orderedSegments[i];
                var s2 = orderedSegments[i + 1];

                allSegments.Add(s1);

                if (currentProcessPipe.Orientation == Orientation.Horizontal)
                {
                    allSegments.Add(new LinePipeSegment(new Point(s1.StartPoint.X + s1.Length, 0),
                        s2.StartPoint.X - (s1.StartPoint.X + s1.Length),
                        currentProcessPipe.Orientation));
                }
                else
                {
                    allSegments.Add(new LinePipeSegment(new Point(0, s1.StartPoint.Y + s1.Length),
                        s2.StartPoint.Y - (s1.StartPoint.Y + s1.Length),
                        currentProcessPipe.Orientation));
                }

                allSegments.Add(s2);
            }
            
            return allSegments;
        }

        private static Tuple<ProcessPipe, ProcessPipe> SortPipes(ProcessPipe pipe1, ProcessPipe pipe2)
        {
           var arr= new[] {pipe1, pipe2}.OrderBy(p => p.Rect.Left).ThenBy(p => p.Rect.Top).ThenBy(p => p.Orientation).ToArray();
            return Tuple.Create(arr[0], arr[1]);
        }

        public static bool HasFlagEx(this Side side, Side flag)
        {
            return (side & flag) != 0;
        }
    }

    internal class ProcessPipe
    {
        public ProcessPipe(Pipe pipe)
        {
            Rect = new Rect(Canvas.GetLeft(pipe), Canvas.GetTop(pipe), pipe.Width, pipe.Height);
            Pipe = pipe;
            Segments=new List<IPipeSegment>();
            Connectors = new List<IConnector>();
        }

        public Pipe Pipe { get; }
        public Rect Rect { get; }
        public Orientation Orientation => Pipe.Orientation;

        public List<IPipeSegment> Segments { get; }

        public List<IConnector> Connectors { get; }

        public override string ToString()
        {
            return $"{Rect} {Orientation}";
        }
    }

    internal interface IConnector
    {
        Rect Rect { get; }
    }

    internal class BridgeConnector : IConnector
    {
        public Rect Rect { get; }

        public BridgeConnector(Rect rect)
        {
            Rect = rect;
        }

        public ProcessPipe Pipe1 { get; private set; }
        public ProcessPipe Pipe2 { get; private set; }

        public void AddPipes(ProcessPipe pipe1, ProcessPipe pipe2)
        {
            if (Pipe1 != null || Pipe2 != null)
            {
                throw new Exception("!!!");
            }
            if (pipe1 == null || pipe2 == null)
            {
                throw new Exception("!!!");
            }

            if (pipe1.Connectors.Contains(this))
            {
                throw new Exception("!!");
            }
            if (pipe2.Connectors.Contains(this))
            {
                throw new Exception("!!");
            }

            Pipe1 = pipe1;
            pipe1.Connectors.Add(this);

            Pipe2 = pipe2;
            pipe2.Connectors.Add(this);
        }
    }

    internal class CornerConnector : IConnector
    {
        public Rect Rect { get; }
        private readonly ProcessPipe[] _pipes;
        
        public CornerConnector(Rect rect)
        {
            Rect = rect;
            _pipes=new ProcessPipe[4];
        }

        public ProcessPipe Pipe1 => _pipes[0];
        public ProcessPipe Pipe2 => _pipes[1];
        public ProcessPipe Pipe3 => _pipes[2];
        public ProcessPipe Pipe4 => _pipes[3];

        public void AddPipe(ProcessPipe pipe)
        {
            if (pipe.Connectors.Contains(this))
            {
                //throw new Exception("!!");
                return;
            }
            pipe.Connectors.Add(this);

            bool set = false;
            for (int i = 0; i < _pipes.Length; i++)
            {
                if (_pipes[i] == null)
                {
                    _pipes[i] = pipe;
                    set = true;
                    break;
                }
            }

            if (!set)
            {
                throw new Exception("!!!");
            }
        }
    }



    internal interface IPipeSegment
    {
        Point StartPoint { get; }

        double Length { get; }

        Orientation Orientation { get; }
    }

    internal class ConnectorSegment : IPipeSegment
    {
        public ConnectorSegment(Point startPoint, Orientation orientation, Side side)
        {
            StartPoint = startPoint;
            Orientation = orientation;
            Side = side;
        }

        public Point StartPoint { get; }

        public double Length => Pipe.PipeWidth;

        public Orientation Orientation { get; }

        public Side Side { get; }
    }

    internal class LinePipeSegment : IPipeSegment
    {
        public LinePipeSegment(Point startPoint, double length, Orientation orientation)
        {
            StartPoint = startPoint;
            Length = length;
            Orientation = orientation;
        }

        public Point StartPoint { get; }
        public double Length { get; }
        public Orientation Orientation { get; }

        public override string ToString()
        {
            return $"Segment StartPoint: {StartPoint}, lenght: {Length}";
        }
    }


}