using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1sServerWidget
{
    internal static class ListNoAccessBase
    {
        private static List<Model.InfoBase> _list = new List<Model.InfoBase>();
        private static List<string> _listName = new List<string>();

        internal static List<Model.InfoBase> List { get => _list; set => _list = value; }
        internal static List<string> ListName { get => _listName; set => _listName = value; }
    }
}
