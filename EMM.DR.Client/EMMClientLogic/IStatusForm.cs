using System;
//using System.Windows.Forms;

namespace EMMClient.EMMClientLogic
{

	/// <summary>
	///Enumeracion utilizada para representar las
	///fases de la sincronizacion
	/// </summary>
	public enum ImageStatus
	{
		SyncClient,
		SyncServer,
		SyncError,
		ClientError,
		ServerError,
		Complete,
		AuthenticationError,
		ConnectionError,
		Connecting
	}

	public enum ConduitState
	{
		Executing, //Se esta ejecutando el conducto
		Finished,  //La ejecucion termino correctamente
		Aborted,   //La ejecucion se aborto por algun error
		Canceled   //El usuario cancelo la ejecucion
	}

	/// <summary>
	/// Summary description for IStatusForm.
	/// </summary>
	public interface IStatusForm
	{
		void UpdateInterface(ImageStatus currentStatus,string title, string message,int progress);
		void ShowErrorClient(string message);
		void ShowErrorServer(string message);
		void ShowErrorConnection(string message);
		void ShowWindow();
		void CloseWindow();
		void FocusWindow();
		//DialogResult ShowDialog(string message);
		bool Canceled();
	}
}
