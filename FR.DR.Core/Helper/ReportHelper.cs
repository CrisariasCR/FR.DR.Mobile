using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Softland.ERP.FR.Mobile.Cls;
using Softland.ERP.FR.Mobile.Cls.Reporte;

namespace Softland.ERP.FR.Mobile.Cls.Reporte
{
    internal static class ReportHelper
    {
        /// <summary>
        /// Folder donde se ponen los reportes
        /// </summary>
        private const string FolderReportes = "Reportes";
        
        /// <summary>
        /// Se crea la ruta completa del reporte, a partir del FRmConfig.FullAppPath
        /// y se asume que los reportes van a estar localizados en el subfolder Reportes
        /// y se concatena la extensi�n .rdl
        /// </summary>
        /// <param name="nombreReporte">nombre del archivo del reporte sin extensi�n</param>
        /// <returns></returns>
        internal static string CrearRutaReporte(Rdl nombreReporte)
        {
            if (FRmConfig.Tama�oPapel.Equals(2))
            {
                return System.IO.Path.Combine(FRmConfig.FullAppPath, ReportHelper.FolderReportes+"2", nombreReporte.ToString() + ".rdl");
            }
            if (FRmConfig.Tama�oPapel ==3)
            {
                return System.IO.Path.Combine(FRmConfig.FullAppPath, ReportHelper.FolderReportes+"3", nombreReporte.ToString() + ".rdl");
            }
            return System.IO.Path.Combine(FRmConfig.FullAppPath, ReportHelper.FolderReportes, nombreReporte.ToString() + ".rdl");
        }

        internal static string DevolverRutaReportes()
        {
            if (FRmConfig.Tama�oPapel.Equals(2))
            {
                return System.IO.Path.Combine(FRmConfig.FullAppPath, ReportHelper.FolderReportes+"2");
            }
            if (FRmConfig.Tama�oPapel ==3)
            {
                return System.IO.Path.Combine(FRmConfig.FullAppPath, ReportHelper.FolderReportes+"3");
            }
            return System.IO.Path.Combine(FRmConfig.FullAppPath, ReportHelper.FolderReportes);
 
        }
    }
}