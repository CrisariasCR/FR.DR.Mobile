using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMF.Printing
{

    public enum Port
    {
        ANYCOM_BT,
        BELKIN_BT,
        COMPAQ_BT,
        IPAQ_BT,
        SOCKETCOM_BT,
        WIDCOMM_BT,
        BTQUIKPRINT,
        INFRARED,
        COM1,
        COM2,
        COM3,
        COM4,
        COM5,
        COM6,
        COM7,
        COM8,
        COM9,
        LPT,

        /// <summary>
        /// Puerto de impresora mediante IP de red. Solo para PrinterCE
        /// </summary>
        /// <remarks>Solo para PrinterCE</remarks>
        NETIP,

        /// <summary>
        /// Puerto de impresora mediante ID de red. Solo para PrinterCE
        /// </summary>
        /// <remarks>Solo para PrinterCE</remarks>
        NETPATH,

        /// <summary>
        /// Puerto para imprimir a archivo
        /// </summary>
        TOFILE,

        /// <summary>
        /// Puerto de impresora mediante IP de red. Solo para AscciCE
        /// </summary>
        /// <remarks>Solo para AscciCE</remarks>
        PRINTERCE_SHARE,

        /// <summary>
        /// Selecciona el puerto por defecto. Solo para PrinterCE
        /// </summary>
        /// <remarks>Solo para PrinterCE</remarks>
        USE_CURRENT
    }

    public enum Printer
    {
        ABLE_AP1300,
        AXIOHM_A631,
        BROTHER,
        CANONBJ300,
        CANONBJ360,
        CANONBJ600,
        CITIZEN_203,
        CITIZEN_CMP10,
        CITIZEN_PD04,
        CITIZEN_PD22,
        CITIZEN_PN60,
        DYMOCOSTAR,
        ELTRADE,
        EPSON_ESCP2,
        EPSON_STYLUS,
        EPSON_TM_P60,
        EXTECH_2,
        EXTECH_3,
        EXTECH_4,
        FUJITSU_FTP628,
        GEBE_FLASH,
        GENERIC24_180,
        GENERIC24_203,
        GENERIC24_360,
        HP_PCL,
        INTERMEC,
        IPC_PP50,
        IPC_PP55,
        OMNIPRINT,
        ONEIL,
        PANASONIC_JTH200PR,
        PENTAX_200,
        PENTAX_300,
        PENTAX_II,
        PENTAX_RUGGEDJET,
        PERIPHERON_NOMAD,
        POCKET_SPECTRUM,
        S_PRINT,
        SATO,
        SEIKO_L465,
        SEIKO3445,
        SEIKOLABELWRITER,
        SIPIX,
        TALLY_MIP360,
        TALLY_MTP4,
        ZEBRA,
        USE_CURRENT
    }

    /// <summary>
    /// Summary description for IPrintable.
    /// </summary>
    public interface IPrintable
    {
        string GetObjectName();
        object GetField(string name);
    }

}

