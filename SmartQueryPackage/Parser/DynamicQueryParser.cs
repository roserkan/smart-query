using SmartQueryPackage.Dynamic;
using SmartQueryPackage.Requests;

namespace SmartQueryPackage.Parser;

public static class DynamicQueryParser
{
    public static DynamicQuery ParseFromUrl(SmartRequest request)
    {
        var sort = ParseSort(request.Sort);
        var filter = ParseFilter(request.Filter);

        return new DynamicQuery(sort, filter);
    }

    private static IEnumerable<Sort>? ParseSort(string? sortQuery)
    {
        try
        {
            if (string.IsNullOrEmpty(sortQuery))
                return null;

            var sorts = sortQuery.Split(',');

            return sorts.Select(s =>
            {
                var parts = s.Split('_');
                return new Sort(parts[0], parts[1]);
            });
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static Filter? ParseFilter(string? filterQuery)
    {
        try
        {
            if (string.IsNullOrEmpty(filterQuery))
                return null;

            var filterParts = filterQuery.Split('-');
            var mainFilters = new List<Filter>();

            foreach (var part in filterParts)
            {
                var opFieldValues = part.Split('=');
                var opField = opFieldValues[0].Split('_');

                var values = opFieldValues[1].Split(',');
                if (values.Length == 1)
                {
                    mainFilters.Add(new Filter(opField[1], opField[0], values[0], null, null));
                }
                else
                {
                    var subFilters = new List<Filter>();
                    foreach (var value in values)
                    {
                        subFilters.Add(new Filter(opField[1], opField[0], value, null, null));
                    }
                    mainFilters.Add(new Filter { Logic = "or", Filters = subFilters });
                }
            }

            return new Filter { Logic = "and", Filters = mainFilters };
        }
        catch (Exception)
        {
            return null;
        }
    }

}

