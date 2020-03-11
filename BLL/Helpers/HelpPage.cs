using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Helpers
{
    public class HelpPage
    {
        public string Header { get; set; }
        public string Guildline { get; set; }
        public List<string> ContentList { get; set; }
    }

    public class IndexPage
    {
        public string Message { get; set; }
    }
}
