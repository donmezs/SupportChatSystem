using SupportChatSystem.Domain.Exceptions;

namespace SupportChatSystem.Domain.Entities;
public class Team : BaseEntity
{
    public string Name { get; set; }
    public bool IsOverflow { get; set; }
    public virtual ICollection<Agent> Agents { get; set; }

    public Team(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new TeamNameInvalidException("Team name cannot be null or whitespace.");

        Name = name;
        Agents = new List<Agent>();
    }
}
