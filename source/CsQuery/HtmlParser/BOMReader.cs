using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CsQuery.Implementation;

namespace CsQuery.HtmlParser
{
    /// <summary>
    /// A class to parse and expose information about the byte order marks (BOM) for a stream.
    /// </summary>

    public class BOMReader
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        ///
        /// <param name="stream">
        /// The stream to analyze.
        /// </param>

        public BOMReader(Stream stream)
        {
            DefaultEncoding = Encoding.UTF8;
            InputStream = stream;
            Parse();
        }

        private Stream InputStream;
        private byte[] Header = new byte[5];
        private int BytesRead;
        private int BomLength;
        
        public Encoding Encoding { get; protected set; }

        /// <summary>
        /// The input stream stripped of the BOM
        /// </summary>

        public Stream StreamWithoutBOM 
        { 
            get {
                if (!IsBOM)
                {
                    return StreamWithBOM;
                }

                Stream stream;
                if (InputStream.CanSeek)
                {
                    InputStream.Position = BomLength;
                    stream = InputStream;
                }
                else if (BytesRead == 5 && InputStream.CanRead)
                {
                    var headerStream = new MemoryStream(Header, BomLength, 5 - BomLength);
                    stream = new CombinedStream(headerStream, InputStream);
                }
                else
                {
                    stream = new MemoryStream(Header, BomLength, 5 - BytesRead);
                }
                return stream;
            }

        }

        /// <summary>
        /// The original stream
        /// </summary>

        public Stream StreamWithBOM
        {
            get
            {
                Stream stream;
                if (InputStream.CanSeek)
                {
                    InputStream.Position = 0;
                    stream = InputStream;
                }
                else if (BytesRead == 5 && InputStream.CanRead)
                {
                    var headerStream = new MemoryStream(Header);
                    stream = new CombinedStream(headerStream, InputStream);
                }
                else
                {
                    stream = new MemoryStream(Header, 0, BytesRead);
                }
                return stream;
            }
        }

        /// <summary>
        /// Gets or sets the default encoding for the stream (if no BOM detected)
        /// </summary>

        public Encoding DefaultEncoding { get; protected set; }

        /// <summary>
        /// When true, indicates a valid BOM was detected
        /// </summary>

        public bool IsBOM { get; protected set; }

        /// <summary>
        /// The document had no BOM, but was an XML document.
        /// </summary>

        public bool IsXML { get; protected set; }

        /// <summary>
        /// Parses the input stream to obtain an encoding
        /// </summary>

        protected void Parse()
        {

            int BytesRead = InputStream.Read(Header, 0, 5);           
            Encoding = GetFileEncoding();
        }

        private Encoding GetFileEncoding()
        {
            
            Encoding enc = Encoding.Default;


            if (Matches(new byte[] { 0xef, 0xbb, 0xbf }))
            {
                enc = Encoding.UTF8;
                BomLength=3;
                IsBOM = true;
            }
            else if (Matches(new byte[] { 0x00, 0x00, 0xfe, 0xff }))
            {
                enc = new UTF32Encoding(true, true);
                BomLength=4;
                IsBOM = true;
            }
            else if (Matches(new byte[] { 0xff, 0xfe, 0x00, 0x00 }))
            {
                enc = Encoding.UTF32;
                BomLength=4;
                IsBOM = true;
            }
            else if (Matches(new byte[] {  0x2b, 0x2f, 0x76 })) {
                enc = Encoding.UTF7;
                BomLength=3;
                IsBOM = true;
            }
            else if (Matches(new byte[] { 0xfe, 0xff }))
            {
                enc = Encoding.BigEndianUnicode;
                BomLength=2;
                IsBOM = true;
            }
            else if (Matches(new byte[] { 0xfe, 0xff }))
            {
                enc = Encoding.Unicode;
                BomLength=2;
                IsBOM = true;
            }
                
            else if (Matches(new byte[] { 0x3c, 0x3f, 0x78, 0x6d, 0x6c }))
            {
                enc = Encoding.Default;
                BomLength=5;
                IsXML = true;
            }

            return enc;
        }

        /// <summary>
        /// Test if the header matches the bytes passed (up to the length of the array passed)
        /// </summary>
        ///
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        ///
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>

        private bool Matches(byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                if (Header[i] != buffer[i])
                {

                    return false;
                }

            }
            return true;
        }
    }
}
