using EmoteEnercitiesMessages;

namespace CaseBasedController.GameInfo
{
    public class Player
    {
        public Gender Gender;
        public string Name;
        public EnercitiesRole Role;

        public Player(string name, EnercitiesRole role, Gender gender = Gender.Male)
        {
            this.Name = name;
            this.Role = role;
            this.Gender = gender;
        }

        public bool IsAI()
        {
            return Role == EnercitiesRole.Mayor;
        }

        public override string ToString()
        {
            return string.Format("'{0}':{1}", Name, Role);
        }
    }
}