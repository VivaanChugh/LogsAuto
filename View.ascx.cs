using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using CsvHelper;
using System.Globalization;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;

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
                        var failures = ValidateRow(dict);
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

                if (failedRows.Count > 0)
                {
                    gvValidationResults.DataSource = failedRows;
                    gvValidationResults.DataBind();
                    lblResult.Text = $"{failedRows.Count} row(s) failed validation.";
                    lblResult.ForeColor = System.Drawing.Color.Red;
                }
                else
                {
                    gvValidationResults.DataSource = null;
                    gvValidationResults.DataBind();
                    lblResult.Text = "All validations passed!";
                    lblResult.ForeColor = System.Drawing.Color.Green;
                }
            }
            catch (Exception ex)
            {
                lblResult.Text = $"Error: {ex.Message}";
                lblResult.ForeColor = System.Drawing.Color.Red;
            }
        }

        private List<string> ValidateRow(IDictionary<string, object> row)
        {
            List<string> errors = new List<string>();
            string Get(string key) => row.ContainsKey(key) ? row[key]?.ToString()?.Trim() ?? "" : "";
            string hostIP = Get("Host_IP");
            string hostName = Get("HostName(Without_dot)");
            string department = Get("department");
            string deviceType = Get("Device_Type");
            string logStatus = Get("log_status_24hrs");
            // 1. Host-IP Match
            if (((hostIP == "10.58.17.20") && (!(hostName == "SBICSRV04"))) || ((hostIP == "10.58.17.19") && (!(hostName == "SBICSRV03"))) || ((hostIP == "10.58.17.109") && (!(hostName == "SBICSRV02"))) || ((hostIP == "10.58.37.37") && (!(hostName == "SBICSRV07"))) || ((hostIP == "10.58.17.34") && (!(hostName == "SBICSRV11"))) || ((hostIP == "10.58.17.33") && (!(hostName == "SBICSRV12"))) || ((hostIP == "10.58.17.32") && (!(hostName == "SBICSRV01"))))
            {
                errors.Add($"Host-IP Match Error (Found: IP = {hostIP}, Name = {hostName})");
            }
           
            
            // 2. Log Status Validity
            if ((department == "FO - Canada") && (!((hostIP == "10.58.17.20") || (hostIP == "10.58.17.19") || (hostIP == "10.58.17.109") || (hostIP == "10.58.37.37") || (hostIP == "10.58.17.34") || (hostIP == "10.58.17.33") || (hostIP == "10.58.17.32"))))
            {
                errors.Add($"Department/IP Match Error (Found: Department = {department}, IP = {hostIP})");
            }

            List<string> AppServer = new List<string> { "10.58.17.20", "10.58.17.19", "10.58.37.37", "10.58.17.32" };
            List<string> Data = new List<string> { "10.58.17.109", "10.58.17.34"};
            List<string> Web = new List<string> { "10.58.17.33" };
            // 3. Device Type/IP Match
            if  ((AppServer.Contains(hostIP)) && !(deviceType == "Application_Server"))
            {
                errors.Add($" Device Type/IP Match Error (Found: Device Type = {deviceType}, IP = {hostIP})");
            }
            if ((Data.Contains(hostIP)) && !(deviceType == "Database"))
            {
                errors.Add($" Device Type/IP Match Error (Found: Device Type = {deviceType}, IP = {hostIP})");
            }
            if ((Web.Contains(hostIP)) && !(deviceType == "Web_Server"))
            {
                errors.Add($" Device Type/IP Match Error (Found: Device Type = {deviceType}, IP = {hostIP})");
            }

            // 4. Log Status Validity
            if (logStatus != "Log received")
            {
                errors.Add($"Log Status Validity Error (Found: Log Status = {logStatus})");
            }

            // 5. Source Validation


            // 6. New Device Detection

            List<string> validlist = new List<string> { "Low", "Medium", "High", "Critical" };
            // 7. Device Criticality Validation
            if (!(validlist.Contains(Get("Device_Criticality"))))
            {
                errors.Add($"Device Criticality (Found: Device Criticality = {Get("Device_Criticality")})");
            }
                

            // 8. DR Exception
            string deviceState = Get("Device_State");
            string environment = Get("Environment");
            if (hostName == "SBICSRV07" && (deviceState != "DR-Passive" || environment != "DR"))
            {
                errors.Add($"Name/DeviceState/Environment Error (Found: HostName = {hostName}, DeviceState = {deviceState}, Environment = {environment})");
            }
            else if (hostName != "SBICSRV07" && (deviceState != "DC-Active" || environment != "PR"))
            {
                errors.Add($"Name/DeviceState/Environment Error (Found: HostName = {hostName}, DeviceState = {deviceState}, Environment = {environment})");
            }

            // 9. Application Tag Presence
            string application = Get("Application");
            if (application == null || application == " " || application == "")
                errors.Add("Application Tag Error (Missing)");

            // 10. OS Integrated = Yes
            if ((!Get("OS_Integrated_Status").Equals("Integrated")) && !(Get("OS_Integrated_Status").Equals("Not Applicable")))
                errors.Add($"OS Integrated Status (Found: {Get("OS_Integrated_Status")})");

            // 11. Freshness of Log Date
            string logDateStr = Get("lastLogRecieved");

            DateTime thisDay = DateTime.Today;

            // Parse the log string (known to be in d/M/yyyy format)
            DateTime logDate = DateTime.ParseExact(logDateStr, "d/M/yyyy", CultureInfo.InvariantCulture);

            // Compare dates directly
            if (logDate.Date != thisDay)
            {
                // Format for readable output (optional)
                string formattedLogDate = logDate.ToString("M/d/yyyy");
                string formattedToday = thisDay.ToString("M/d/yyyy");

                errors.Add($"Freshness of Log Date Error (Found: Log Date = {formattedLogDate}, Expected = {formattedToday})");
            }


            // 12. Field Completeness
            if ((hostName == null || hostName == " ") || (hostIP == null || hostIP == " ") || (logStatus == null || logStatus == " ") || (deviceType == null || deviceType == " ") || (logStatus == null || logStatus == " ") || (Get("Device_Criticality") == null || Get("Device_Criticality") == " ") || (Get("Device_State") == null || Get("Device_State") == " ") || (Get("OS_Integrated_Status") == null || Get("OS_Integrated_Status") == " ") || (logDateStr == null || logDateStr == " "))
            {
                errors.Add("Field Completeness Error (One or more fields are empty)");
            }

            return errors;
        }

        public class ValidationResult
        {
            public int Row { get; set; }
            public string FailedValidations { get; set; }
        }
    }
}
