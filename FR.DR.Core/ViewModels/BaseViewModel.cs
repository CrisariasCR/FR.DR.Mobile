using System;
using System.Net;
using System.Collections.Generic;
//using System.Windows.Controls;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido;
using Cirrious.MvvmCross.Commands;
using Cirrious.MvvmCross.ExtensionMethods;
using Cirrious.MvvmCross.Interfaces.ServiceProvider;
using Cirrious.MvvmCross.ViewModels;
using FR.Core.Model;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using System.Windows.Input;
using Softland.MvvmCross.MessageService;
using Softland.MvvmCross.TinyMessenger;
using System.Windows.Forms;
using FR.DR.Core.Helper;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class BaseViewModel : MvxViewModel, IMvxServiceConsumer
    {
        /// <summary>
        /// Ejecuta una acción, y si falla muestra un mensaje
        /// </summary>
        /// <param name="action"></param>
        /// <param name="mensaje"></param>
        public void TryMsj(Action action, string mensaje)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                mostrarAlerta(mensaje + " " + ex.Message);
            }
        }

        public void ShowMessage(string title, string message, MessageBoxButtons buttons, Action<DialogResult> callback)
        {
            this.GetService<IMessageReporter>().ShowMessage(title, message, buttons, MessageBoxIcon.None, callback);
        }

        public void ShowMessage(string title, string message, MessageBoxButtons buttons, MessageBoxIcon icon, Action<DialogResult> callback)
        {
            this.GetService<IMessageReporter>().ShowMessage(title, message, buttons, icon, callback);
        }

#region mostrar*
        public void mostrarMensaje(Mensaje.Accion accion, string procesoP, Action<DialogResult> callback)
        {
            Mensaje.mostrarMensaje(this, accion, procesoP, callback);
        }

        public void mostrarMensaje(Mensaje.Accion accion, string procesoP)
        {
            Mensaje.mostrarMensaje(this, accion, procesoP, null);
        }

        public void mostrarMensaje(Mensaje.Accion accion)
        {
            Mensaje.mostrarMensaje(this, accion, null);
        }

        public void mostrarAlerta(string alerta, Action<DialogResult> callback)
        {
            Mensaje.mostrarAlerta(this, alerta, callback);
        }
        public void mostrarAlerta(string alerta)
        {
            Mensaje.mostrarAlerta(this, alerta, null);
        }
#endregion mostrar*

        protected BaseViewModel()    
        {        
            MessengerHub = this.GetService<ITinyMessengerHub>();    
        }     
        
        protected ITinyMessengerHub MessengerHub { get; private set; }

        /// <summary>
        /// permite invocar o navegar hacia un View en modalidad de diálogo
        /// </summary>
        /// <typeparam name="TViewModel">El tipo del View que se desea mostrar</typeparam>
        /// <typeparam name="TResult">El tipo del resultado que va a retornar</typeparam>
        /// <param name="parameterValues">Los parámetros que recibe el view</param>
        /// <param name="onResult"></param>
        /// <returns></returns>
        protected bool RequestDialogNavigate<TViewModel, TResult>(IDictionary<string, object> parameterValues,
            Action<TResult> onResult) where TViewModel : DialogViewModel<TResult> 
        {
            parameterValues = parameterValues ?? new Dictionary<string, object>(); 

            // verifica que no venga un parámetro con dicho nombre, pues el mismo se va a utilizar
            // para pasar el id del mensaje con que se van a comunicar los views (parentView/childDialogView)
            if (parameterValues.ContainsKey("messageId"))
            {
                throw new ArgumentException("parameterValues cannot contain an item with the key 'messageId'");
            }
            
            string messageId = Guid.NewGuid().ToString();  // genera el id del mensaje
            
            parameterValues["messageId"] = messageId; // asigna el id del mensaje en los parametros
            
            TinyMessageSubscriptionToken token = null; 
            
            // realiza la suscripción del mensaje, indicando el tipo de mensaje<resultado> y la accion
            // a ejecutar cuando el mensaje sea recibido,
            // y que sólo se active cuando el mensaje tenga el id correspondiente
            token = MessengerHub.Subscribe<DialogResultMessage<TResult>>(
                msg => 
                    {
                        if (token != null) 
                        {
                            // se desuscribe cuando ya llegó el mensaje
                            MessengerHub.Unsubscribe<DialogResultMessage<TResult>>(token);
                            // y ejecuta la acción correspondiente a la recepción del mensaje
                            onResult(msg.Result);
                        }
                    }, 
                msg => msg.MessageId == messageId); 

            // navega hacia el View indicado
            return RequestNavigate<TViewModel>(parameterValues); 
        }

    }

    public class ListViewModel : BaseViewModel
    {
        public IObservableCollection<string> Header { get { return new SimpleObservableCollection<string>() { "Header" }; } }
    }

    public class ListaArticulosViewModel : ListViewModel
    {
        protected CriterioArticulo criterioActual;
        public virtual CriterioArticulo CriterioActual
        {
            get { return criterioActual; }
            set
            {
                if (value != criterioActual)
                {
                    criterioActual = value;
                    RaisePropertyChanged("CriterioActual");
                }
            }
        }

        public IObservableCollection<CriterioArticulo> Criterios { get; set; }

        public ListaArticulosViewModel()
        {
            Criterios = new SimpleObservableCollection<CriterioArticulo>()
                    { CriterioArticulo.Codigo,
                      CriterioArticulo.Barras,
                      CriterioArticulo.Descripcion,
                      CriterioArticulo.Familia,
                      CriterioArticulo.Clase
                    };
            CriterioActual = Pedidos.CriterioBusquedaDefaultBD;
            TextoBusqueda = "";
        }

        protected string textoBusqueda = string.Empty;
        public virtual string TextoBusqueda
        {
            get { return textoBusqueda; }
            set { textoBusqueda = value; RaisePropertyChanged("TextoBusqueda"); }
        }

        public virtual ICommand ComandoRefrescar
        {
            get { return new MvxRelayCommand(Refrescar); }
        }

        public virtual void Refrescar() { }
    }
}
