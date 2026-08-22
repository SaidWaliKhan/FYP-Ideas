using CrispyKitchen.Application.Common.Interfaces;

namespace CrispyKitchen.Infrastructure.Security;

/// <summary>
/// Real-world analogy: hashing is like putting a document through a
/// paper shredder in a very specific, repeatable pattern. You can't
/// un-shred it back into the original password (one-way), but if you
/// feed the SAME original document through the SAME shredder again,
/// you get matching shredded output — that's how Verify() works,
/// it never "decrypts," it just re-shreds and compares.
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    // workFactor 12 = how many times the hash algorithm loops internally.
    // Higher = slower to compute = harder to brute-force. 12 is a solid
    // default in 2026 — high enough to matter, low enough not to make
    // login noticeably slow.
    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);

    public bool Verify(string password, string passwordHash) => BCrypt.Net.BCrypt.Verify(password, passwordHash);
}