using cleddmessenger.Models;

namespace cleddmessenger.Utils
{
   public  class GroupManager
    {
        private List<Group> Groups = new List<Group>();

        public void CreateGroup(string groupName)
        {
            if (Groups.Any(g => g.GroupName == groupName))
                throw new Exception("Group name already exists.");

            Groups.Add(new Group { GroupName = groupName });
        }
  public void RenameGroup(string oldName, string newName)
    {
        var group = Groups.FirstOrDefault(g => g.GroupName == oldName);
        if (group == null)
            throw new Exception("Group not found.");

        group.GroupName = newName;
    }

 public void AddPeerToGroup(string groupName, Peer peerName)
    {
        var group = Groups.FirstOrDefault(g => g.GroupName == groupName);
        if (group == null)
            throw new Exception("Group not found.");

        if (!group.Members.Contains(peerName))
            group.Members.Add(peerName);
    }

    public void RemovePeerFromGroup(string groupName, Peer peerName)
    {
        var group = Groups.FirstOrDefault(g => g.GroupName == groupName);
        if (group == null)
            throw new Exception("Group not found.");

        group.Members.Remove(peerName);
    }
    public List<Group> GetGroups() => Groups;



    }
}


