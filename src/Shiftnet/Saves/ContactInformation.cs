namespace Shiftnet.Saves
{
    public class ContactInformation
    {
        private Npc _npc;
        private Contact _contact;

        public ContactInformation(Contact contact, Npc npc)
        {
            _contact = contact;
            _npc = npc;
        }

        public string FullName => _npc.FullName;
        public string Username => $"@{_npc.Username}";
    }
}