using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreBlog.Helpers
{
    public static class PageHelper
    {
        public static IEnumerable<int> PageNumbers(int pageNumber, int pageCount)
        {
            // range of 5
            // +2 from left border or -2 from right border

            if (pageCount <= 6)
            {
                for (int i = 1; i <= pageCount; i++)
                {
                    yield return i;
                }
            }
            else
            {
                int midPoint = pageNumber < 3 ? 3
                    : pageNumber > pageCount - 2 ? pageCount - 2
                    : pageNumber;

                int lowerBound = midPoint - 2;
                int upperBound = midPoint + 2;

                if (lowerBound != 1)
                {
                    yield return 1;
                    if (lowerBound - 1 > 1)
                    {
                        yield return -1;
                    }
                }

                for (int i = lowerBound; i <= upperBound; i++)
                {
                    yield return i;
                }

                if (upperBound != pageCount)
                {
                    if (pageCount - upperBound > 1)
                    {
                        yield return -1;
                    }
                    yield return pageCount;
                }
            }
        }
    }
}
