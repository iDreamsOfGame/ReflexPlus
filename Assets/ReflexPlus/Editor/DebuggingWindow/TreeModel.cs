using System;
using System.Collections.Generic;
using System.Linq;

namespace ReflexPlusEditor.DebuggingWindow
{
    // The TreeModel is a utility class working on a list of serializable TreeElements where the order and the depth of each TreeElement define
    // the tree structure. Note that the TreeModel itself is not serializable (in Unity we are currently limited to serializing lists/arrays) but the 
    // input list is.
    // The tree representation (parent and children references) are then build internally using TreeElementUtility.ListToTree (using depth 
    // values of the elements). 
    // The first element of the input list is required to have depth == -1 (the hidden root) and the rest to have
    // depth >= 0 (otherwise an exception will be thrown)

    public class TreeModel<T> where T : TreeElement
    {
        private IList<T> _data;

        public T Root { get; private set; }

        public TreeModel(IList<T> data)
        {
            SetData(data);
        }

        public T Find(int id)
        {
            return _data.FirstOrDefault(element => element.Id == id);
        }

        private void SetData(IList<T> data)
        {
            Init(data);
        }

        private void Init(IList<T> data)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data), "Input data is null. Ensure input is a non-null list.");
            if (_data.Count > 0)
            {
                Root = TreeElementUtility.ListToTree(data);
            }
        }

        public IList<int> GetAncestors(int id)
        {
            var parents = new List<int>();
            TreeElement T = Find(id);
            if (T != null)
            {
                while (T.Parent != null)
                {
                    parents.Add(T.Parent.Id);
                    T = T.Parent;
                }
            }

            return parents;
        }

        public IList<int> GetDescendantsThatHaveChildren(int id)
        {
            var searchFromThis = Find(id);
            if (searchFromThis != null)
            {
                return GetParentsBelowStackBased(searchFromThis);
            }

            return new List<int>();
        }

        private static IList<int> GetParentsBelowStackBased(TreeElement searchFromThis)
        {
            var stack = new Stack<TreeElement>();
            stack.Push(searchFromThis);

            var parentsBelow = new List<int>();
            while (stack.Count > 0)
            {
                var current = stack.Pop();
                if (current.HasChildren)
                {
                    parentsBelow.Add(current.Id);
                    foreach (var T in current.Children)
                    {
                        stack.Push(T);
                    }
                }
            }

            return parentsBelow;
        }
    }
}