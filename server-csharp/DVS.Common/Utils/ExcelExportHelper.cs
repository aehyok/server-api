using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace DVS.Common
{
    /// <summary>
    /// Excel导出帮助类
    /// </summary>
    public static class ExcelExportHelper
    {
        public static string ExcelContentType => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        /// <summary>
        /// List转DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DataTable ListToDataTable<T>(List<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable dataTable = new DataTable();
            for (int i = 0; i < properties.Count; i++)
            {
                PropertyDescriptor property = properties[i];
                dataTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
            }
            object[] values = new object[properties.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = properties[i].GetValue(item);
                }
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }

        /// <summary>
        /// List转DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DataTable ListToDataTable<T>(List<T> data, Dictionary<string, string> columns)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable dataTable = new DataTable();
            foreach (var item in columns)
            {
                // PropertyDescriptor property = properties[i];
                // dataTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
                dataTable.Columns.Add(item.Value);
            }
            object[] values = new object[columns.Count];
            foreach (T item in data)
            {
                //for (int i = 0; i < values.Length; i++)
                //{
                //    values[i] = properties[i].GetValue(item);
                //}
                int i = 0;
                foreach (var column in columns)
                {
                    // values[i] = properties[i].;
                    var p = properties.Find(column.Key, false);
                    values[i] = p.GetValue(item);
                    i += 1;
                }

                dataTable.Rows.Add(values);
            }
            return dataTable;
        }
        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="dataTable">数据源</param>
        /// <param name="heading">工作簿Worksheet</param>
        /// <param name="showSrNo">//是否显示行编号</param>
        /// <param name="columnsToTake">要导出的列</param>
        /// <returns></returns>
        public static byte[] ExportExcel(DataTable dataTable, string[] headings, Dictionary<string, string> columnsToTake, bool showSrNo = false)
        {
            byte[] result;
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet workSheet = package.Workbook.Worksheets.Add("Sheet1");
                int startRowFrom = headings == null || headings.Length == 0 ? 1 : headings.Length + 1;  //开始的行
                //是否显示行编号
                if (showSrNo)
                {
                    DataColumn dataColumn = dataTable.Columns.Add("#", typeof(int));
                    dataColumn.SetOrdinal(0);
                    int index = 1;
                    foreach (DataRow item in dataTable.Rows)
                    {
                        item[0] = index;
                        index++;
                    }
                }
                //Add Content Into the Excel File
                workSheet.Cells["A" + startRowFrom].LoadFromDataTable(dataTable, true);
                // autofit width of cells with small content 
                int columnIndex = 1;
                foreach (DataColumn item in dataTable.Columns)
                {
                    ExcelRange columnCells = workSheet.Cells[workSheet.Dimension.Start.Row, columnIndex, workSheet.Dimension.End.Row, columnIndex];
                    int maxLength = columnCells.Max(cell => cell.Value.ToString().Count());
                    if (maxLength < 150)
                    {
                        workSheet.Column(columnIndex).AutoFit();
                    }
                    columnIndex++;
                }
                // format header - bold, yellow on black 
                using (ExcelRange r = workSheet.Cells[startRowFrom, 1, startRowFrom, dataTable.Columns.Count])
                {
                    r.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Font.Bold = true;
                    r.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    r.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#ffffff"));
                }
                // format cells - add borders 
                if (dataTable.Rows.Count > 0)
                {
                    using (ExcelRange r = workSheet.Cells[startRowFrom + 1, 1, startRowFrom + dataTable.Rows.Count, dataTable.Columns.Count])
                    {
                        r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        r.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                        r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                        r.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                        r.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                    }
                }
                // removed ignored columns 
                //for (int i = dataTable.Columns.Count - 1; i >= 0; i--)
                //{
                //    if (i == 0 && showSrNo)
                //    {
                //        continue;
                //    }
                //    if (!columnsToTake.Contains(dataTable.Columns[i].ColumnName))
                //    {
                //        workSheet.DeleteColumn(i + 1);
                //    }
                //}

                if (headings != null)
                {
                    for (int i = 0; i < headings.Length; i++)
                    {
                        // workSheet.InsertRow(1, 1);
                        string cellsIndex = "A" + (i + 1);
                        workSheet.Cells[cellsIndex].Value = headings[i];
                        workSheet.Cells[cellsIndex].Style.Font.Size = 12;
                        workSheet.Cells[(i + 1), 1, (i + 1), columnsToTake.Count].Merge = true;
                        if (i == 0)
                        {
                            // 第一行居中
                            workSheet.Cells[cellsIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        }
                        // fromRow, fromCol, toRow, toCol
                        // workSheet.InsertColumn(1, 1);
                        // workSheet.InsertRow(i+1, 1);
                        // workSheet.Column(1).Width = 5;
                    }

                }
                result = package.GetAsByteArray();
                return result;
            }
        }
        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="headings"></param>
        /// <param name="isShowSlNo"></param>
        /// <param name="columnsToTake"></param>
        /// <returns></returns>
        public static byte[] ExportExcel<T>(List<T> data, string[] headings, Dictionary<string, string> columnsToTake, bool isShowSlNo = false)
        {
            return ExportExcel(ListToDataTable(data, columnsToTake), headings, columnsToTake, isShowSlNo);
        }
    }
}
