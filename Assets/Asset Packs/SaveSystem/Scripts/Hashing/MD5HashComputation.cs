
namespace SaveSystem.Hashing
{
    public class MD5HashComputation : HashingScript
    {
        #region CONSTRUCTOR
        public MD5HashComputation() : base()
        {
            hashAlgorithm = System.Security.Cryptography.MD5.Create();
        }
        #endregion
    }
}
