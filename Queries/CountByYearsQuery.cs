﻿using FmpAnalyzer.Data;
using FmpDataContext;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FmpAnalyzer.Queries
{
    /// <summary>
    /// Counts synbols in database given years and dates list.
    /// </summary>
    public class CountByYearsQuery : QueryBase
    {
        public CountByYearsQuery(DataContext dataContext) : base(dataContext) { }

        /// <summary>
        /// Run
        /// </summary>
        /// <param name="yearFrom"></param>
        /// <param name="yearTo"></param>
        /// <param name="dates"></param>
        /// <returns></returns>
        public int Run(int yearFrom, int yearTo, List<string> dates)
        {
            var datesList = BuildDatesList(yearFrom, yearTo, dates);

            int result = (from income in DataContext.IncomeStatements
                          where datesList.Contains(income.Date)
                          select income.Symbol).Distinct().Count();
            return result;
        }

        /// <summary>
        /// BuildDatesList
        /// </summary>
        /// <param name="yearFrom"></param>
        /// <param name="yearTo"></param>
        /// <param name="dates"></param>
        /// <returns></returns>
        private List<string> BuildDatesList(int yearFrom, int yearTo, List<string> dates)
        {
            List<string> resultList = new List<string>();

            for (int year = yearFrom; year <= yearTo; year++)
            {
                foreach (var date in dates)
                {
                    resultList.Add(year.ToString() + date[4..]);
                }
            }

            return resultList;
        }
    }
}