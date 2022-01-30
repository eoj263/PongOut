namespace PongOut
{
    public interface IDamageable
    {
        /// <summary>
        /// Damage the object
        /// </summary>
        /// <param name="ammount"></param>
        /// <returns>True if the damage was given, false if not</returns>
        bool Damage(float ammount);
    }
}
