using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1sServerWidget
{
    internal static class ListNoAccessBase
    {
        internal static List<Model.InfoBase> List { get; set; } = new List<Model.InfoBase>();
        internal static List<string> ListName { get; set; } = new List<string>();
    }
}
