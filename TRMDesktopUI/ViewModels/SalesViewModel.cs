using System.ComponentModel;
using System.Dynamic;
using System.Windows;

using AutoMapper;

using Caliburn.Micro;

using Microsoft.Extensions.Configuration;

using TRMDesktopUI.Library.Api;
using TRMDesktopUI.Library.Models;
using TRMDesktopUI.Models;

namespace TRMDesktopUI.ViewModels
{
	public class SalesViewModel(IProductEndpoint productEndpoint, IConfiguration config, ISaleEndpoint saleEndpoint, IMapper mapper, StatusInfoViewModel status, IWindowManager window) : Screen
	{
		private readonly IProductEndpoint _productEndpoint = productEndpoint;
		private readonly IConfiguration _config = config;
		private readonly ISaleEndpoint _saleEndpoint = saleEndpoint;
		private readonly IMapper _mapper = mapper;
		private readonly StatusInfoViewModel _status = status;
		private readonly IWindowManager _window = window;
		private BindingList<ProductDisplayModel> _products = [];
		private ProductDisplayModel? _selectedProduct;
		private CartItemDisplayModel? _selectedCartItem;
		private BindingList<CartItemDisplayModel> _cart = [];
		private int _itemQuantity = 1;

		public BindingList<ProductDisplayModel> Products
		{
			get
			{
				return _products;
			}
			set
			{
				_products = value;
				NotifyOfPropertyChange(() => Products);
			}
		}

		public ProductDisplayModel? SelectedProduct
		{
			get
			{
				return _selectedProduct;
			}
			set
			{
				_selectedProduct = value;
				NotifyOfPropertyChange(() => SelectedProduct);
				NotifyOfPropertyChange(() => CanAddToCart);
			}
		}

		public CartItemDisplayModel? SelectedCartItem
		{
			get
			{
				return _selectedCartItem;
			}
			set
			{
				_selectedCartItem = value;
				NotifyOfPropertyChange(() => SelectedCartItem);
				NotifyOfPropertyChange(() => CanRemoveFromCart);
			}
		}

		public BindingList<CartItemDisplayModel> Cart
		{
			get
			{
				return _cart;
			}
			set
			{
				_cart = value;
				NotifyOfPropertyChange(() => Cart);
			}
		}

		public int ItemQuantity
		{
			get
			{
				return _itemQuantity;
			}
			set
			{
				_itemQuantity = value;
				NotifyOfPropertyChange(() => ItemQuantity);
				NotifyOfPropertyChange(() => CanAddToCart);
			}
		}

		public string SubTotal
		{
			get
			{
				return CalculateSubTotal().ToString("C");
			}
		}

		public string Tax
		{
			get
			{
				return CalculateTax().ToString("C");
			}
		}

		public string Total
		{
			get
			{
				decimal total = CalculateSubTotal() + CalculateTax();
				return total.ToString("C");
			}
		}

		public bool CanAddToCart
		{
			get
			{
				bool output = false;

				// Make sure something is selected
				// Make sure there is an item quantity
				if ( ItemQuantity > 0 && SelectedProduct?.QuantityInStock >= ItemQuantity )
				{
					output = true;
				}

				return output;
			}
		}

		public bool CanRemoveFromCart
		{
			get
			{
				bool output = false;

				// Make sure something is selected
				if ( SelectedCartItem != null && SelectedCartItem?.QuantityInCart > 0 )
				{
					output = true;
				}

				return output;
			}
		}

		public bool CanCheckOut
		{
			get
			{
				bool output = false;

				// Make sure there is something in the cart
				if ( Cart.Count > 0 )
				{
					output = true;
				}

				return output;
			}
		}

		protected override async void OnViewLoaded(object view)
		{
			base.OnViewLoaded(view);
			try
			{
				await LoadProductsAsync();
			}
			catch ( Exception ex )
			{
				dynamic settings = new ExpandoObject();
				settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
				settings.ResizeMode = ResizeMode.NoResize;
				settings.Title = "System Error";

				if ( ex.Message == "Unauthorized" )
				{
					_status.UpdateMessage("Unauthorized Access", "You do not have permission to interact with the Sales Form.");
					await _window.ShowDialogAsync(_status, null, settings);
				}
				else
				{
					_status.UpdateMessage("Fatal Exception", ex.Message);
					await _window.ShowDialogAsync(_status, null, settings);
				}

				await TryCloseAsync();
			}
		}

		private async Task LoadProductsAsync()
		{
			List<ProductModel> productList = await _productEndpoint.GetAll();
			List<ProductDisplayModel> products = _mapper.Map<List<ProductDisplayModel>>(productList);
			Products = new BindingList<ProductDisplayModel>(products);
		}

		private async Task ResetSalesViewModelAsync()
		{
			Cart = [];
			// TODO - Add clearing the selectedCartItem if it does not do it itself
			await LoadProductsAsync();

			NotifyOfPropertyChange(() => SubTotal);
			NotifyOfPropertyChange(() => Tax);
			NotifyOfPropertyChange(() => Total);
			NotifyOfPropertyChange(() => CanCheckOut);
		}

		private decimal CalculateSubTotal()
		{
			decimal subTotal = 0;

			foreach ( CartItemDisplayModel item in Cart )
			{
				subTotal += item.Product.RetailPrice * item.QuantityInCart;
			}

			return subTotal;
		}

		private decimal CalculateTax()
		{
			decimal taxAmount = 0;
			decimal taxRate = _config.GetValue<decimal>("taxRate") / 100;

			taxAmount = Cart
				.Where(x => x.Product.IsTaxable)
				.Sum(x => x.Product.RetailPrice * x.QuantityInCart * taxRate);

			return taxAmount;
		}

		public void AddToCart()
		{
			if ( SelectedProduct is not null )
			{
				CartItemDisplayModel? existingItem = Cart.FirstOrDefault(x => x.Product == SelectedProduct);

				if ( existingItem != null )
				{
					existingItem.QuantityInCart += ItemQuantity;
				}
				else
				{
					CartItemDisplayModel item = new()
					{
						Product = SelectedProduct,
						QuantityInCart = ItemQuantity
					};
					Cart.Add(item);
				}

				SelectedProduct.QuantityInStock -= ItemQuantity;
				ItemQuantity = 1;
				NotifyOfPropertyChange(() => SubTotal);
				NotifyOfPropertyChange(() => Tax);
				NotifyOfPropertyChange(() => Total);
				NotifyOfPropertyChange(() => CanCheckOut);
			}
		}

		public void RemoveFromCart()
		{
			if ( SelectedCartItem is not null )
			{
				SelectedCartItem.Product.QuantityInStock += 1;

				if ( SelectedCartItem.QuantityInCart > 1 )
				{
					SelectedCartItem.QuantityInCart -= 1;
				}
				else
				{
					_ = Cart.Remove(SelectedCartItem);
				}

				NotifyOfPropertyChange(() => SubTotal);
				NotifyOfPropertyChange(() => Tax);
				NotifyOfPropertyChange(() => Total);
				NotifyOfPropertyChange(() => CanCheckOut);
				NotifyOfPropertyChange(() => CanAddToCart);
			}
		}

		public async Task CheckOutAsync()
		{
			// Create a SaleModel and post to the API
			SaleModel sale = new();

			foreach ( CartItemDisplayModel item in Cart )
			{
				sale.SaleDetails.Add(new SaleDetailModel
				{
					ProductId = item.Product.Id,
					Quantity = item.QuantityInCart
				});
			}

			await _saleEndpoint.PostSale(sale);

			await ResetSalesViewModelAsync();
		}
	}
}
