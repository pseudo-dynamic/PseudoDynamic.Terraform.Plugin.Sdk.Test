﻿namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType
{
    internal class ComplexTypeVisitor
    {
        protected virtual VisitContext Rewrite(VisitContext context) =>
            context;

        private void VisitPropertySegment(IVisitPropertySegmentContext context)
        {
            if (context.VisitType.TryGetGenericArguments(out var genericArguments)) {
                for (int i = 0; i < genericArguments.Length; i++) {
                    var genericArgument = genericArguments[i];
                    RewriteThenVisit(new VisitPropertyGenericSegmentContext(context, genericArgument, i));
                }
            }
        }

        protected virtual void VisitPropertyGenericArgument(VisitPropertyGenericSegmentContext context) =>
            VisitPropertySegment(context);

        protected virtual void VisitProperty(VisitPropertyContext context) =>
            VisitPropertySegment(context);

        protected virtual void VisitComplex(VisitContext context)
        {
            context.RememberVisitTypeBeingVisited();
            var complexMetadata = context.ComplexTypeMetadata!;

            foreach (var property in complexMetadata.SupportedProperties) {
                RewriteThenVisit(new VisitPropertyContext(context, property));
            }
        }

        protected virtual void Visit(VisitContext context)
        {
            if (context.ContextType == VisitContextType.Complex) {
                VisitComplex(context);
            } else if (context.ContextType == VisitContextType.Property) {
                VisitProperty((VisitPropertyContext)context);
            } else if (context.ContextType == VisitContextType.PropertyGenericSegment) {
                VisitPropertyGenericArgument((VisitPropertyGenericSegmentContext)context);
            } else {
                throw new ArgumentException($"The context type is not supported: {context.ContextType.Id}");
            }
        }

        protected void RewriteThenVisit(VisitContext context) =>
            Visit(Rewrite(context));

        public virtual void RewriteThenVisitDynamic(VisitContext context)
        {
            var dynamicType = context.VisitType;

            //if (dynamicType.IsComplexType()) {
            //    RewriteThenVisit(context with { ContextType = VisitContextType.Complex.Inherits(context.ContextType) });
            //} else {
            //    RewriteThenVisit(context);
            //}

            RewriteThenVisit(context);
        }

        /// <summary>
        /// Represents the entry point of visiting a complex type.
        /// </summary>
        /// <param name="complexType"></param>
        /// <param name="context"></param>
        /// <exception cref="ArgumentException"></exception>
        public virtual void RewriteThenVisitComplex(Type complexType, Context? context = null)
        {
            if (complexType.IsGenericTypeDefinition) {
                throw new ArgumentException("The specified type must be a closed generic type");
            }

            if (!complexType.IsComplexType()) {
                throw new ArgumentException("The specified type must be either class or struct");
            }

            RewriteThenVisit(context.ToVisitingContext(complexType, VisitContextType.Complex));
        }

        public void RewriteThenVisitComplex<T>(Context? context = null) =>
            RewriteThenVisitComplex(typeof(T), context);
    }
}
