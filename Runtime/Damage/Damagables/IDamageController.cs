
namespace SF.DamageModule
{
    /// <summary>
    ///  Used to declare external controllers for health that calculate damage than sends it to the health script.
    /// </summary>
    public interface IDamageController : IDamage
    {
        public int Damage { get;}
        public int CalculateDamage(int damage);
    }
}