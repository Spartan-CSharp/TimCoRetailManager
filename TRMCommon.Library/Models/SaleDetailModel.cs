﻿namespace TRMCommon.Library.Models
{
	public class SaleDetailModel
	{
		public int Id { get; set; }
		public int SaleId { get; set; }
		public int ProductId { get; set; }
		public int Quantity { get; set; }
		public decimal PurchasePrice { get; set; }
		public decimal Tax { get; set; }
	}
}
