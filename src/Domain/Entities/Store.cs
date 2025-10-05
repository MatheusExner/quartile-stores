namespace Domain.Entities;

public class Store : Entity
{

	/// <summary>
	/// Name of the store
	/// </summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// Street address of the store
	/// </summary>
	public string Address { get; set; } = string.Empty;

	/// <summary>
	/// City where the store is located
	/// </summary>
	public string City { get; set; } = string.Empty;

	/// <summary>
	/// Country where the store is located
	/// </summary>
	public string Country { get; set; } = string.Empty;

	/// <summary>
	/// ID of the company that owns the store
	/// </summary>
	public Guid CompanyId { get; set; }

	/// <summary>
	/// The company that owns the store
	/// </summary>
	public virtual Company Company { get; set; } = null!;
}
