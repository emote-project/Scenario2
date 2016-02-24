using System;
using EmoteEnercitiesMessages;
using EmoteEvents;

namespace EnercitiesAI.AI
{
    /// <summary>
    ///     Represents an AI player within the game.
    ///     It has a role and a strategy that defines  its decisions towards game states.
    /// </summary>
    public class Player : IDisposable
    {
        public Player(EnercitiesRole role, Strategy initialStrategy)
        {
            this.Role = role;
            this.Strategy = initialStrategy;
        }

        public EnercitiesRole Role { get; set; }

        public Strategy Strategy { get; set; }

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion

        public override string ToString()
        {
            return string.Format("role: {0}, strat: {1}", this.Role, this.Strategy);
        }
    }
}