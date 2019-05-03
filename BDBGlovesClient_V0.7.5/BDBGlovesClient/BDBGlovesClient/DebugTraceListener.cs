using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BDBGlovesClient
{
    public class DebugTraceListener : TraceListener
    {
        private ListBox DebugListBox = null;

        public DebugTraceListener(ListBox list)
        {
            this.DebugListBox = list;
        }

        public override void WriteLine(string s)
        {
            if (DebugListBox != null) { DebugListBox.Items.Add(s); }
        }

        public override void Write(string s)
        {
            WriteLine(s);
        }
    }
}
