using System;
using System.Collections.Generic;
using System.Text;
using Repo;

namespace CheckingConstraints
{
  public class Constraint
  {
    public INode Root { get; private set; }
    public IModel Tree { get; private set; }
    public int UnitHash { get; private set; }
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
