namespace TRMDataManager.Library.Models
{
	public class ProductModel
	{
		public int Id { get; set; }
		public string ProductName { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public decimal RetailPrice { get; set; }
		public int QuantityInStock { get; set; }
		public bool IsTaxable { get; set; }
		public string? ProductImage { get; set; }
	}
}
