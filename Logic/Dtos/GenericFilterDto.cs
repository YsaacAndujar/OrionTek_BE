using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Dtos
{
    public class GenericFilterDto
    {
        private int _page = 1;
        private int _entitiesPerPage = 10;
        public int Page
        {
            get { return _page; }
            set { _page = Math.Max(1, value); }
        }

        public int EntitiesPerPage
        {
            get { return _entitiesPerPage; }
            set { _entitiesPerPage = Math.Min(Math.Max(1, value), 200); }
        }
    }
}
