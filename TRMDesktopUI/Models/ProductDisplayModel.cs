using System.ComponentModel;

namespace TRMUI.Models
{
	public class ProductDisplayModel : INotifyPropertyChanged
	{
		public int Id { get; set; }
		public string ProductName { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public decimal RetailPrice { get; set; }

		private int _quantityInStock;

		public int QuantityInStock
		{
			get
			{
				return _quantityInStock;
			}
			set
			{
				_quantityInStock = value;
				CallPropertyChanged(nameof(QuantityInStock));
			}
		}

		public bool IsTaxable { get; set; }

		public event PropertyChangedEventHandler? PropertyChanged;

		public void CallPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
