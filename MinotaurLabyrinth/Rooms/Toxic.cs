namespace MinotaurLabyrinth
{
    /// <summary>
    /// Represents a Toxic room, which make the hero can play at most 3 steps.
    /// </summary>
    public class Toxic : Room
    {
        const int MAX_PLAY_STEPS = 3;
        static Toxic()
        {
            RoomFactory.Instance.Register(RoomType.Toxic, () => new Toxic());
        }

        /// <inheritdoc/>
        public override RoomType Type { get; } = RoomType.Toxic;

        /// <inheritdoc/>
        public override bool IsActive { get; protected set; } = true;

        /// <summary>
        /// Activates the Toxic, causing the hero can play at most 3 steps.
        /// </summary>
        public override void Activate(Hero hero, Map map)
        {
            if (IsActive)
            {
                ConsoleHelper.WriteLine("You walk into the Toxic room, and you have to play a dice game.If you win, nothing happen, If you lost, you would get punishment!", ConsoleColor.Red);
                ConsoleHelper.WriteLine("Choose large, small, after that will dice 3 dices, and calculate the result.", ConsoleColor.Gray);
                bool smallFlag = Console.ReadLine() switch
                {
                    "small" => true,
                    "large" => false,
                    _ => false
                };

                bool winFlag = false;
                int point = Dice3Dices();
                if (smallFlag)
                {
                    winFlag = point <= 11;
                }
                else
                {
                    winFlag = point >= 12;
                }

                if (winFlag)
                {
                    ConsoleHelper.WriteLine("Congrat you win the dice game, nothing will happen.", ConsoleColor.Yellow);
                }
                else
                {
                    ConsoleHelper.WriteLine($"Be careful, you can play at most {MAX_PLAY_STEPS} steps from now.", ConsoleColor.Red);
                    hero.IsPoisoned = true;
                    hero.StillCanPlaySteps = MAX_PLAY_STEPS;

                    //set the callback for the game to call when game ended.
                    //because this room already finish, but the game keep going.
                    hero.Callback = HandleCauseDeth;
                }

                IsActive = false;
            }
        }

        /// <inheritdoc/>
        public override DisplayDetails Display()
        {
            return IsActive ? new DisplayDetails($"[{Type.ToString()[0]}]", ConsoleColor.Red)
                            : base.Display();
        }

        /// <summary>
        /// Displays sensory information about the pit, based on the hero's distance from it.
        /// </summary>
        /// <param name="hero">The hero sensing the pit.</param>
        /// <param name="heroDistance">The distance between the hero and the pit room.</param>
        /// <returns>Returns true if a message was displayed; otherwise, false.</returns>
        public override bool DisplaySense(Hero hero, int heroDistance)
        {
            if (!IsActive)
            {
                if (base.DisplaySense(hero, heroDistance))
                {
                    return true;
                }
                if (heroDistance == 0)
                {
                    ConsoleHelper.WriteLine("You will experiece a terrible game in this room.", ConsoleColor.DarkGray);
                    return true;
                }
            }
            else if (heroDistance == 1 || heroDistance == 2)
            {
                ConsoleHelper.WriteLine(heroDistance == 1 ? "You feel a toxic. There is a toxic in a nearby room!" : "Your intuition tells you that something scary is nearby", ConsoleColor.DarkGray);
                return true;
            }
            return false;
        }

        private int Dice3Dices()
        {
            int number = 0;
            for (int i = 0; i < 3; ++i)
            {
                number += RandomNumberGenerator.Next(1, 6);
            }

            return number;
        }

        //handle call back when the game ended.
        public static void HandleCauseDeth(Hero hero)
        {
            hero.Kill("You have gotton a toxic and died.");
        }
    }
}
