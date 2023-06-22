namespace MinotaurLabyrinth
{
    /// <summary>
    /// Represents an abstract base class for the various monster types in the game.
    /// </summary>
    public abstract class Monster : IActivatable
    {
        /// <summary>
        /// Gets or sets a value indicating whether the monster is alive.
        /// </summary>
        public bool IsAlive { get; set; } = true;

        /// <summary>
        /// Activates the monster when the hero and the monster are both in the same room.
        /// Gives the monster a chance to perform its actions.
        /// </summary>
        /// <param name="hero">The hero encountering the monster.</param>
        /// <param name="map">The current game map.</param>
        public abstract void Activate(Hero hero, Map map);

        /// <summary>
        /// Displays sensory information about the monster, based on the hero's distance from it.
        /// </summary>
        /// <param name="hero">The hero sensing the monster.</param>
        /// <param name="heroDistance">The distance between the hero and the monster.</param>
        /// <returns>Returns true if a message was displayed; otherwise, false.</returns>
        public abstract bool DisplaySense(Hero hero, int heroDistance);

        /// <summary>
        /// Displays the current state of the monster.
        /// </summary>
        /// <returns>Returns a DisplayDetails object containing the monster's display information.</returns>
        public abstract DisplayDetails Display(bool isOccupyInADestoryedRoom = false);
    }
}
