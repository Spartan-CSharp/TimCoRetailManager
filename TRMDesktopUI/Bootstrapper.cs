using System.IO;
using System.Windows;
using System.Windows.Controls;

using AutoMapper;

using Caliburn.Micro;

using Microsoft.Extensions.Configuration;

using TRMCommon.Library.Models;
using TRMUI.Helpers;
using TRMUI.Library.Api;
using TRMUI.Library.Models;
using TRMUI.Models;
using TRMUI.ViewModels;

namespace TRMUI
{
	public class Bootstrapper : BootstrapperBase
	{
		private readonly SimpleContainer _container = new SimpleContainer();

		public Bootstrapper()
		{
			Initialize();

			_ = ConventionManager.AddElementConvention<PasswordBox>(
			PasswordBoxHelper.BoundPasswordProperty,
			"Password",
			"PasswordChanged");
		}

		private static IMapper ConfigureAutomapper()
		{
			MapperConfiguration config = new MapperConfiguration(cfg =>
			{
				_ = cfg.CreateMap<ProductModel, ProductDisplayModel>();
				_ = cfg.CreateMap<CartItemModel, CartItemDisplayModel>();
			});

			IMapper output = config.CreateMapper();

			return output;
		}

		private static IConfiguration AddConfiguration()
		{
			IConfigurationBuilder builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json");

#if DEBUG
			builder.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
#else
			builder.AddJsonFile("appsettings.Production.json", optional: true, reloadOnChange: true);
#endif
			return builder.Build();
		}

		protected override void Configure()
		{
			_ = _container.Instance(ConfigureAutomapper());

			_ = _container.Instance(_container)
				.PerRequest<IProductEndpoint, ProductEndpoint>()
				.PerRequest<IUserEndpoint, UserEndpoint>()
				.PerRequest<ISaleEndpoint, SaleEndpoint>();

			_ = _container
				.Singleton<IWindowManager, WindowManager>()
				.Singleton<IEventAggregator, EventAggregator>()
				.Singleton<ILoggedInUserModel, LoggedInUserModel>()
				.Singleton<IAPIHelper, APIHelper>();

			_container.RegisterInstance(typeof(IConfiguration), "IConfiguration", AddConfiguration());

			GetType().Assembly.GetTypes()
				.Where(type => type.IsClass)
				.Where(type => type.Name.EndsWith("ViewModel"))
				.ToList()
				.ForEach(viewModelType => _container.RegisterPerRequest(
					viewModelType, viewModelType.ToString(), viewModelType));
		}

		protected override void OnStartup(object sender, StartupEventArgs e)
		{
			_ = DisplayRootViewForAsync<ShellViewModel>();
		}

		protected override object GetInstance(Type service, string key)
		{
			return _container.GetInstance(service, key);
		}

		protected override IEnumerable<object> GetAllInstances(Type service)
		{
			return _container.GetAllInstances(service);
		}

		protected override void BuildUp(object instance)
		{
			_container.BuildUp(instance);
		}
	}
}
