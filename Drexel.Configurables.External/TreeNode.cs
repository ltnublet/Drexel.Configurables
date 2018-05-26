using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Drexel.Configurables.External
{
    /// <summary>
    /// Represents a generic node in a tree, containing a value of type <typeparamref name="T"/>, and possessing some
    /// parents and children.
    /// </summary>
    /// <typeparam name="T">
    /// The type of object the node contains.
    /// </typeparam>
    public class TreeNode<T> : IDisposable, IEnumerable<T>
    {
        private readonly List<TreeNode<T>> innerChildren;

        /// <summary>
        /// Instantiates a new <see cref="TreeNode{T}"/> instance using the supplied values.
        /// </summary>
        /// <param name="value">
        /// The value of the <see cref="TreeNode{T}"/>.
        /// </param>
        /// <param name="parent">
        /// The parent of the <see cref="TreeNode{T}"/>, or <see langword="null"/> if the instance has no parent.
        /// </param>
        /// <param name="children">
        /// The children of the <see cref="TreeNode{T}"/>, or <see langword="null"/> if the instance has no children.
        /// If <see langword="null"/>, <see cref="Children"/> will be set to an empty <see cref="List{T}"/>.
        /// </param>
        public TreeNode(T value, TreeNode<T> parent = null, List<TreeNode<T>> children = null)
        {
            this.Value = value;
            this.Parent = parent;
            this.Parent?.Add(this);

            this.innerChildren = new List<TreeNode<T>>();
            if (children != null)
            {
                this.AddRange(children);
            }
        }

        /// <summary>
        /// Represents the value of this node.
        /// </summary>
        public T Value { get; private set; }

        /// <summary>
        /// Contains the parent of this node, or null if no parent.
        /// </summary>
        public TreeNode<T> Parent { get; private set; }

        /// <summary>
        /// Contains the children of this node.
        /// </summary>
        public IReadOnlyList<TreeNode<T>> Children
        {
            get
            {
                return this.innerChildren;
            }
        }

        /// <summary>
        /// Adds the supplied <see cref="TreeNode{T}"/> as a child to the current node.
        /// </summary>
        /// <param name="child">
        /// The node to add as a child. Cannot be <see langword="null"/>.
        /// </param>
        public void Add(TreeNode<T> child)
        {
            if (child == null)
            {
                throw new ArgumentNullException(nameof(child), "Child to add must be non-null.");
            }

            child.Parent = this;
            this.innerChildren.Add(child);
        }

        /// <summary>
        /// Adds all the <see cref="TreeNode{T}"/>s in the supplied parameter <paramref name="children"/> as children
        /// of the current node.
        /// </summary>
        /// <param name="children">
        /// The list of nodes to add as children. Cannot be <see langword="null"/>, or contain <see langword="null"/>.
        /// </param>
        public void AddRange(IEnumerable<TreeNode<T>> children)
        {
            if (children == null)
            {
                throw new ArgumentNullException(nameof(children), "Range of children to add must be non-null.");
            }

            if (children.Any(x => x == null))
            {
                throw new ArgumentException("Range of children to add must contain no nulls.", nameof(children));
            }

            foreach (TreeNode<T> child in children)
            {
                child.Parent = this;
            }

            this.innerChildren.AddRange(children);
        }

        /// <summary>
        /// Disposes the current <see cref="TreeNode{T}"/> and its <see cref="TreeNode{T}.Value"/> (if
        /// <see cref="TreeNode{T}.Value"/> is a disposable type).
        /// </summary>
        public void Dispose()
        {
            if (this.Value is IDisposable asDisposable)
            {
                asDisposable.Dispose();
            }
        }

        /// <summary>
        /// Returns an <see cref="IEnumerator{T}"/> representing this <see cref="TreeNode{T}"/>'s values.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> representing this element's value, and a depth-first result of children's
        /// values.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            yield return this.Value;

            foreach (TreeNode<T> child in this.Children)
            {
                yield return child.Value;

                foreach (IEnumerable<T> childValues in child.Children.AsEnumerable())
                {
                    foreach (T value in childValues)
                    {
                        yield return value;
                    }
                }
            }
        }

        /// <summary>
        /// Removes the supplied <see cref="TreeNode{T}"/> from the current node's children.
        /// </summary>
        /// <param name="child">
        /// The node to remove from the children. Cannot be <see langword="null"/>.
        /// </param>
        public void Remove(TreeNode<T> child)
        {
            if (child == null)
            {
                throw new ArgumentException("Child to remove must be non-null.", nameof(child));
            }

            child.Parent = null;
            this.innerChildren.Remove(child);
        }

        /// <summary>
        /// Explicit implementation of GetEnumerator().
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerator"/> representing this element's value, and a depth-first result of children's values.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
