using System.Security.Cryptography;
using System.Text;

namespace BusReservationService.Util;

static public class Argon2Helper
{
    private const int SaltSize = 32; // 32バイト = 256ビット
    private const int HashSize = 32; // 32バイト = 256ビット
    private const int Iterations = 600000; // OWASP 2023推奨最小値(スペックにより変更)

    // パスワードをハッシュ化（登録時）
    public static (byte[] hash, byte[] salt) HashPassword(string password)
    {
        // ソルトを生成
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

        // PBKDF2でハッシュ化
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize
        );

        return (hash, salt);
    }



    // パスワードを検証（ログイン時）
    public static bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt)
    {
        // 入力されたパスワードを同じ方法でハッシュ化
        byte[] computedHash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            storedSalt,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize
        );

        // 固定時間比較（タイミング攻撃対策）
        return CryptographicOperations.FixedTimeEquals(computedHash, storedHash);
        // すべてのバイトを必ず比較するため、時間差が生じない
    }

}
