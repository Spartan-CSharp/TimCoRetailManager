using Caliburn.Micro;

namespace TRMUI.ViewModels
{
	public class StatusInfoViewModel : Screen
	{
		public string Header { get; private set; } = string.Empty;
		public string Message { get; private set; } = string.Empty;

		public void UpdateMessage(string header, string message)
		{
			Header = header;
			Message = message;

			NotifyOfPropertyChange(() => Header);
			NotifyOfPropertyChange(() => Message);
		}

		public void Close()
		{
			TryCloseAsync();
		}
	}
}
