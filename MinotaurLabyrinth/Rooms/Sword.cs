namespace MinotaurLabyrinth
{
    public class Sword : Room
    {
        static Sword()
        {
            RoomFactory.Instance.Register(RoomType.Sword, () => new Sword());
        }
        public override RoomType Type { get; } = RoomType.Sword;

        //need to set default state is active.
        public override bool IsActive { get; protected set; } = true;

        //update the display, when it is not active, then show the base class display method
        public override DisplayDetails Display()
        {
            return new DisplayDetails($"[{Type.ToString()[0]}]", ConsoleColor.Yellow);
        }

        public override bool DisplaySense(Hero hero, int heroDistance)
        {
            if (heroDistance == 0)
            {
                if (hero.HasSword) ConsoleHelper.WriteLine("This is the sword room but you've already picked up the sword!", ConsoleColor.DarkCyan);
                else ConsoleHelper.WriteLine("You can sense that the sword is nearby!", ConsoleColor.DarkCyan);
                return true;
            }
            return false;
        }
    }
}
