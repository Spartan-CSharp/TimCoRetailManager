using TRMCommon.Library.Models;

namespace TRMUI.Library.Models
{
	public class CartItemModel
	{
		public ProductModel Product { get; set; } = new ProductModel();
		public int QuantityInCart { get; set; }
	}
}
