using System.Configuration;

using Microsoft.Extensions.Configuration;

using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess
{
	public class SaleData(IProductData productData, ISqlDataAccess sql, IConfiguration config) : ISaleData
	{
		private readonly IProductData _productData = productData;
		private readonly ISqlDataAccess _sql = sql;
		private readonly IConfiguration _config = config;

		public decimal GetTaxRate()
		{
			string rateText = _config.GetValue<string>("TaxRate") ?? throw new InvalidOperationException("AppSetting 'TaxRate' not found.");

			bool isValidTaxRate = decimal.TryParse(rateText, out decimal output);

			if ( isValidTaxRate == false )
			{
				throw new ConfigurationErrorsException("The tax rate is not set up properly");
			}

			output /= 100;

			return output;
		}

		public void SaveSale(SaleModel saleInfo, string cashierId)
		{
			//TODO: Make this SOLID/DRY/Better
			// Start filling in the sale detail models we will save to the database
			List<SaleDetailDBModel> details = [];
			decimal taxRate = GetTaxRate();

			foreach ( SaleDetailModel item in saleInfo.SaleDetails )
			{
				SaleDetailDBModel detail = new SaleDetailDBModel
				{
					ProductId = item.ProductId,
					Quantity = item.Quantity
				};

				// Get the information about this product
				ProductModel productInfo = _productData.GetProductById(detail.ProductId) ?? throw new Exception($"The product Id of {detail.ProductId} could not be found in the database.");

				detail.PurchasePrice = productInfo.RetailPrice * detail.Quantity;

				if ( productInfo.IsTaxable )
				{
					detail.Tax = detail.PurchasePrice * taxRate;
				}

				details.Add(detail);
			}

			// Create the Sale model
			SaleDBModel sale = new SaleDBModel
			{
				SubTotal = details.Sum(x => x.PurchasePrice),
				Tax = details.Sum(x => x.Tax),
				CashierId = cashierId
			};

			sale.Total = sale.SubTotal + sale.Tax;

			try
			{
				_sql.StartTransaction("TRMData");

				// Save the sale model
				_sql.SaveDataInTransaction("dbo.spSale_Insert", sale);

				// Get the ID from the sale mode
				sale.Id = _sql.LoadDataInTransaction<int, dynamic>("spSale_Lookup", new { sale.CashierId, sale.SaleDate }).FirstOrDefault();

				// Finish filling in the sale detail models
				foreach ( SaleDetailDBModel item in details )
				{
					item.SaleId = sale.Id;
					// Save the sale detail models
					_sql.SaveDataInTransaction("dbo.spSaleDetail_Insert", item);
				}

				_sql.CommitTransaction();
			}
			catch
			{
				_sql.RollbackTransaction();
				throw;
			}
		}

		public List<SaleReportModel> GetSaleReport()
		{
			List<SaleReportModel> output = _sql.LoadData<SaleReportModel, dynamic>("dbo.spSale_SaleReport", new { }, "TRMData");

			return output;
		}
	}
}
