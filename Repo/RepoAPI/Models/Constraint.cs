namespace RepoAPI.Models
{
  public class Constraint
  {
    public Node Root { get; set; }
    public Model Tree { get; set; }
    public string Name { get; private set; }
    public int UnitHash { get; set; }
  }
}
