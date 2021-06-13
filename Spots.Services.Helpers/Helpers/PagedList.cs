using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots.Services.Helpers
{
    public class PagedList<T> : List<T>
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public bool HasPrevious => (CurrentPage > 1);
        public bool HasNext => (CurrentPage < TotalPages);


        public PagedList()
        {

        }
        public PagedList(List<T> collection) : base(collection)
        {

        }
        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
            :base(items)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }


        public static PagedList<T> Create(IQueryable<T> source, int pageNmber, int pageSize, bool includeAll)
        {
            var count = source.Count();
            if (includeAll)
            {
                return new PagedList<T>(source.ToList(), count, 1, count+1);
            }
            var items = source.Skip((pageNmber - 1) * pageSize).Take(pageSize).ToList();
            return new PagedList<T>(items, count, pageNmber, pageSize);
        }
    }
}
