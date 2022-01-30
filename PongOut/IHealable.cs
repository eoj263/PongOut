namespace PongOut
{
    public interface IHealable
    {
        /// <summary>
        /// Heal a thing
        /// </summary>
        /// <param name="ammount">The ammount to heal</param>
        /// <returns>The ammount that was healed</returns>
        float Heal(float ammount);
    }
}
