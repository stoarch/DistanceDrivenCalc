using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//
//
using System.IO;
using System.Data;
using Microsoft.Office.Interop.Excel;
using System.Data.OleDb;

namespace DistanceDrivenCalc
{
    class Data {
        public string ArrivalDateStr;
        public string Name;
        public string Street;
        public string Zip;
        public string City;
    }

    class ExcelReader
    {
        public string Path = "";
        public string NameFile = "";

        static private string error = "";
        static string errorMessage;
        static Data fileData;
        static private string CommonStrCheck;

        static int RowCount = 1;

        static string CurrentError = "";
        //static string strErr = "";

        public ExcelReader()
        {            
        }

        public bool LoadXLS()
        {
            bool Res = false;
            error = "Ok";

            fileData = new Data();
            Res = ProcessExcelFile(Path, NameFile);
            if (!Res)
                if (error == "Ok")
                    error = "Invalid file.";

            return Res;
        }

        public string GetError()
        {
            return error;
        }

        public Data[] ReturnExcelSheetData()
        {
            Data[] excelData = new Data[RowCount];

            for (int i = 0; i < RowCount; i++)
            {
                fileData = GetRowData(i);
                excelData[i] = fileData;
            }

            return excelData;
        }

        Data GetRowData(int rowIndex)
        {
            Data Elem = new Data();
            try
            {
                int commaPos = 0;
                string strTemp = CommonStrCheck;
                string temp;
                while (true)
                {
                    commaPos = strTemp.IndexOf(";");
                    if (commaPos == -1)
                    {
                        temp = strTemp;
                        if (temp == "")
                        {
                            break;
                        }
                        int eqIndex = temp.IndexOf("=");

                        string DName = temp.Substring(0, eqIndex);
                        string DResult = temp[(eqIndex + 1)..];

                        break;
                    }
                    else
                    {
                        temp = strTemp;
                        string curStr = temp.Substring(0, commaPos);
                        curStr = curStr.Replace("%", ";");

                        string[] strArr;
                        char[] charArr = new char[] { ';' };
                        strArr = curStr.Split(charArr);

                        string tmp = Convert.ToString(strArr[0].Trim());
                        if (tmp.Contains("num"))
                        {
                            tmp = tmp.Replace("num", "");
                            if ((rowIndex + 1) == Convert.ToInt32(tmp))
                            {
                                CurrentError = "Ok";

                                Elem.ArrivalDateStr = ToString(strArr, 1, "data");
                                Elem.Name = ToString(strArr, 2, "nazwa"); 
                                Elem.Street = ToString(strArr, 3, "ulica"); 
                                Elem.Zip = ToString(strArr, 4, "kod pocztowy");
                                Elem.City = ToString(strArr, 3, "miasto");

                                if (CurrentError != "Ok")
                                    return null;
                                
                                return Elem;
                            }
                        }

                        strTemp = strTemp[(commaPos + 1)..temp.Length];
                    }
                }
            }
            catch
            {
                return null;
            }
            return Elem;
        }

        private static string ToString(string[] rowData, int index, string columnName)
        {
            if (rowData.Length <= index)
            {
                CurrentError = "Error converting param #" + index.ToString() + CurrentError;
                return "";
            }
            string Data = rowData[index].Trim();
            try
            {
                return Data;
            }
            catch (Exception)
            {
                CurrentError = "Error converting to string:" + Data + ". str:" + columnName + ", parm#" + index.ToString() + CurrentError;
                return "";
            }
        }

        private static Int32 ToInt32(string[] rowData, int Index, string columnName)
        {
            if (rowData.Length <= Index)
            {
                CurrentError = "Error converting parameter#" + Index.ToString() + CurrentError;
                return 0;
            }

            string Data = rowData[Index].Trim();

            try
            {
                return Convert.ToInt32(Data);
            }
            catch (Exception)
            {
                CurrentError = "Error converting to (int):" + Data + ". column:" + columnName + ", param#" + Index.ToString() + CurrentError;
                return 1;
            }
        }

