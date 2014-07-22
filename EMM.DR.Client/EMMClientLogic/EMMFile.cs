using System;

using System.Collections.Generic;
using System.Text;

namespace EMMClient.EMMClientLogic
{
    public class EMMFile
    {

        #region Constructores
        /// <summary>
        /// Contructor con parametros.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="version"></param>
        /// <param name="date"></param>
        public EMMFile(string id,string version,string type, string name, string versionDetail, string typeDetail, string date)
        {
            this.ID = id;
            this.Version = version;
            this.Type = type;
            this.Name = name;
            this.VersionDetail = versionDetail;
            this.TypeDetail = typeDetail;
            this.Date = date;
        }
        /// <summary>
        /// Inicializa una instancia a partir de una cadena de caracteres con la forma (ID;Nombre;Version;Fecha)
        /// </summary>
        /// <param name="datos">(archivo;version;tipo;nombre;versionDetalle;tipoDetalle;fecha)</param>
        public EMMFile(string datos)
        {
            string[] split = datos.Split(';');

            if (split.Length == 7)
            {
                this.ID = split[0];
                this.Version = split[1];
                this.Type = split[2];
                this.Name = split[3];
                this.VersionDetail = split[4];
                this.TypeDetail = split[5];
                this.Date = split[6];
            }
            else
            {
                this.ID = datos;
            }
        }

        #endregion

        #region Atributos
        private string name;
        private string id;
        private string version;
        private string date;

        private string type;
        private string versionDetail;
        private string typeDetail;
        #endregion

        #region Propiedades
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string ID
        {
            get { return id; }
            set { id = value; }
        }
        public string Version
        {
            get { return version; }
            set { version = value; }
        }
        public string Date
        {
            get { return date; }
            set { date = value; }
        }
        public string Type
        {
            get { return type; }
            set { type = value; }
        }
        public string VersionDetail
        {
            get { return versionDetail; }
            set { versionDetail = value; }
        }
        public string TypeDetail
        {
            get { return typeDetail; }
            set { typeDetail = value; }
        }
        private bool selected;
        /// <summary>
        /// Indica el estado del archivo.
        /// </summary>
        public bool Selected
        {
            get { return selected; }
            set { selected = value; }
        }

        public string Seleccionado
        {
            get
            {
                if (Selected)
                    return "Si";
                else
                    return "No";
            }
        }
        #endregion
        
    }
}
