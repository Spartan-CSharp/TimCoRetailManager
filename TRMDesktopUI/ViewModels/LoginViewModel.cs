using Caliburn.Micro;

using TRMDesktopUI.EventModels;
using TRMDesktopUI.Library.Api;

namespace TRMDesktopUI.ViewModels
{
	public class LoginViewModel(IAPIHelper apiHelper, IEventAggregator events) : Screen
	{
		private readonly IAPIHelper _apiHelper = apiHelper;
		private readonly IEventAggregator _events = events;
		private string _userName = "tim@iamtimcorey.com";
		private string _password = "Pwd12345.";
		private string _errorMessage = string.Empty;

		public string UserName
		{
			get
			{
				return _userName;
			}
			set
			{
				_userName = value;
				NotifyOfPropertyChange(() => UserName);
				NotifyOfPropertyChange(() => CanLogIn);
			}
		}

		public string Password
		{
			get
			{
				return _password;
			}
			set
			{
				_password = value;
				NotifyOfPropertyChange(() => Password);
				NotifyOfPropertyChange(() => CanLogIn);
			}
		}

		public bool IsErrorVisible
		{
			get
			{
				bool output = false;

				if ( ErrorMessage?.Length > 0 )
				{
					output = true;
				}

				return output;
			}
		}

		public string ErrorMessage
		{
			get
			{
				return _errorMessage;
			}
			set
			{
				_errorMessage = value;
				NotifyOfPropertyChange(() => IsErrorVisible);
				NotifyOfPropertyChange(() => ErrorMessage);
			}
		}

		public bool CanLogIn
		{
			get
			{
				bool output = false;

				if ( UserName?.Length > 0 && Password?.Length > 0 )
				{
					output = true;
				}

				return output;
			}
		}

		public async Task LogInAsync()
		{
			try
			{
				ErrorMessage = "";
				Library.Models.AuthenticatedUser result = await _apiHelper.Authenticate(UserName, Password);

				// Capture more information about the user
				await _apiHelper.GetLoggedInUserInfo(result.Access_Token);

				await _events.PublishOnUIThreadAsync(new LogOnEvent(), new CancellationToken());
			}
			catch ( Exception ex )
			{
				ErrorMessage = ex.Message;
			}
		}
	}
}
