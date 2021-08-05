
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emr_web.Common
{
    public struct Age
    {
        public int years;
        public int months;
        public int days;
    };

    public class CommonSetting
    {
        public static Age GetAge(String ageBuf)
        {
            Age age = new Age();

            if (!string.IsNullOrEmpty(ageBuf))
            {
                int length = ageBuf.Length;
                int yearIndex = ageBuf.IndexOf("Y", 0, length, StringComparison.CurrentCulture);
                int monthIndex = ageBuf.IndexOf("M", 0, length, StringComparison.CurrentCulture);
                int DayIndex = ageBuf.IndexOf("D", 0, length, StringComparison.CurrentCulture);

                String buffer = ageBuf.Substring(0, yearIndex);
                if (!string.IsNullOrEmpty(buffer))
                    age.years = Convert.ToInt32(buffer);

                buffer = ageBuf.Substring(yearIndex + 1, (monthIndex - yearIndex - 1));
                if (!string.IsNullOrEmpty(buffer))
                    age.months = Convert.ToInt32(buffer);

                buffer = ageBuf.Substring(monthIndex + 1, (DayIndex - monthIndex - 1));
                if (!string.IsNullOrEmpty(buffer))
                    age.days = Convert.ToInt32(buffer);
            }
            return age;
        }
        public static Age GetAge(DateTime dob, DateTime visiteDate)
        {
            Age age = new Age();
            if (visiteDate > dob)
            {
                TimeSpan ts = visiteDate.Subtract(dob);
                DateTime buf = DateTime.MinValue + ts;
                age.years = buf.Year - 1;
                age.months = buf.Month - 1;
                age.days = buf.Day - 1;
                //string s = string.Format("{0} Years {1} months {2} days", buf.Year - 1, buf.Month - 1, buf.Day - 1);
            }
            else
            {
                age.years = -1;
                age.months = -1;
                age.days = -1;
            }


            return age;
        }
        public static string generateStudyID()
        {
            string name = "Visit-";
            DateTime now = DateTime.Now;
            name += now.ToString("ddMMyyyy") + "-" + now.ToString("hhmmss");
            return name;
        }
        public static string GetPurchaseOrderNumber()
        {
            string name = "Pur-";
            DateTime now = DateTime.Now;
            name += now.ToString("ddMMyyyy") + "-" + now.ToString("hhmmss");
            return name;
        }
        public static byte[] GetImageBytes(IFormFile formFile)
        {
            string imagebytes = "";
            byte[] image = null;
            try
            {
                using (var ms = new MemoryStream())
                {
                    formFile.CopyTo(ms);
                    image = ms.ToArray();
                    imagebytes = Convert.ToBase64String(image);
                }
            }
            catch (Exception ex)
            {

            }
            return image;
        }
        public static DataTable ParseCSVFile(string fileName, char delimiter, bool isFirstLineHeader)
        {
            DataTable dataTable = new DataTable();
            if (!File.Exists(fileName))
            {
            }
            else
            {
                StreamReader tr = new StreamReader(fileName, Encoding.GetEncoding(1250));
                try
                {
                    string line = tr.ReadLine();
                    string[] columns = line.Split(delimiter);

                    if (isFirstLineHeader)
                    {
                        foreach (string col in columns)
                        {
                            dataTable.Columns.Add(new DataColumn(col.Trim()));
                        }
                        dataTable.Columns.Add(new DataColumn("IsDataAvailable"));

                    }
                    else
                    {
                        foreach (string col in columns)
                        {
                            dataTable.Columns.Add(new DataColumn());
                        }
                        dataTable.Columns.Add(new DataColumn("IsDataAvailable"));

                        int cnt = 0;
                        DataRow dr = dataTable.NewRow();
                        foreach (string col in columns)
                        {
                            dr[cnt++] = col.Trim();
                        }
                        dataTable.Rows.Add(dr);
                    }

                    line = tr.ReadLine();

                    for (; line != null; line = tr.ReadLine())
                    {
                        columns = line.Split(delimiter);

                        int cnt = 0;
                        DataRow dr = dataTable.NewRow();
                        bool isDataAvailable = false;
                        foreach (string col in columns)
                        {
                            dr[cnt++] = col.Trim();
                            if (!col.Trim().Equals(String.Empty))
                                isDataAvailable = true;
                        }
                        if (isDataAvailable)
                            dr["IsDataAvailable"] = true;
                        else
                            dr["IsDataAvailable"] = false;
                        dataTable.Rows.Add(dr);
                    }

                    for (int column = 0; column < dataTable.Columns.Count; column++)
                    {
                        if (dataTable.Columns[column].ColumnName.Trim().Contains("Column"))
                        {
                            bool isColumnDataAvailable = false;
                            for (int row = 0; row < dataTable.Rows.Count; row++)
                            {
                                if (dataTable.Rows[row][column].ToString().Trim().Length != 0)
                                {
                                    isColumnDataAvailable = true;
                                    break;
                                }
                            }

                            if (!isColumnDataAvailable)
                            {
                                dataTable.Columns.RemoveAt(column);
                                column--;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    tr.Close();
                }
            }
            return dataTable;
        }
        public static bool IsValidImportCSVfile(string filename, char delimiter)
        {
            bool isvalid = true;
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException();
            }
            TextReader tr = new StreamReader(filename);
            try
            {
                string header = tr.ReadLine();

                string[] columns = header.Split(delimiter);

                isvalid &= (columns.Length >= 8);

                if (isvalid)
                {
                    isvalid &= columns[0].ToLower().Contains("DrugName");
                    isvalid &= columns[1].ToLower().Contains("Category");
                    isvalid &= columns[2].ToLower().Contains("Uom");
                    isvalid &= columns[3].ToLower().Contains("Gst");
                    isvalid &= columns[4].ToLower().Contains("ScheduleType");
                    isvalid &= columns[5].ToLower().Contains("HSnCode");
                    isvalid &= columns[6].ToLower().Contains("Company");
                    isvalid &= columns[6].ToLower().Contains("Type");
                }
            }
            catch (Exception exp)
            {
                isvalid = false;
            }
            finally
            {
                tr.Close();
                isvalid = true;
            }
            return isvalid;
        }
        public static DateTime getDataformat(string DateValue, DateTime date)
        {
            string sysFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
            try
            {
                if (DateValue != null || DateValue != "")
                {
                    IFormatProvider cultureDDMMYYYY = new CultureInfo("fr-Fr", true);
                    IFormatProvider cultureMMDDYYYY = new CultureInfo("en-US", true);
                    DateTime currentDate = DateTime.Now;
                    IFormatProvider culture = cultureDDMMYYYY;
                    DateTime.TryParse(DateValue, culture, DateTimeStyles.NoCurrentDateDefault, out date);
                }
            }
            catch (Exception ex)
            {

            }
            return date;
        }
    }
}

