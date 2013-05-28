
namespace Ester.Model.Interfaces
{
	public delegate void ViewModelConfiguredEventHandler(IEsterViewModel sender);

	public interface IEsterViewModel
	{
		event ViewModelConfiguredEventHandler ViewModelConfiguredEvent;
		bool IsReady { get; }
		string Title { get; }
		void Configure();
	}
}
