namespace Bhp.VM
{
    public interface ICrypto
    {
        byte[] Hash160(byte[] message);

        byte[] Hash256(byte[] message);

        byte[] Sign(byte[] message, byte[] privateKey, byte[] publicKey);

        bool VerifySignature(byte[] message, byte[] signature, byte[] pubkey);
    }
}
