# LogsAuto - DNN Module

**LogsAuto** is a custom DNN module designed for intranet environments where users can upload log files (CSV format). The module performs a series of validations on each row and displays a summary of validation errors or confirms a successful upload.

---

## 🧩 Features

- Upload CSV files via web interface
- Automatically validates each row for:
  - Host-IP format
  - Required fields present
  - Valid log status
  - Date/time format and range
  - Consistency checks across records
- Shows all failed validations in a structured table
- Summary message for successful uploads

---

## 📁 Folder Structure

DesktopModules/
└── LogsAuto/
├── View.ascx
├── View.ascx.cs
├── View.ascx.resx
├── Components/
│ └── LogValidator.cs
├── Data/
│ └── DataProvider.cs
├── Providers/
│ └── DataProviders/
│ └── SqlDataProvider/
│ └── SqlDataProvider.cs
├── Settings.ascx
├── ModuleBase.cs
└── Resources/



---

## 🚀 Getting Started

### Prerequisites

- DotNetNuke 10+
- .NET Framework 4.8
- Visual Studio 2022
- Local DNN instance set up in IIS (`C:\inetpub\wwwroot\DNN`)

### Installation

1. Clone this repo or place the folder under:
C:\inetpub\wwwroot\DNN\DesktopModules\LogsAuto

2. Add the project to your DNN solution in Visual Studio.

3. Make sure the project references DNN assemblies from:
C:\inetpub\wwwroot\DNN\bin\

4. Set `Copy Local = False` for DNN references.

5. Build the project in **Debug | Any CPU** mode.

---

## ⚙️ Usage

1. Navigate to a page in your DNN portal.
2. Add the **LogsAuto** module.
3. Upload a `.csv` file using the upload interface.
4. Review:
- Error table if validation fails.
- Success message if all validations pass.

---

## 🧪 Validation Rules

The module checks for the following:

| # | Validation Rule                   |
|--:|-----------------------------------|
| 1 | Hostname and IP must match format |
| 2 | Required fields must not be empty |
| 3 | Log status must be valid (e.g., Success/Fail) |
| 4 | Date must follow `MM/dd/yyyy HH:mm` |
| 5 | Time must be within expected range |
| ... | Up to 12 validations customizable |

---

## 📌 Known Issues

- Uploads with thousands of rows may slow down rendering
- Only `.csv` format supported for now
- Does not support multiple uploads in one session

---

## 📄 License

This module is proprietary and developed for internal use. Contact the author for permission to reuse.

---

## 👨‍💻 Author

Vivaan Chugh  
University of Waterloo  
Email: [your-email@example.com]

