﻿namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.Specialized
{
    /// <summary>
    /// A specialized class for capturing children of one visitation.
    /// This technique can be used to transform a recursive visitors
    /// into a declarative visitor.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class SameDepthCapturing<T>
    {
        public bool IsSameDepthCapturingEnabled => _queueSameDepth;

        private bool _queueSameDepth;
        private Queue<T> _sameDepthCaptured = new Queue<T>();

        public bool EnableSameDepthCapturing(bool enable = true) => _queueSameDepth = enable;

        public void CaptureSameDepth(T value) => _sameDepthCaptured.Enqueue(value);

        public List<T> CaptureSameDepth(Action<T> visitSameDepth, T visitSameDepthArgument)
        {
            EnableSameDepthCapturing();
            visitSameDepth(visitSameDepthArgument);
            EnableSameDepthCapturing(false);

            var capturedContexts = new List<T>(_sameDepthCaptured);
            _sameDepthCaptured.Clear();
            return capturedContexts;
        }
    }
}
