namespace TRMDataManager.Library.Models
{
	public class SaleReportModel
	{
		public DateTime SaleDate { get; set; }
		public decimal SubTotal { get; set; }
		public decimal Tax { get; set; }
		public decimal Total { get; set; }
		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public string EmailAddress { get; set; } = string.Empty;
	}
}
