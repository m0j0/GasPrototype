using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MugenMvvmToolkit;
using Prototype.Core.Interfaces.GasPanel;

namespace Prototype.Core.Models.GasPanel
{
    public sealed class PipeScheme : IDisposable
    {
        #region Fields

        private readonly HashSet<IVertex> _vertices;
        private readonly List<Edge> _edges;
        private readonly PropertyChangedEventHandler _propertyChangedEventHandler;
        private bool _isDisposed;

        #endregion

        #region Constructors

        public PipeScheme(params VerticesPair[] vertices)
        {
            _vertices = new HashSet<IVertex>();
            _edges = new List<Edge>();
            _propertyChangedEventHandler = ReflectionExtensions.MakeWeakPropertyChangedHandler(this, (scheme, obj, args) => scheme.InvalidateFlows());

            foreach (var pair in vertices)
            {
                CreateEdge(pair.StartVertex, pair.EndVertex);
            }

            Initialize();
        }

        #endregion

        #region Methods

        internal IPipeVm FindPipeVm(IVertex startVertex, IVertex endVertex)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("Cannot perform the operation, because object is disposed.");
            }

            return _edges.FirstOrDefault(edge => edge.Equals(startVertex, endVertex))?.PipeVm;
        }

        private void Initialize()
        {
            ValidateVertices();

            foreach (var vertex in _vertices.OfType<ValveVertex>())
            {
                vertex.ValveVm.PropertyChanged += _propertyChangedEventHandler;
            }

            InvalidateFlows();
        }

        private void ValidateVertices()
        {
            if (_vertices.Count(vertex => vertex is SourceVertex) == 0)
            {
                throw new InvalidOperationException("Should be at least one source");
            }

            if (_vertices.Count(vertex => vertex is DestinationVertex) == 0)
            {
                throw new InvalidOperationException("Should be at least one destination");
            }

            foreach (var vertex in _vertices)
            {
                vertex.Validate();
            }
            
            var destinationVertices = _vertices
                .OfType<DestinationVertex>()
                .ToArray();

            var sourceVertices = _vertices
                .OfType<SourceVertex>();

            foreach (var sourceVertex in sourceVertices)
            {
                var algo = new DepthFirstDirectedPaths(sourceVertex, vertex => vertex.GetAllAdjacentVertices());

                foreach (var destinationVertex in destinationVertices)
                {
                    var paths = algo.PathsTo(destinationVertex);
                    if (paths == null || paths.Count == 0)
                    {
                        throw new InvalidOperationException("Unreachable vertices detected");
                    }
                }
            }
        }

        private void InvalidateFlows()
        {
            foreach (var edge in _edges)
            {
                edge.PipeVm.HasFlow = false;
            }

            var destinationVertices = _vertices
                .OfType<DestinationVertex>()
                .ToArray();

            var sourceVertices = _vertices
                .OfType<SourceVertex>();

            foreach (var sourceVertex in sourceVertices)
            {
                var algo = new DepthFirstDirectedPaths(sourceVertex, vertex => vertex.GetAdjacentVertices());

                foreach (var destinationVertex in destinationVertices)
                {
                    var paths = algo.PathsTo(destinationVertex);
                    if (paths == null || paths.Count == 0)
                    {
                        continue;
                    }

                    foreach (var path in paths)
                    {
                        for (var i = 0; i < path.Count - 1; i++)
                        {
                            var edge = _edges.Find(pathEdge => pathEdge.Equals(path[i], path[i + 1]));
                            edge.PipeVm.HasFlow = true;
                        }
                    }
                }
            }
        }

        private Edge CreateEdge(IVertex startVertex, IVertex endVertex)
        {
            InitializeVertice(startVertex);
            InitializeVertice(endVertex);

            var bidirectional = !(startVertex is SourceVertex) && !(endVertex is DestinationVertex);

            var edge = new Edge(new PipeVm(), startVertex, endVertex, bidirectional);
            startVertex.AddAdjacentVertex(endVertex);
            if (bidirectional)
            {
                endVertex.AddAdjacentVertex(startVertex);
            }

            _edges.Add(edge);

            return edge;
        }

        private void InitializeVertice(IVertex vertex)
        {
            if (vertex.Owner != null && vertex.Owner != this)
            {
                throw new InvalidOperationException("Vertex cannot be used twice");
            }

            if (vertex.Owner == null)
            {
                vertex.Owner = this;
                _vertices.Add(vertex);
            }
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            foreach (var vertex in _vertices.OfType<ValveVertex>())
            {
                vertex.ValveVm.PropertyChanged -= _propertyChangedEventHandler;
            }
        }

        #endregion
    }
}