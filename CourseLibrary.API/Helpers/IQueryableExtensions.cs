﻿using CourseLibrary.API.Services;
using System.Linq.Dynamic.Core;

namespace CourseLibrary.API.Helpers;

public static class IQueryableExtensions
{
    public static IQueryable<T> ApplySort<T>(
        this IQueryable<T> source,
        string orderBy,
        Dictionary<string, PropertyMappingValue> mappingDictionary)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (mappingDictionary is null)
        {
            throw new ArgumentNullException(nameof(mappingDictionary));
        }

        if (string.IsNullOrEmpty(orderBy))
        {
            return source;
        }

        var orderByString = string.Empty;

        // the orderBy string is separated by ",", so we split it.
        var orderByAfterSplit = orderBy.Split(',');

        // apply each orderBy clause
        foreach (var orderByClause in orderByAfterSplit)
        {
            // trim the orderBy clause, as it might contain leading
            // or trailing spaces. Can not trim the var in foreach
            // so use another var.
            var trimmedOrderByClause = orderByClause.Trim();

            // if the sort options ends with "desc", we order
            // descending. otherwise ascending
            var orderDescending = trimmedOrderByClause.EndsWith(" desc");

            // remove " asc" or " desc" from the orderBy clause, so we
            // get the property name to look in the mapping dictionary
            var indexOfFirstSpace = trimmedOrderByClause.IndexOf(" ");
            var propertyName = indexOfFirstSpace == -1 ?
                trimmedOrderByClause : trimmedOrderByClause
                    .Remove(indexOfFirstSpace);

            // find the matching property
            if (mappingDictionary.ContainsKey(propertyName) == false)
            {
                throw new ArgumentException($"Key mapping for {propertyName} is missing.");
            }

            // get the PropertyMappingValue
            var propertyMappingValue = mappingDictionary[propertyName];
            if (propertyMappingValue is null)
            {
                throw new ArgumentException(nameof(propertyMappingValue));
            }

            // revert sort order if necessary
            if (propertyMappingValue.Revert)
            {
                orderDescending = !orderDescending;
            }

            // Run through the property names
            foreach (var destinationProperty in
                propertyMappingValue.DestinationProperties)
            {
                orderByString = orderByString +
                    (string.IsNullOrEmpty(orderByString) ? string.Empty : ", ")
                    + destinationProperty
                    + (orderDescending ? " descending" : " ascending");
            }
        }

        return source.OrderBy(orderByString);
    }
}
