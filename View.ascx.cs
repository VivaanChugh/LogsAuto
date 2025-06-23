using System;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using CsvHelper;
using System.Globalization;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Vivaan.Modules.LogsAuto
{
    public partial class View : LogsAutoModuleBase
    {
        protected void btnValidate_Click(object sender, EventArgs e)
        {
            if (!fuCsvUpload.HasFile)
            {
                lblResult.Text = "Please upload a CSV file.";
                lblResult.ForeColor = System.Drawing.Color.Red;
                return;
            }

            var failedRows = new List<ValidationResult>();

            try
            {
                using (var reader = new StreamReader(fuCsvUpload.FileContent))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<dynamic>().ToList();
                    int rowNumber = 2; // Header is row 1

                    foreach (var record in records)
                    {
                        var dict = (IDictionary<string, object>)record;
                        var failures = new List<string>();

                        switch (ddlLogType.SelectedValue)
                        {
                            case "1":
                                failures = ValidateLogType1(dict);
                                break;
                            case "2":
                                failures = ValidateLogType2(dict);
                                break;
                            case "3":
                                failures = ValidateLogType3(dict);
                                break;
                            case "4":
                                failures = ValidateLogType4(dict);
                                break;
                            case "5":
                                failures = ValidateLogType5(dict);
                                break;
                            case "6":
                                failures = ValidateLogType6(dict);
                                break;

                            case "7":
                                failures = ValidateLogType7(dict);
                                break;
                            case "8":
                                failures = ValidateLogType8(dict);
                                break;
                            case "9":
                                failures = ValidateLogType9(dict);
                                break;
                            default:
                                failures.Add("Invalid log type selected.");
                                break;
                        }

                        if (failures.Any())
                        {
                            failedRows.Add(new ValidationResult
                            {
                                Row = rowNumber,
                                FailedValidations = string.Join(", ", failures)
                            });
                        }

                        rowNumber++;
                    }
                }

                gvValidationResults.DataSource = failedRows.Count > 0 ? failedRows : null;
                gvValidationResults.DataBind();

                lblResult.Text = failedRows.Count > 0
                    ? $"{failedRows.Count} row(s) failed validation."
                    : "All validations passed!";
                lblResult.ForeColor = failedRows.Count > 0
                    ? System.Drawing.Color.Red
                    : System.Drawing.Color.Green;
            }
            catch (Exception ex)
            {
                lblResult.Text = $"Error: {ex.Message}";
                lblResult.ForeColor = System.Drawing.Color.Red;
            }
        }

        private List<string> ValidateLogType1(IDictionary<string, object> row)
        {
            List<string> errors = new List<string>();
            string Get(string key) => row.ContainsKey(key) ? row[key]?.ToString()?.Trim() ?? "" : "";
            string hostIP = Get("Host_IP");
            

            string hostName = Get("HostName(Without_dot)");
            string department = Get("department");
            string deviceType = Get("Device_Type");
            string logStatus = Get("log_status_24hrs");

            // Host-IP Match Validation 
            if (((hostIP == "10.58.17.20") && (hostName != "SBICSRV04")) || ((hostIP == "10.58.17.19") && (hostName != "SBICSRV03")) || ((hostIP == "10.58.17.109") && (hostName != "SBICSRV02")) || ((hostIP == "10.58.37.37") && (hostName != "SBICSRV07")) || ((hostIP == "10.58.17.34") && (hostName != "SBICSRV11")) || ((hostIP == "10.58.17.33") && (hostName != "SBICSRV12")) || ((hostIP == "10.58.17.32") && (hostName != "SBICSRV01")))
            {
                errors.Add($"Host-IP Match Error (Found: IP = {hostIP}, Name = {hostName})");
            }

            // Department/IP Match Validation
            if ((department == "FO - Canada") && (!new[] { "10.58.17.20", "10.58.17.19", "10.58.17.109", "10.58.37.37", "10.58.17.34", "10.58.17.33", "10.58.17.32" }.Contains(hostIP)))
            {
                errors.Add($"Department/IP Match Error (Found: Department = {department}, IP = {hostIP})");
            }

            var AppServer = new List<string> { "10.58.17.20", "10.58.17.19", "10.58.37.37", "10.58.17.32" };
            var Data = new List<string> { "10.58.17.109", "10.58.17.34" };
            var Web = new List<string> { "10.58.17.33" };


            // Device Type/IP Match Validation
            if (AppServer.Contains(hostIP) && deviceType != "Application_Server")
                errors.Add($"Device Type/IP Match Error (Device Type = {deviceType}, IP = {hostIP})");
            if (Data.Contains(hostIP) && deviceType != "Database")
                errors.Add($"Device Type/IP Match Error (Device Type = {deviceType}, IP = {hostIP})");
            if (Web.Contains(hostIP) && deviceType != "Web_Server")
                errors.Add($"Device Type/IP Match Error (Device Type = {deviceType}, IP = {hostIP})");

            // Log Status Validation
            if (logStatus != "Log received")
                errors.Add($"Log Status Validity Error (Found: Log Status = {logStatus}, Expected: Log Status = Log received)");

            // Device Criticality Validation
            var validCrit = new List<string> { "Low", "Medium", "High", "Critical" };
            if (!validCrit.Contains(Get("Device_Criticality")))
                errors.Add($"Device Criticality Error (Found: {Get("Device_Criticality")}, Expected: Low, Medium, High, or Critical)");

            string deviceState = Get("Device_State");
            string environment = Get("Environment");

            // Device State/Environment Validation
            if (hostName == "SBICSRV07" && (deviceState != "DR-Passive" || environment != "DR"))
                errors.Add($"Device State/Environment Error for {hostName}");
            else if (hostName != "SBICSRV07" && (deviceState != "DC-Active" || environment != "PR"))
                errors.Add($"Device State/Environment Error for {hostName}");

            if (string.IsNullOrWhiteSpace(Get("Application")))
                errors.Add("Application Tag Error (Missing)");

            string osStatus = Get("OS_Integrated_Status");
            if (osStatus != "Integrated" && osStatus != "Not Applicable")
                errors.Add($"OS Integrated Status Error (Found: {osStatus}, Expected: Integrated or Not Applicable)");

            switch (DropDownList1.SelectedValue)
            {
                case "1":
                    DateTime today = DateTime.Today;
                    int month = today.Month;
                    int day = today.Day;
                    int year = today.Year;
                    string logDateStr = Get("lastLogRecieved");

                    string[] dateParts = Regex.Split(logDateStr, @"[/\-]");
                    if (dateParts.Length != 3 || !int.TryParse(dateParts[0], out int logDay) || !int.TryParse(dateParts[1], out int logMonth) || !int.TryParse(dateParts[2], out int logYear))
                    {
                        errors.Add($"Log Date Format Error (Found: {logDateStr}, Expected: MM/DD/YYYY or DD-MM-YYYY)");
                        break;
                    }
            

                    if (logMonth != month || logDay != day || logYear != year)
                    {
                        errors.Add($"Log Date Error (Found: {logDateStr}, Expected: {month}/{day}/{year})");

                    }
                    break;
                case "2":

                    break;
            }
            

            return errors;

        }

        private List<string> ValidateLogType2(IDictionary<string, object> row)
        {
            // TODO: Add your own validation rules for Log Type 2
            List<string> errors = new List<string>();
            errors.Add("Validation for Log Type 2 is not implemented yet.");
            return errors;
        }

        private List<string> ValidateLogType3(IDictionary<string, object> row)
        {
            // TODO: Add your own validation rules for Log Type 3
            List<string> errors = new List<string>();
            errors.Add("Validation for Log Type 3 is not implemented yet.");
            return errors;
        }

        private List<string> ValidateLogType4(IDictionary<string, object> row)
        {
            // TODO: Add your own validation rules for Log Type 4
            List<string> errors = new List<string>();
            errors.Add("Validation for Log Type 4 is not implemented yet.");
            return errors;
        }

        private List<string> ValidateLogType5(IDictionary<string, object> row)
        {
            // TODO: Add your own validation rules for Log Type 5
            List<string> errors = new List<string>();
            errors.Add("Validation for Log Type 5 is not implemented yet.");
            return errors;
        }


        private List<string> ValidateLogType6(IDictionary<string, object> row)
        {
            // Add your own validation rules for Log Type 6
            List<string> errors = new List<string>();
            errors.Add("Log Type 6");
            return errors;
        }

        private List<string> ValidateLogType7(IDictionary<string, object> row)
        {
            // TODO: Add your own validation rules for Log Type 7
            List<string> errors = new List<string>();
            errors.Add("Log Type 7");
            return errors;
        }

        private List<string> ValidateLogType8(IDictionary<string, object> row)
        {
            // TODO: Add your own validation rules for Log Type 8
            List<string> errors = new List<string>();
            errors.Add("Log Type 8");
            return errors;
        }

        private List<string> ValidateLogType9(IDictionary<string, object> row)
        {
            // TODO: Add your own validation rules for Log Type 9
            List<string> errors = new List<string>();
            errors.Add("Log Type 9");
            return errors;
        }



        public class ValidationResult
        {
            public int Row { get; set; }
            public string FailedValidations { get; set; }
        }
    }
}
