using System.IO;
using System.Collections.Generic;
using System;
using OpenQA.Selenium;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;


//using org.apache.poi.ss.usermodel.*;
//using org.apache.poi.xssf.usermodel.XSSFSheet;
//using org.apache.poi.xssf.usermodel.XSSFWorkbook;


/**
 * @author driscollj
 * edited by sahil.srivastava on 16th Jan 2020 - added {@link #convertXlsxToCSV(driver, File, string)}
 */

namespace Automation_Framework.Helpers.Files
{
    public class ExcelHelper
    {



        private ExcelHelper()
        {
        }


        /**
         * Excel file converter
         *
         * @param file the excel file
         * @return string representing contents of the file
         */
        public static string convertXlsxToString(string filePath)
        {
            try
            {
                using (SpreadsheetDocument excel = SpreadsheetDocument.Open(filePath, false))
                {
                    WorkbookPart wkb = excel.WorkbookPart;
                    //select the first sheet        
                    WorksheetPart wks = wkb.WorksheetParts.First();
                    string fileToString = string.Empty;
                    SheetData sheetData = wks.Worksheet.Elements<SheetData>().First();
                    var stringTable = wkb.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                    foreach (Row r in sheetData.Elements<Row>())
                    {
                        foreach (Cell theCell in r.Elements<Cell>())
                        {
                            // If the cell represents an integer number, you are done. 
                            // For dates, this code returns the serialized value that 
                            // represents the date. The code handles strings and 
                            // Booleans individually. For shared strings, the code 
                            // looks up the corresponding value in the shared string 
                            // table. For Booleans, the code converts the value into 
                            // the words TRUE or FALSE.
                            string value = theCell.CellValue.Text;
                            if (theCell.DataType != null)
                            {
                                switch (theCell.DataType.Value)
                                {
                                    case CellValues.Boolean:
                                        if (value == "0")
                                        {
                                            value = "FALSE";
                                        }
                                        else
                                            value = "TRUE";
                                        break;
                                    case CellValues.SharedString:
                                        if (stringTable != null)
                                        {
                                            value = stringTable.SharedStringTable.ElementAt(int.Parse(value)).InnerText;
                                        }
                                        break;
                                    default:
                                        break;

                                }
                            }
                            fileToString = fileToString + value + " ";
                            Console.Write(fileToString + " ");
                        }
                        fileToString = fileToString + Environment.NewLine;
                    }
                    return fileToString;
                }
            }
            catch (IOException e)
            {
                Serilog.Log.Error("Exception reading excel file: " + e.Message);
                return null;
            }
            

        }

        /**
         * Read from XLSX file, return CSV formatted string.
         *
         * @param file XLSX file.
         * @return CSV string.
         */
        public static string convertXlsxToCSV(IWebDriver driver, string filePath)
        {
            return convertXlsxToCSV(driver, filePath, null);
        }

        public static string convertXlsxToCSV(IWebDriver driver, string filePath, string sheetName)
        {
            //StringBuffer fileContent = new StringBuffer();

            //try (InputStream inputStream = new FileInputStream(filePath)) {
            //    try (Workbook wb = WorkbookFactory.create(inputStream)) {
            //        if (sheetName != null) {
            //            if (wb.getSheet(sheetName) == null) {
            //                Serilog.Log.Error("Sheet with name \"" + sheetName + "\" not found in workbook/excel file \"" + file.getName() + "\"");
            //            } else {
            //                fileContent.append(sheetName);
            //                fileContent.append("\n");
            //                fileContent.append(getXlsxSheetCSVContent(driver, wb.getSheet(sheetName)));
            //                fileContent.append("\n");
            //            }
            //        } else {
            //            for (int i = 0; i < wb.getNumberOfSheets(); i++) {
            //                fileContent.append(wb.getSheetAt(i).getSheetName());
            //                fileContent.append("\n");
            //                fileContent.append(getXlsxSheetCSVContent(driver, wb.getSheetAt(i)));
            //                fileContent.append("\n");
            //            }
            //        }
            //    }
            //} catch (FileNotFoundException ex) {
            //    Serilog.Log.Error("Error while reading XLSX file.", ex);
            //} catch (IOException ex) {
            //    Serilog.Log.Error("Error while reading XLSX file.", ex);
            //}
            //return fileContent.toString();
            return string.Empty;
        }

        /**
         * Read from XLSX sheet.
         *
         * @param sheet XLSX sheet objectValue.
         * @return CSV string of the sheet content.
         */
        //private static string getXlsxSheetCSVContent(IWebDriver driver, Sheet sheet) {
        //    List<string> rowContents = new List<string>();
        //    FormulaEvaluator evaluator = sheet.getWorkbook().getCreationHelper().createFormulaEvaluator();

        //    for (int i = 0; i <= sheet.getLastRowNum(); i++) {
        //        Row row = sheet.getRow(i);
        //        List<string> cellContents = new List<string>();

        //        if (row != null) {
        //            for (int j = 0; j < row.getLastCellNum(); j++) {
        //                if (row.getCell(j) != null && row.getCell(j).getCellType().Equals(CellType.FORMULA)) {
        //                    cellContents.Add("'" + evaluator.evaluate(row.getCell(j)).formatAsString() + "'");
        //                } else {
        //                    cellContents.Add("'" + row.getCell(j) + "'");
        //                }
        //            }
        //            rowContents.Add(string.join(",", cellContents));
        //        }
        //    }
        //    return string.join("\n", rowContents);
        //}
    }
}
