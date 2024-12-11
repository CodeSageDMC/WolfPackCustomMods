namespace Eco.Client.Utils
{
    using System;

    public static class UnityMigrationHelper
    {
        public static void MigrateMe(string code) => throw new NotSupportedException($"Unity version after upgrade doesn't support that code and require migration: {code}");

        public static TReturn MigrateMe<TReturn>(string code)
        {
            MigrateMe(code);
            return default;
        }
    }
}
