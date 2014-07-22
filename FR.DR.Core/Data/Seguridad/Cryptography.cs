using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace Softland.ERP.FR.Mobile.Cls.Seguridad
{
	/// <summary>
	/// Summary description for Cryptography.
	/// </summary>
	public class Cryptography
	{
		static int KeySize = 34;
		static bool bSeeded = false;
		static uint[] aucValueMap = new uint []  { 0x0e, 0x08, 0x0a, 0x01, 0x07, 0x06, 0x0f, 0x02, 
													 0x00, 0x09, 0x0b, 0x0c, 0x03, 0x0d, 0x05, 0x04 };
		static uint [] aucReverseValueMap = new uint[]{ 0x08, 0x03, 0x07, 0x0c, 0x0f, 0x0e, 0x05, 0x04, 
														  0x01, 0x09, 0x02, 0x0a, 0x0b, 0x0d, 0x00, 0x06 };
		static System.Random rand;

		public Cryptography()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public string EncryptString(string Data, string Key)
		{
			byte[] buffer = new byte[KeySize];
			string HexRepresentation="";

			EncryptStringRandomKey(Data,buffer,KeySize);

            for (int index = 0; index < KeySize; index++)
            {
                HexRepresentation=HexRepresentation+Uri.HexEscape((char)buffer[index]).Replace("%",""); 
                // UNDONE LINEA COMENTADA NO FUNCIONA
            }
			return HexRepresentation;
		}

		public string DecryptString(string Data, string Key)
		{
            byte[] buffer = new byte[KeySize];
            int HexPos = 0;
            string HexSymbol;

            for (int index = 0; index < (KeySize * 2); index += 2)
            {
                HexSymbol = "%" + Data.Substring(index, 2);
                HexPos = 0;
                buffer[index / 2] = (byte)Uri.HexUnescape(HexSymbol, ref HexPos);
            }

            return DecryptStringRandomKey(buffer, KeySize);			
		}

		//---------------------------------------------------------------------------
		// SeedRandom:  seed the random number generator (once per session)
		//---------------------------------------------------------------------------
		static void SeedRandom()
		{

			if ( !bSeeded )
			{
				rand = new  System.Random((int)System.DateTime.Now.Ticks);
				bSeeded = true;
			}
		}

		//---------------------------------------------------------------------------
		// MapbyteValue:
		//---------------------------------------------------------------------------
		static uint MapbyteValue( uint ucValue )
		{

			return (aucValueMap[ (ucValue & 0x0f) ]) |
				(aucValueMap[ (ucValue >> 4) ] << 4);
		}

		//---------------------------------------------------------------------------
		// UnmapbyteValue:
		//---------------------------------------------------------------------------
		static uint UnmapbyteValue( uint ucValue )
		{


			return (uint)aucReverseValueMap[ (ucValue & 0x0f) ] |
				(aucReverseValueMap[ (ucValue >> 4) ] << 4);
		}

		//---------------------------------------------------------------------------
		// RotateRight:
		//---------------------------------------------------------------------------
		static byte RotateRight( uint ucValue, int iBits )
		{
			iBits = iBits % 8;
			return (byte)((ucValue >> iBits) | (ucValue << (8 - iBits)));
		}

		//---------------------------------------------------------------------------
		// RotateLeft:
		//---------------------------------------------------------------------------
		static byte RotateLeft( uint ucValue, int iBits )
		{
			iBits = iBits % 8;
			return  (byte)((ucValue << iBits) | (ucValue >> (8 - iBits)));
		}

		//---------------------------------------------------------------------------
		// EncryptString:
		//---------------------------------------------------------------------------
		private bool EncryptString( uint[] pszSrc, uint[] pucDest, int iDestLen, uint wKey )
		{
			bool     bOK = true;
			int      ibyteLen = pszSrc.Length;

			if ( ibyteLen > (iDestLen + 2) )       // space 2 uint length
				bOK = false;                        // buffer too small
			else
			{
				// initialize buffer with random data
				SeedRandom();

				for ( int iPos = 0; iPos < iDestLen; iPos += 2 )
				{
					int iRandom = rand.Next(255);
					pucDest[ iPos ] = (uint)iRandom;
		 
					//memcpy( &(pucDest[ iPos ]), &iRandom, min( 2, iDestLen - iPos ) );
				}

				EncryptData( pszSrc, pucDest, ibyteLen, wKey );

				uint wbyteLen = ((uint)ibyteLen) ^ wKey;
				pucDest[ iDestLen - 1 ] = (uint)(wbyteLen & 0xff);
				pucDest[ iDestLen - 2 ] = (uint)((wbyteLen) >> 8);
			}

			return bOK;
		}

		//---------------------------------------------------------------------------
		// DecryptString:
		//---------------------------------------------------------------------------
		// @eql@2308 CString DecryptString( uint[] pucSrc, int iSrcLen, UInt16 wKey )
		private uint[] DecryptString( uint[] pucSrc, int iSrcLen,uint  wKey )
		{
      
			// @eql@2308
			// CString  strResult;   
			//uint[]   pucResult;
			uint[]	lpResult = null;   
			int      ibyteLen;

			uint wbyteLen = pucSrc[ iSrcLen - 1 ];
			wbyteLen |= pucSrc[ iSrcLen - 2 ] << 8;
			ibyteLen = (int)(wbyteLen ^ wKey);


			// @eql@2308 pucResult = (uint[])strResult.GetBufferSetLength( (ibyteLen + 1) * sizeof( _TCHAR ) );
			lpResult = new uint[ ibyteLen ];

			// @eql@2308 DecryptData( pucSrc, pucResult, ibyteLen, wKey );
			DecryptData( pucSrc, lpResult, ibyteLen, wKey );
   
			// @eql@2308 pucResult[ ibyteLen ] = _TCHAR( 0 );
			//lpResult[ ibyteLen ] = 0x0;

			// @eql@2308 strResult.ReleaseBuffer();

			return lpResult;
		}

		//---------------------------------------------------------------------------
		// EncryptStringRandomKey:
		//---------------------------------------------------------------------------

		private bool EncryptStringRandomKey( string pszSrc, byte[] pucDest, int iDestLen )
		{
			bool result;
	
			uint[] uintpucDest = new uint[pucDest.Length];

			byte[] tmpbytes = System.Text.ASCIIEncoding.UTF8.GetBytes(pszSrc);
			uint[] uintpszSrc = new uint[tmpbytes.Length];

			for (int index=0;index<tmpbytes.Length;index++)
				uintpszSrc[index]=(uint)tmpbytes[index];
	
			result=EncryptStringRandomKey(uintpszSrc,uintpucDest,iDestLen);

			for (int index=0;index<pucDest.Length;index++)
				pucDest[index]=(byte)uintpucDest[index];

			return result;
		}

		private bool EncryptStringRandomKey( uint[] pszSrc, uint[] pucDest, int iDestLen )
		{
			bool  bOK = true;
			int   ibyteLen = pszSrc.Length;

			if ( ibyteLen > (iDestLen + 4) )       // space for key + 2 uint length
				bOK = false;                        // buffer too small
			else
			{
				uint  wKey;

				SeedRandom();
				wKey = (uint)rand.Next(255);
				bOK = EncryptString( pszSrc, pucDest, iDestLen - 2, wKey );
				wKey ^= 0xfebe;
				pucDest[ iDestLen - 1 ] = (uint)(wKey >> 8);
				pucDest[ iDestLen - 2 ] = (uint)(wKey & 0xff);
			}

			return bOK;
		}

		//---------------------------------------------------------------------------
		// DecryptStringRandomKey:
		//---------------------------------------------------------------------------
		// @eql@2308 CString DecryptStringRandomKey( uint[] pucSrc, int iSrcLen )

		private string DecryptStringRandomKey( byte[] pucSrc, int iSrcLen )
		{
			uint[] uintpszSrc = new uint[pucSrc.Length];
			uint[] uintresult;
			byte[] byteresult;

			for (int index=0;index<pucSrc.Length;index++)
				uintpszSrc[index]=(uint)pucSrc[index];

			uintresult=DecryptStringRandomKey(uintpszSrc,iSrcLen);

			byteresult = new byte[uintresult.Length];

			for (int index=0;index<uintresult.Length;index++)
				byteresult[index]=(byte)uintresult[index];

			return System.Text.ASCIIEncoding.UTF8.GetString(byteresult,0,byteresult.Length);
	
		}

		private uint[] DecryptStringRandomKey( uint[] pucSrc, int iSrcLen )
		{
			uint  wKey;

			wKey = (uint)pucSrc[ iSrcLen - 1 ] << 8;
			wKey |= (uint)pucSrc[ iSrcLen - 2 ] & 0xff;
			wKey ^=0xfebe;

			return DecryptString( pucSrc, iSrcLen - 2, wKey );
		}

		//---------------------------------------------------------------------------
		// EncryptData:
		//---------------------------------------------------------------------------
		private void EncryptData( 
			uint[]  pSrcData, 
			uint[]   pDestData, 
			int      iLength,
			uint     wKey )
		{
			uint[] pucSrc = (uint[])pSrcData;
			uint[] pucDest = (uint[])pDestData;
			for ( int ibyte = 0; ibyte < iLength; ibyte++ )
			{
				//uint ucKey = (uint)((ibyte & 1) ? wKey >> 8 : wKey & 0xff);
				uint ucKey = (uint)((ibyte & 1)==1 ? wKey >> 8 : wKey & 0xff);

				pucDest[ ibyte ] =
					ucKey ^ RotateRight( MapbyteValue( pucSrc[ ibyte ] ), ibyte );
			}
		}

		//---------------------------------------------------------------------------
		// DecryptData:
		//---------------------------------------------------------------------------
		private void DecryptData( 
			uint[]  pSrcData, 
			uint[]   pDestData, 
			int      iLength,
			uint     wKey )
		{
			uint[] pucSrc = (uint[])pSrcData;
			uint[] pucDest = (uint[])pDestData;
			for ( int ibyte = 0; ibyte < iLength; ibyte++ )  
			{
				//uint ucKey = (uint)((ibyte & 1) ? wKey >> 8 : wKey & 0xff);
				uint ucKey = (uint)((ibyte & 1)==1 ? wKey >> 8 : wKey & 0xff);

				pucDest[ ibyte ] =
					UnmapbyteValue( RotateLeft( ucKey ^ pucSrc[ ibyte ], ibyte ) );
			}
		}
	}
}
