namespace Domain.Entities
{
    public class Product : Entity
    {
        /// <summary>
        /// Name of the product
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Price of the product
        /// </summary>
        public decimal Price { get; set; } = 0;

        /// <summary>
        /// Description of the product
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Foreign key to the Store entity
        /// </summary>
        public Guid StoreId { get; set; }   

        /// <summary>
        /// Navigation property to the Store entity
        /// </summary>
        public virtual Store Store { get; set; } = new Store();
    }
}