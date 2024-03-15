using System;

namespace TRMDataManager.Library.Models
{
	public class SaleDBModel
	{
		public int Id { get; set; }
		public string CashierId { get; set; } = string.Empty;
		public DateTime SaleDate { get; set; } = DateTime.UtcNow;
		public decimal SubTotal { get; set; }
		public decimal Tax { get; set; }
		public decimal Total { get; set; }
	}
}
