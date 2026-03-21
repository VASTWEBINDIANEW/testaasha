using System.Data.Entity;

namespace Vastwebmulti.Models
{
    /// <summary>
    /// Partial class extension for VastwebmultiEntities DbContext.
    /// Provides performance configuration and a factory method for optimized (read-only) contexts.
    /// </summary>
    public partial class VastwebmultiEntities
    {
        /// <summary>
        /// Creates a context optimized for read-heavy (non-tracking) operations.
        /// Disables proxy creation and lazy loading to reduce memory overhead and prevent unintended N+1 queries.
        /// Use this for report pages, dashboards, and any action that only reads data without updating.
        /// </summary>
        /// <example>
        /// // Instead of: using (var db = new VastwebmultiEntities())
        /// // Use for read-only:
        /// using (var db = VastwebmultiEntities.CreateReadOnly())
        /// {
        ///     var data = db.Recharge_info.AsNoTracking().Where(...).ToList();
        /// }
        /// </example>
        public static VastwebmultiEntities CreateReadOnly()
        {
            var ctx = new VastwebmultiEntities();
            // Disable proxy creation — reduces memory by not creating dynamic proxy classes
            ctx.Configuration.ProxyCreationEnabled = false;
            // Disable lazy loading — prevents unintended N+1 queries on navigation properties
            ctx.Configuration.LazyLoadingEnabled = false;
            // Disable change tracking — significant memory/CPU savings for read-only queries
            ctx.Configuration.AutoDetectChangesEnabled = false;
            return ctx;
        }

        /// <summary>
        /// Sets the database command timeout (in seconds) for long-running queries.
        /// Default is 30 seconds. Increase for bulk export or heavy report queries.
        /// </summary>
        public void SetCommandTimeout(int seconds = 30)
        {
            this.Database.CommandTimeout = seconds;
        }
    }
}
