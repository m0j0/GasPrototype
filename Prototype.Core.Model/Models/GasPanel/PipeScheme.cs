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
        private DestinationVertex[] _destinationVertices;
        private SourceVertex[] _sourceVertices;

        #endregion

        #region Constructors

        public PipeScheme(params VerticesPair[] vertices)
        {
            _vertices = new HashSet<IVertex>();
            _edges = new List<Edge>();
            _propertyChangedEventHandler = ReflectionExtensions.MakeWeakPropertyChangedHandler(this, (scheme, obj, args) =>
            {
                scheme.InvalidatePaths();
            });

            foreach (var pair in vertices)
            {
                CreateEdge(pair.StartVertex, pair.EndVertex);
            }

            Initialize();
        }

        #endregion

        #region Properties

        public int PipesCount => _edges.Count;

        public int VerticesCount => _vertices.Count;

        public IEnumerable<IVertex> Vertices => _vertices;

        #endregion

        #region Methods

        internal Edge CreateEdge(IVertex startVertex, IVertex endVertex)
        {
            InitializeVertice(startVertex);
            InitializeVertice(endVertex);

            var edge = FindEdge(startVertex, endVertex);
            if (edge != null)
            {
                return edge;
            }

            var bidirectional = !(startVertex is SourceVertex) && !(endVertex is DestinationVertex);
            edge = new Edge(new PipeVm(), startVertex, endVertex, bidirectional);
            startVertex.AddAdjacentVertex(endVertex);
            if (bidirectional)
            {
                endVertex.AddAdjacentVertex(startVertex);
            }

            _edges.Add(edge);

            return edge;
        }

        public void AddVertices(params IVertex[] vertices)
        {
            if (vertices.Length < 2)
            {
                throw new ArgumentException("Should be at least two vertices");
            }

            for (var i = 0; i < vertices.Length -1; i++)
            {
                CreateEdge(vertices[i], vertices[i + 1]);
            }

            Initialize();
        }

        internal IPipeVm FindPipeVm(IVertex startVertex, IVertex endVertex)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("Cannot perform the operation, because object is disposed.");
            }

            return _edges.FirstOrDefault(edge => edge.Equals(startVertex, endVertex))?.PipeVm;
        }

        internal Edge FindEdge(IVertex startVertex, IVertex endVertex)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("Cannot perform the operation, because object is disposed.");
            }

            return _edges.FirstOrDefault(edge => edge.Equals(startVertex, endVertex));
        }

        private void Initialize()
        {
            //ValidateVertices();

            foreach (var vertex in _vertices)
            {
                // TODO
                vertex.PropertyChanged += _propertyChangedEventHandler;

                if (vertex is ValveVertex valveVertex)
                {
                    valveVertex.ValveVm.PropertyChanged += _propertyChangedEventHandler;
                }
            }

            InvalidatePaths();
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
            
            _destinationVertices = _vertices
                .OfType<DestinationVertex>()
                .ToArray();

            _sourceVertices = _vertices
                .OfType<SourceVertex>()
                .ToArray();

            foreach (var sourceVertex in _sourceVertices)
            {
                var algo = new DepthFirstDirectedPaths(sourceVertex, vertex => vertex.GetAllAdjacentVertices());

                foreach (var destinationVertex in _destinationVertices)
                {
                    var paths = algo.PathsTo(destinationVertex);
                    if (paths == null || paths.Count == 0)
                    {
                        throw new InvalidOperationException("Unreachable vertices detected");
                    }
                }
            }
        }

        private void InvalidatePaths()
        {
            _destinationVertices = _vertices
                .OfType<DestinationVertex>()
                .ToArray();

            _sourceVertices = _vertices
                .OfType<SourceVertex>()
                .ToArray();

            foreach (var edge in _edges)
            {
                edge.PipeVm.IsPresent = false;
            }

            foreach (var sourceVertex in _sourceVertices.Where(vertex => vertex.IsPresent))
            {
                var algo = new DepthFirstDirectedPaths(sourceVertex, vertex => vertex.GetAllAdjacentVertices().Where(vertex1 => vertex1.IsPresent).ToArray());

                foreach (var destinationVertex in _destinationVertices.Where(vertex => vertex.IsPresent))
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
                            edge.PipeVm.IsPresent = true;
                        }
                    }
                }
            }

            InvalidateFlows();
        }

        private void InvalidateFlows()
        {
            foreach (var edge in _edges)
            {
                edge.PipeVm.HasFlow = false;
            }
            
            foreach (var sourceVertex in _sourceVertices)
            {
                var algo = new DepthFirstDirectedPaths(sourceVertex, vertex => vertex.GetAdjacentVertices());

                foreach (var destinationVertex in _destinationVertices)
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