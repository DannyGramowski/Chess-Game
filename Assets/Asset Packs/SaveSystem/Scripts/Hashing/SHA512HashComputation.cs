
namespace SaveSystem.Hashing
{
    public class SHA512HashComputation : HashingScript
    {
        #region CONSTRUCTOR
        public SHA512HashComputation() : base()
        {
            hashAlgorithm = System.Security.Cryptography.SHA512.Create();
        }
        #endregion
    }
}
