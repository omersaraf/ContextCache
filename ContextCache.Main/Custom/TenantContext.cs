namespace ContextCache.Main.Custom
{
    public class TenantContext : ICacheContext
    {
        public string TenantId { get; }

        public TenantContext(string tenantId)
        {
            TenantId = tenantId;
        }
    }
}