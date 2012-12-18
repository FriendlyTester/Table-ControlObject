using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using System.Collections.ObjectModel;

namespace ControlObjects
{
    public class Table
    {
        protected IWebElement _tableBody; //Protected reference to the table body.
        protected List<IWebElement> _tableHeaders; //List of all the tableHeaders used to get the column indexs.
        protected List<IWebElement> _tableRows; //List of all the rows to loop through to look for matches.

        public Table(IWebElement table)
        {
            try
            {
                //Look for and assign the tbody element
                _tableBody = table.FindElement(By.TagName("tbody"));
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Couldn't find a tbody tag for this table. {0}", ex.Message));
            }

            try
            {
                //Look for all the table headers
                _tableHeaders = table.FindElements(By.TagName("th")).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Couldn't find any th tags within this table. {0}", ex.Message));
            }

            try
            {
                //Look for all the table rows
                _tableRows = _tableBody.FindElements(By.TagName("tr")).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("This table doesn't contain any rows. {0}", ex.Message));
            }

        }

        protected int FindColumnIndex(string columnName)
        {
            IWebElement desiredColumn = null;

            try { desiredColumn = _tableHeaders.First(d => d.Text.Trim() == columnName); }
            catch (Exception ex)
            {
                if (ex is ArgumentNullException)
                {
                    throw new Exception(string.Format("Unable to find {0} in the list of columns found", columnName));
                }
                if (ex is InvalidOperationException)
                {
                    throw new Exception(string.Format("More than one column was found for {0}", columnName));
                }
                else
                {
                    throw;
                }
            }
            //We have to add one as list if zero indexed, however we would say column 1 not 0, and also XPath isn't 0 based.
            return _tableHeaders.IndexOf(desiredColumn) + 1;
        }


        /// <summary>
        /// Find the first row that contains the specified value in the specified column
        /// </summary>
        /// <param name="columnName">The column to look for the value in</param>
        /// <param name="knownValue">The value to look for</param>
        /// <returns>The matching row element</returns>
        public IWebElement FindRowMatchingColumnData(string columnName, string knownValue)
        {
            IWebElement requiredRow = null;
            int columnIndex = FindColumnIndex(columnName);

            try { requiredRow = _tableRows.First(d => d.FindElement(By.XPath(string.Format("td[{0}]", columnIndex))).Text == knownValue); }
            catch (Exception ex)
            {
                if (ex is NoSuchElementException)
                {
                    throw new Exception(string.Format("Column index {0} doesn't exist", columnIndex));
                }
                if (ex is ArgumentNullException)
                {
                    throw new Exception(string.Format("Row containing {0} in column index {1} was not found", knownValue, columnIndex));
                }
                else
                {
                    throw;
                }
            }

            if (requiredRow != null)
            {
                return requiredRow;
            }

            throw new Exception("Required row is null, unknown error occured");
        }

        /// <summary>
        /// Find a row that comtains the given value in any column
        /// </summary>
        /// <param name="knownValue">The value to look for</param>
        /// <returns>The matching row element</returns>
        public IWebElement FindFirstRowByKnownValue(string knownValue)
        {
            int i;
            i = 1;

            while (i <= _tableHeaders.Count())
            {
                foreach (IWebElement row in _tableRows)
                {
                    if (row.FindElement(By.XPath(string.Format("td[{0}]", i))).Text == knownValue)
                    {
                        return row;
                    }
                }

                i++;
            }

            throw new Exception(string.Format("Unable to find a row containing: {0} in a column", knownValue));
        }

