using System.Collections.Concurrent;

namespace Diplomski.Helpers
{
    public class VerifyLoginCodeVerification
    {
        private readonly ConcurrentDictionary<string, (string Code, DateTime Expiration)> _store
        = new ConcurrentDictionary<string, (string, DateTime)>();

        public void StoreCode(string username, string code, TimeSpan expiration)
        {
            var expirationTime = DateTime.UtcNow.Add(expiration);
            _store[username] = (code, expirationTime);
        }

        public string GetUsernameByCode(string code)
        {
            foreach (var kvp in _store)
            {
                if (kvp.Value.Code == code && kvp.Value.Expiration > DateTime.UtcNow)
                {
                    return kvp.Key;
                }
            }
            throw new Exception("Code not valid or expired");
        }

        public bool ValidateCode(string username, string code)
        {
            if (_store.TryGetValue(username, out var storedCode))
            {
                if (storedCode.Code == code && storedCode.Expiration > DateTime.UtcNow)
                {
                    _store.TryRemove(username, out _);
                    return true;
                }
            }
            return false;
        }

    }
}
