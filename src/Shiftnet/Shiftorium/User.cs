using AlkalineThunder.Pandemic;

namespace Shiftnet.Shiftorium
{
    public class User
    {
        public User(PropertySet json)
        {
            
        }
        
        public string FirstName { get; }
        public string LastName { get; }
        public string Username { get; }
        public string Color { get; }
        public Upload Avatar { get; }
    }
}