        /// <summary>
        /// Check to see if a value is within a specified column
        /// </summary>
        /// <param name="columnName">The column name to check in</param>
        /// <param name="knownValue">The value to look for</param>
        /// <returns>True is the value is found</returns>
        public bool IsValuePresentWithinColumn(string columnName, string knownValue)
        {
            try
            {
                IWebElement matchedColumn = _tableRows.First(d => d.FindElement(By.XPath(string.Format("td[{0}]", FindColumnIndex(columnName)))).Text == knownValue);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Find a cell by the column name and known value
        /// </summary>
        /// <param name="columnName">Column to look for the known value in</param>
        /// <param name="knownValue">The known value to look for</param>
        /// <returns>Matching cell element</returns>
        public IWebElement FindCellByColumnAndKnownValue(string columnName, string knownValue)
        {
            IWebElement matchedRow = null;

            try
            {
                matchedRow = _tableRows.First(d => d.FindElement(By.XPath(string.Format("td[{0}]", FindColumnIndex(columnName)))).Text == knownValue);
            }
            catch (Exception)
            {
                throw new Exception(string.Format("Unable to find a cell in column: {0} containing {1}", columnName, knownValue));
            }

            return matchedRow.FindElement(By.XPath(string.Format("td[{0}]", FindColumnIndex(columnName))));
        }

        /// <summary>
        /// Find a cell my using the row element and the column name
        /// </summary>
        /// <param name="row">The row element to read column value from</param>
        /// <param name="columnName">The column name to read value from</param>
        /// <returns>Matching cell element</returns>
        public IWebElement FindCellByRowAndColumnName(IWebElement row, string columnName)
        {
            IWebElement cell;

            try
            {
                cell = row.FindElement(By.XPath(string.Format("td[{0}]", FindColumnIndex(columnName))));
            }
            catch (Exception)
            {
                throw new Exception("Unable to find a cell using given row and columnName");
            }

            return cell;
        }

        /// <summary>
        /// Returns a list of all the column headers
        /// </summary>
        /// <returns>A list of all the column names as strings</returns>
        public List<string> ReadAllColumnHeaders()
        {
            List<string> columnNames = new List<string>();

            foreach (IWebElement element in _tableHeaders)
            {
                columnNames.Add(element.Text);
            }

            return columnNames;
        }

        /// <summary>
        /// Return the number of columns from the table
        /// </summary>
        /// <returns>Number of columns as an int</returns>
        public int ColumnCount()
        {
            return _tableHeaders.Count();
        }

        /// <summary>
        /// Returns the number of rows in the table
        /// </summary>
        /// <returns>Number of rows as an int</returns>
        public int RowCount()
        {
            return _tableRows.Count();
        }

        /// <summary>
        /// Find a cell by column name and the row number
        /// </summary>
        /// <param name="columnName">The name of the column to read from</param>
        /// <param name="row">The number of the row</param>
        /// <returns>Matching cell element</returns>
        public IWebElement FindCellByColumnAndRowNumber(string columnName, int row)
        {
            IWebElement matchingCell = _tableBody.FindElement(By.XPath(string.Format("tr[{0}]/td[{1}]", row, FindColumnIndex(columnName))));

            return matchingCell;
        }

        /// <summary>
        /// Read all the data from a specific column
        /// </summary>
        /// <param name="columnName">The name of the column to read from</param>
        /// <returns>A list of all column data as strings</returns>
        public List<string> ReadAllDataFromAColumn(string columnName)
        {
            List<string> columnData = new List<string>();
            int columnIndex = FindColumnIndex(columnName);

            foreach (IWebElement row in _tableRows)
            {
                columnData.Add(row.FindElement(By.XPath(string.Format("td[{0}]", columnIndex))).Text);
            }

            return columnData;
        }

        /// <summary>
        /// Read the value of a cell for the row that contains a known value
        /// </summary>
        /// <param name="columnName">The column name to read value from</param>
        /// <param name="knownValue">The value to find a matching row</param>
        /// <returns>The value of cell as a string</returns>
        public string ReadColumnValueForRowContaining(string columnName, string knownValue)
        {
            IWebElement requiredRow = FindFirstRowByKnownValue(knownValue);
            IWebElement requiredCell = FindCellByRowAndColumnName(requiredRow, columnName);

            return requiredCell.Text;
        }

        /// <summary>
        /// Read a cells value by finding the value using a known vale from a specific column
        /// </summary>
        /// <param name="columnToRead">Column to read data from</param>
        /// <param name="knownValue">Known value to match</param>
        /// <param name="knownValueColumn">Column which should contain the known value</param>
        /// <returns>Value of the matching cell as a string</returns>
        public string ReadAColumnForRowContainingValueInColumn(string columnToRead, string knownValue, string knownValueColumn)
        {
            IWebElement requiredRow = FindRowMatchingColumnData(knownValueColumn, knownValue);
            IWebElement requiredCell = FindCellByRowAndColumnName(requiredRow, columnToRead);

            return requiredCell.Text;
        }
    }
}