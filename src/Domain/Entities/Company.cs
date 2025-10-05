namespace Domain.Entities
{
    public class Company : Entity
    {
        /// <summary>
        /// Name of the company
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// List of stores associated with the company
        /// </summary>
        public ICollection<Store> Stores { get; set; } = [];
    }
}