namespace TRMCommon.Library.Models
{
	public class SaleModel
	{
		public int Id { get; set; }
		public string CashierId { get; set; } = string.Empty;
		public DateTime SaleDate { get; set; }
		public decimal SubTotal { get; set; }
		public decimal Tax { get; set; }
		public decimal Total { get; set; }
		public List<SaleDetailModel> SaleDetails { get; set; } = [];
	}
}
