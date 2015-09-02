using System.Collections.Generic;

namespace Base
{
    public interface ITreeNode
    {
        string Name { get; }
        ITreeNode Parent { get; }
        IEnumerable<ITreeNode> Children { get; }
    }
}
