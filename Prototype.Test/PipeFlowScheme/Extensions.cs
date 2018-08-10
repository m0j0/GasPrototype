using System;
using System.Linq;
using Prototype.Core.Controls.PipeFlowScheme;

namespace Prototype.Test.PipeFlowScheme
{
    internal static class Extensions
    {
        public static bool PipeSegmentsFlowHasValue(this IPipe pipe, bool val)
        {
            foreach (var segment in pipe.Segments)
            {
                if (segment.HasFlow != val)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool PipeHasSegmentTypes(this IPipe pipe, params Type[] segmentTypes)
        {
            if (pipe.Segments.Count != segmentTypes.Length)
            {
                return false;
            }

            for (var i = 0; i < pipe.Segments.Count; i++)
            {
                if (pipe.Segments[i].GetType() != segmentTypes[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool PipeDoesNotHaveSegments(this IPipe pipe)
        {
            return pipe.Segments == null || pipe.Segments.Count == 0;
        }

        public static bool PipeHasSegments(this IPipe pipe, int count)
        {
            return pipe.Segments != null && pipe.Segments.Count == count;
        }

        public static bool PipeHasSegmentFlow(this IPipe pipe, params bool[] segmentTypes)
        {
            if (pipe.Segments == null)
            {
                return segmentTypes.Length == 0;
            }
            if (pipe.Segments.Count != segmentTypes.Length)
            {
                return false;
            }

            for (var i = 0; i < pipe.Segments.Count; i++)
            {
                if (pipe.Segments[i].HasFlow != segmentTypes[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool PipeIsFailed(this IPipe pipe, FailType failType)
        {
            if (pipe.Segments.Count != 1)
            {
                return false;
            }

            var failedSegment = pipe.Segments[0] as FailedSegment;
            if (failedSegment == null)
            {
                return false;
            }

            return failedSegment.FailType == failType;
        }

        public static bool PipeIsNotFailed(this IPipe pipe)
        {
            return pipe.Segments.Any(segment => !(segment is FailedSegment));
        }

        public static bool PipeIsEmpty(this IPipe pipe)
        {
            return pipe.Segments == null || pipe.Segments.Count == 0;
        }

        public static bool PipeSegmentsHasLength(this IPipe pipe, params double[] lengths)
        {
            if (pipe.Segments.Count != lengths.Length)
            {
                return false;
            }

            for (var i = 0; i < pipe.Segments.Count; i++)
            {
                if (pipe.Segments[i].Length != lengths[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}