using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityInvestment.Models.ViewModels
{
    public class PageList<T> where T : class
    {
        public int? TotalItemsCount { get; set; } = null!;
        public List<T> Items { get; private set; }
        public Pager Pager { get; private set; }
        public PageList() { }
        public PageList(int pageNo, int pageSize, List<T> ItemsList)
        {
            int itemsCounts = ItemsList.Count();
            if (pageNo < 1)
                pageNo = 1;
            int totalPages = Pager.getTotalPages(itemsCounts, pageSize);
            if (pageNo > totalPages)
                pageNo = totalPages;
            int recSkip = (pageNo - 1) * pageSize;
            var pager = new Pager(itemsCounts, pageNo, pageSize);

            // Setting Start and End item for current page
            pager.ItemFrom = recSkip + 1;
            if (pager.ItemFrom < 0)
                pager.ItemFrom = 0;
            if (pager.CurrentPage >= totalPages)
                pager.ItemTo = itemsCounts;
            else
                pager.ItemTo = recSkip + pageSize;

            // Items on Current page
            List<T> PageItems = ItemsList.Skip(recSkip).Take(pager.PageSize).ToList();

            Items = PageItems;
            Pager = pager;
            TotalItemsCount = itemsCounts;
        }

        public PageList(int pageNo, int pageSize, IQueryable<T> ItemsList)
        {
            int itemsCounts = ItemsList.Count();
            if (pageNo < 1)
                pageNo = 1;
            int totalPages = Pager.getTotalPages(itemsCounts, pageSize);
            if (pageNo > totalPages)
                pageNo = totalPages;
            int recSkip = (pageNo - 1) * pageSize;
            var pager = new Pager(itemsCounts, pageNo, pageSize);

            // Setting Start and End item for current page
            pager.ItemFrom = recSkip + 1;
            if (pager.ItemFrom < 0)
                pager.ItemFrom = 0;
            if (pager.CurrentPage >= totalPages)
                pager.ItemTo = itemsCounts;
            else
                pager.ItemTo = recSkip + pageSize;

            // Items on Current page
            List<T> PageItems = itemsCounts > 0 ? ItemsList.Skip(recSkip).Take(pager.PageSize).ToList() : ItemsList.ToList() ;

            Items = PageItems;
            Pager = pager;
            TotalItemsCount = itemsCounts;
        }
    }
}
