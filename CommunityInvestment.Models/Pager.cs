using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityInvestment.Models
{
    public class Pager
    {
        public int TotalItems { get; private set; }
        public int? ItemFrom { get; set; }
        public int? ItemTo { get; set; }
        public int CurrentPage { get; private set; }
        public int PageSize { get; private set; }
        public int TotalPages { get; private set; }
        public int StartPage { get; private set; }
        public int EndPage { get; private set;}
        public Pager() { }
        public Pager(int totalItems, int currentPage, int pageSize)
        {
            this.TotalItems = totalItems;
            CurrentPage = currentPage;
            PageSize = pageSize;
            int totalPages = getTotalPages(totalItems, pageSize);
            int _currentPage = currentPage;

            int startPage = _currentPage - 2;
            int endPage = _currentPage + 2;

            if(startPage <= 0)
            {
                endPage = endPage - (startPage - 1);
                startPage = 1;
            }

            if(endPage > totalPages)
            {
                endPage = totalPages;
                if(endPage > 5)
                {
                    startPage = endPage - 4;
                }
            }

            TotalItems = totalItems;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = totalPages;
            StartPage = startPage;
            EndPage = endPage;
        }

        public static int getTotalPages(int totalItems, int pageSize)
        {
            return (int)Math.Ceiling((decimal)totalItems / (decimal)pageSize);
        }
    }
}
