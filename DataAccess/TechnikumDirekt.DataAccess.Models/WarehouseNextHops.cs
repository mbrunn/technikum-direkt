namespace TechnikumDirekt.DataAccess.Models
{
    public class WarehouseNextHops
    {
        /// <summary>
        ///     Gets or Sets TraveltimeMins
        /// </summary>
        public int? TraveltimeMins { get; set; }

        /// <summary>
        ///     Gets or Sets Hop
        /// </summary>
        public Hop Hop { get; set; } //TODO not compatible with database
    }
}