        private static bool ProcessExcelFile(string Path, string fileName, int FormatXLS = 1)
        {
            bool Res;
            try
            {
                FileInfo excelFile = new FileInfo(Path + @"\" + fileName);
                if (!excelFile.Exists)
                {
                    fileName = fileName.Replace("xlsx", "xls");
                    excelFile = new FileInfo(Path + @"\" + fileName);
                    if (!excelFile.Exists)
                    {
                        error = "File not found " + fileName + " for loading.";
                        return false;
                    }
                }

                int ind = 1;
                DataSet ds = new DataSet();
                string connectionString = GetConnectionString(excelFile);

                using (var conn = new OleDbConnection(connectionString))
                {
                    conn.Open();
                    var cmd = new OleDbCommand();
                    cmd.Connection = conn;

                    // Get all Sheets in Excel File
                    System.Data.DataTable dtSheet = conn.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables, null);

                    // Loop through all Sheets to get data
                    foreach (DataRow dr in dtSheet.Rows)
                    {
                        string sheetName = dr["TABLE_NAME"].ToString();

                        // Get all rows from the Sheet
                        cmd.CommandText = "SELECT * FROM [" + sheetName + "]";

                        var dt = new System.Data.DataTable();
                        dt.TableName = sheetName;

                        OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                        da.Fill(dt);

                        ds.Tables.Add(dt);
                        break;
                    }

                    List<List<string>> strTable = new List<List<string>>();

                    int rowCount = ds.Tables[0].Rows.Count;
                    int colCount = ds.Tables[0].Columns.Count;

                    string separator = "%";
                    CommonStrCheck = "";

                    for (int i = 0; i < rowCount; i++)
                    {
                        fileData.ArrivalDateStr = ToStringXLS(ds.Tables[0].Rows[i], 1, "head");
                        fileData.Name = ToStringXLS(ds.Tables[0].Rows[i], 2, "head");
                        fileData.Street = ToStringXLS(ds.Tables[0].Rows[i], 3, "head"); 
                        fileData.Zip = ToStringXLS(ds.Tables[0].Rows[i], 4, "head"); 
                        fileData.City = ToStringXLS(ds.Tables[0].Rows[i], 5, "head"); 

                        string TempStr = "num" +
                            ind.ToString() + separator +
                            fileData.ArrivalDateStr + separator +
                            fileData.Name + separator +
                            fileData.Street + separator +
                            fileData.Zip + separator +
                            fileData.City + separator;
                            CommonStrCheck = CommonStrCheck + TempStr + ";";
                        ind++;

                    }
                }
                RowCount = ind;

                if (ind > 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                error = "For reading file, Error: " + Convert.ToString(ex);
                Res = false;
            }
            return Res;
        }

        static string GetConnectionString(FileInfo fileInfo)
        {
            Dictionary<string, string> props = new Dictionary<string, string>();

            // XLSX - Excel 2007, 2010, 2012, 2013
            if (fileInfo.Extension == ".xlsx")
            {
                props["Provider"] = "Microsoft.ACE.OLEDB.12.0;";
                props["Extended Properties"] = "Excel 12.0 XML";
                props["Data Source"] = fileInfo.FullName;
            }
            else if (fileInfo.Extension == ".xls")
            {
                props["Provider"] = "Microsoft.Jet.OLEDB.4.0";
                props["Extended Properties"] = "Excel 8.0";
                props["Data Source"] = fileInfo.FullName;
            }
            else throw new Exception("Unknown file extension!");

            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<string, string> prop in props)
            {
                sb.Append(prop.Key);
                sb.Append('=');
                sb.Append(prop.Value);
                sb.Append(';');
            }

            return sb.ToString();
        }

        private static string ToStringXLS(System.Data.DataRow rowData, int index, string columnName)
        {
            try
            {
                string Data = Convert.ToString(rowData.ItemArray[index]);

                return Data;
            }
            catch (System.IndexOutOfRangeException Err)
            {
                errorMessage = "Error (" + Err.Message + ") retrieving data from column: " + columnName + " by index " + "[" + index.ToString() + "]";
                return "";
            }

            catch (FormatException)
            {
                errorMessage = "Error converting value:" + rowData + ", param#" + index.ToString();
                return "";
            }
        }

        private static Int32 ToInt32XLS(object rowData, int index, string IdStr)
        {
            try
            {
                string Data = Convert.ToString(rowData);
                int retInt = Convert.ToInt32(Data);

                return retInt;
            }
            catch (FormatException)
            {
                errorMessage = "Unable to convert row data to integer:" + rowData + ", param#" + index.ToString();
                return 0;
            }
        }
    }
}

