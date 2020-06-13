using System;
using System.Collections.Generic;
using System.Text;

namespace ZanyStreams
{
    public interface IScopeProvider
    {
        void SetScopeName(string scopeId);

        string GetScopeName();
    }

    public class ScopeProvider : IScopeProvider
    {
        private string scopeName;

        public string GetScopeName()
        {
            if (this.scopeName == null)
                throw new Exception("Tried to retrieve scope id before setting it");

            return this.scopeName;
        }

        public void SetScopeName(string scopeId)
        {
            if (string.IsNullOrEmpty(scopeId))
            {
                throw new ArgumentException("scopeId must not be null or empty", nameof(scopeId));
            }

            this.scopeName = scopeId;
        }
    }
}
