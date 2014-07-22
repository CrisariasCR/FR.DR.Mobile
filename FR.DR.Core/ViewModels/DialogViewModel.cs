using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Threading;
using Cirrious.MvvmCross.Commands;
using Softland.MvvmCross.TinyMessenger;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    /// codigo tomado de http://www.gregshackles.com/2012/11/returning-results-from-view-models-in-mvvmcross/

    /// <summary>
    /// clase para views que pueden ser invocadas como diálogos, es decir que pueden retornar un resultado
    /// o que simplemente se ocupa realizar alguna acción luego de que han finalizado su ejecución
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public abstract class DialogViewModel<TResult> : BaseViewModel
    {
        private string messageId { get; set; }

        protected DialogViewModel(string messageId) 
        {
            this.messageId = messageId; 
        } 
        
        protected void ReturnResult(TResult result) 
        {
            var message = new DialogResultMessage<TResult>(this, messageId, result);                                    
            MessengerHub.Publish(message);
            this.DoClose();            
        } 
    }

    /// <summary>
    /// clase que se utiliza para que el dialogo envíe el mensaje con el resultado al View que es Padre
    /// como se puede apreciar, el resultado es genérico, pues ser cualquier tipo que se indique
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class DialogResultMessage<TResult> : TinyMessageBase
    {
        public TResult Result { get; private set; }

        public string MessageId { get; set; }

        public DialogResultMessage(object sender, string messageId, TResult result)
            : base(sender)
        {
            Result = result; MessageId = messageId;
        }
    }

}