using System.ComponentModel;

namespace TRMUI.Models
{
	public class CartItemDisplayModel : INotifyPropertyChanged
	{
		public ProductDisplayModel Product { get; set; } = new ProductDisplayModel();

		private int _quantityInCart;

		public int QuantityInCart
		{
			get
			{
				return _quantityInCart;
			}
			set
			{
				_quantityInCart = value;
				CallPropertyChanged(nameof(QuantityInCart));
				CallPropertyChanged(nameof(DisplayText));
			}
		}

		public string DisplayText
		{
			get
			{
				return $"{Product.ProductName} ({QuantityInCart})";
			}
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		public void CallPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
