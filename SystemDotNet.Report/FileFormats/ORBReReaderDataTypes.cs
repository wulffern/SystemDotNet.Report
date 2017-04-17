using System;
using System.Collections.Generic;
using System.Text;

namespace SystemDotNet.PostProcessing
{
    public class ORBReReaderDataCsv : RegExReaderData
    {

        public ORBReReaderDataCsv()
            : base(';', new string[] { "#", "//" })
        {
            this.Name = "Comma Separated Values (;)";

        }
    }

    public class ORBReReaderDataCsv0 : RegExReaderData
    {

        public ORBReReaderDataCsv0(): base(',', new string[] { "#", "//" }) 
        {
            this.Name = "Comma Separated Values (,)";

        }
    }

    public class ORBReReaderDataEldo : RegExReaderData
    {

        public ORBReReaderDataEldo(): base(' ', new string[] { "#", "//" })  
        {
            this.Name = "Space Separated values";
        }
    }

    public class ORBReReaderDataTab : RegExReaderData
    {

        public ORBReReaderDataTab(): base('\t', new string[] { "#", "//" }) 
        {
            this.Name = "Tab Separated Values";
        }
    }


}
