using System.Collections.Concurrent;

namespace Diplomski.Helpers
{
    public class ResetPasswordVerification
    {
        private readonly ConcurrentDictionary<string, (byte[] emailHash, DateTime Expiration)> _store
        = new ConcurrentDictionary<string, (byte[], DateTime)>();

        public void StoreEmailHash(string username, byte[] emailHash, TimeSpan expiration)
        {
            var expirationTime = DateTime.UtcNow.Add(expiration);
            _store[username] = (emailHash, expirationTime);
        }

        public string GetUsernameByEmailHash(byte[] emailHash)
        {
            foreach (var kvp in _store)
            {
                if (kvp.Value.emailHash.SequenceEqual(emailHash) && kvp.Value.Expiration > DateTime.UtcNow)
                {
                    return kvp.Key;
                }
            }
            throw new Exception("Email not valid or expired");
        }

        public bool ValidateEmailHash(string username, byte[] emailHash)
        {
            if (_store.TryGetValue(username, out var storedCode))
            {
                if (storedCode.emailHash.SequenceEqual(emailHash) && storedCode.Expiration > DateTime.UtcNow)
                {
                    _store.TryRemove(username, out _);
                    return true;
                }
            }
            return false;
        }
    }
}
