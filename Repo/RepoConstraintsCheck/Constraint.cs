using Repo;

namespace RepoConstraintsCheck
{
    public class Constraint
    {
        public INode Root { get; set; }
        public IModel Tree { get; set; }
        public int UnitHash { get; set; }
        public string Name { get; private set; }
        public Constraint(string name, INode root, IModel tree)
        {
            Root = root;
            Tree = tree;
            Name = name;
            UnitHash = name.GetHashCode();
        }
    }
}
