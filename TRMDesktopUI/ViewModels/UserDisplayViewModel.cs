using System.ComponentModel;
using System.Dynamic;
using System.Windows;

using Caliburn.Micro;

using TRMDesktopUI.Library.Api;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.ViewModels
{
	public class UserDisplayViewModel(StatusInfoViewModel status, IWindowManager window, IUserEndpoint userEndpoint) : Screen
	{
		private readonly StatusInfoViewModel _status = status;
		private readonly IWindowManager _window = window;
		private readonly IUserEndpoint _userEndpoint = userEndpoint;
		private BindingList<UserModel> _users = [];
		private UserModel? _selectedUser;
		private string? _selectedUserRole;
		private string? _selectedAvailableRole;
		private string? _selectedUserName;
		private BindingList<string> _userRoles = [];
		private BindingList<string> _availableRoles = [];

		public BindingList<UserModel> Users
		{
			get
			{
				return _users;
			}
			set
			{
				_users = value;
				NotifyOfPropertyChange(() => Users);
			}
		}

		public UserModel? SelectedUser
		{
			get
			{
				return _selectedUser;
			}
			set
			{
				_selectedUser = value;
				SelectedUserName = value?.Email;
				UserRoles = value is not null ? new BindingList<string>(value.Roles.Select(x => x.Value).ToList()) : ([]);
				//TODO - Pull this out into a method/event
				_ = LoadRolesAsync();
				NotifyOfPropertyChange(() => SelectedUser);
			}
		}

		public string? SelectedUserRole
		{
			get
			{
				return _selectedUserRole;
			}
			set
			{
				_selectedUserRole = value;
				NotifyOfPropertyChange(() => SelectedUserRole);
				NotifyOfPropertyChange(() => CanRemoveSelectedRole);
			}
		}

		public string? SelectedAvailableRole
		{
			get
			{
				return _selectedAvailableRole;
			}
			set
			{
				_selectedAvailableRole = value;
				NotifyOfPropertyChange(() => SelectedAvailableRole);
				NotifyOfPropertyChange(() => CanAddSelectedRole);
			}
		}

		public string? SelectedUserName
		{
			get
			{
				return _selectedUserName;
			}
			set
			{
				_selectedUserName = value;
				NotifyOfPropertyChange(() => SelectedUserName);
			}
		}

		public BindingList<string> UserRoles
		{
			get
			{
				return _userRoles;
			}
			set
			{
				_userRoles = value;
				NotifyOfPropertyChange(() => UserRoles);
			}
		}

		public BindingList<string> AvailableRoles
		{
			get
			{
				return _availableRoles;
			}
			set
			{
				_availableRoles = value;
				NotifyOfPropertyChange(() => AvailableRoles);
			}
		}

		public bool CanAddSelectedRole
		{
			get
			{
				return SelectedUser is not null && SelectedAvailableRole is not null;
			}
		}

		public bool CanRemoveSelectedRole
		{
			get
			{
				return SelectedUser is not null && SelectedUserRole is not null;
			}
		}

		protected override async void OnViewLoaded(object view)
		{
			base.OnViewLoaded(view);
			try
			{
				await LoadUsersAsync();
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

		private async Task LoadUsersAsync()
		{
			List<UserModel> userList = await _userEndpoint.GetAll();
			Users = new BindingList<UserModel>(userList);
		}

		private async Task LoadRolesAsync()
		{
			Dictionary<string, string> roles = await _userEndpoint.GetAllRoles();

			AvailableRoles.Clear();

			foreach ( KeyValuePair<string, string> role in roles )
			{
				if ( UserRoles.IndexOf(role.Value) < 0 )
				{
					AvailableRoles.Add(role.Value);
				}
			}
		}

		public async void AddSelectedRoleAsync()
		{
			if ( SelectedUser is not null && SelectedAvailableRole is not null )
			{
				await _userEndpoint.AddUserToRole(SelectedUser.Id, SelectedAvailableRole);

				UserRoles.Add(SelectedAvailableRole);
				_ = AvailableRoles.Remove(SelectedAvailableRole);
			}
		}

		public async void RemoveSelectedRoleAsync()
		{
			if ( SelectedUser is not null && SelectedUserRole is not null )
			{
				await _userEndpoint.RemoveUserFromRole(SelectedUser.Id, SelectedUserRole);

				AvailableRoles.Add(SelectedUserRole);
				_ = UserRoles.Remove(SelectedUserRole);
			}
		}
	}
}
