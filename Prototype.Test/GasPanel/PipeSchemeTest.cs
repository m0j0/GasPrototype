using System;
using System.ComponentModel;
using MugenMvvmToolkit;
using MugenMvvmToolkit.Models;
using NUnit.Framework;
using Prototype.Core.Models.GasPanel;

namespace Prototype.Test.GasPanel
{
    [TestFixture]
    internal class PipeSchemeTest : UnitTestBase
    {
        #region SetUp

        [SetUp]
        public void SetUp()
        {
            Initialize(new MugenContainer());
        }

        #endregion

        #region Initialization

        [Test]
        public void TestPipeSchemeInitialization1()
        {
            var scheme = new PipeScheme();

            Assert.Throws<InvalidOperationException>(() => scheme.Initialize());
        }

        [Test]
        public void TestPipeSchemeInitialization2()
        {
            var scheme = new PipeScheme();
            var sourceVertex = new SourceVertex();
            var vertex = new Vertex();
            scheme.CreatePipe(sourceVertex, vertex);

            Assert.Throws<InvalidOperationException>(() => scheme.Initialize());
        }

        [Test]
        public void TestPipeSchemeInitialization3()
        {
            var scheme = new PipeScheme();
            var destinationVertex = new DestinationVertex();
            var vertex = new Vertex();
            scheme.CreatePipe(vertex, destinationVertex);

            Assert.Throws<InvalidOperationException>(() => scheme.Initialize());
        }

        [Test]
        public void TestPipeSchemeInitialization4()
        {
            var scheme = new PipeScheme();
            var sourceVertex = new SourceVertex();
            var destinationVertex = new DestinationVertex();
            scheme.CreatePipe(sourceVertex, destinationVertex);

            Assert.DoesNotThrow(() => scheme.Initialize());
        }

        [Test]
        public void TestPipeSchemeTwiceInitialization()
        {
            var scheme1 = new PipeScheme();
            var scheme2 = new PipeScheme();
            var vertex1 = new Vertex();
            var vertex2 = new Vertex();
            scheme1.CreatePipe(vertex1, vertex2);

            Assert.Throws<InvalidOperationException>(() => scheme2.CreatePipe(vertex1, vertex2));
        }

        #endregion

        #region Dispose

        [Test]
        public void DisposeTest()
        {
            var valveVm1 = new ValveVm();
            var valveVm2 = new ValveVm();

            var valveVertex1 = new ValveVertex(valveVm1);
            var valveVertex2 = new ValveVertex(valveVm2);

            var sourceVertex = new SourceVertex();

            var destinationVertex = new DestinationVertex();

            var count1 = GetPropertyChangedSubscribersCount(valveVm1);
            var count2 = GetPropertyChangedSubscribersCount(valveVm2);

            var scheme = new PipeScheme();
            scheme.CreatePipe(sourceVertex, valveVertex1);
            scheme.CreatePipe(valveVertex1, valveVertex2);
            scheme.CreatePipe(valveVertex2, destinationVertex);
            scheme.Initialize();

            Assert.Greater(GetPropertyChangedSubscribersCount(valveVm1), count1);
            Assert.Greater(GetPropertyChangedSubscribersCount(valveVm2), count2);

            scheme.Dispose();

            Assert.AreEqual(GetPropertyChangedSubscribersCount(valveVm1), count1);
            Assert.AreEqual(GetPropertyChangedSubscribersCount(valveVm2), count2);
        }

        private int GetPropertyChangedSubscribersCount(INotifyPropertyChanged obj)
        {
            var fieldInfo = obj.GetType().GetFieldEx(nameof(obj.PropertyChanged), MemberFlags.NonPublic | MemberFlags.Instance);
            var field = fieldInfo.GetValue(obj);
            var eventDelegate = (MulticastDelegate) field;
            return eventDelegate.GetInvocationList().Length;
        }

        #endregion
    }
}