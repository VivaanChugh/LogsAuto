<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="Vivaan.Modules.LogsAuto.View" %>

<div class="logs-auto-container">
    <h2>CSV Log Validator</h2>

    <label for="ddlLogType"><strong>Select Log Type:</strong></label>
    <asp:DropDownList ID="ddlLogType" runat="server" CssClass="form-control dropdown">
        <asp:ListItem Text="Log Type 1" Value="1" />
        <asp:ListItem Text="Log Type 2" Value="2" />
        <asp:ListItem Text="Log Type 3" Value="3" />
        <asp:ListItem Text="Log Type 4" Value="4" />
        <asp:ListItem Text="Log Type 5" Value="5" />
        <asp:ListItem Text="Log Type 6" Value="6" />
        <asp:ListItem Text="Log Type 7" Value="7" />
        <asp:ListItem Text="Log Type 8" Value="8" />
        <asp:ListItem Text="Log Type 9" Value="9" />
    </asp:DropDownList>

    <asp:DropDownList ID="DropDownList1" runat="server" CssClass="form-control dropdown">
        <asp:ListItem Text="Checking Date" Value="1" />
        <asp:ListItem Text="Not Checking Date" Value="2" />
    </asp:DropDownList>



    <asp:FileUpload ID="fuCsvUpload" runat="server" CssClass="btn btn-primary"/>
    <asp:Button ID="btnValidate" runat="server" Text="Validate CSV" OnClick="btnValidate_Click" CssClass="btn btn-primary" />

    <br /><br />

    <asp:Label ID="lblResult" runat="server" Font-Bold="true" />

    <br /><br />

    <asp:GridView ID="gvValidationResults" runat="server"
                  AutoGenerateColumns="false"
                  GridLines="Both"
                  ShowHeader="true"
                  
                  BorderStyle="Solid"
                  BorderWidth="1px"
                  CellPadding="5"
                  CssClass="validation-grid">
        <Columns>
            <asp:BoundField DataField="Row" HeaderText="Row Number" />
            <asp:BoundField DataField="FailedValidations" HeaderText="Validation Errors" />
        </Columns>
    </asp:GridView>
</div>

<style>
    .logs-auto-container {
        padding: 20px;
        background-color: #f9f9f9;
        border: 1px solid;
        border-radius: 6px;
        max-width: 1600px;
    }


    .validation-grid {
        border-collapse: collapse;
        width: 100%;
    }

    .dropdown {
        padding: 12px 24px;
        border: 1px solid;
        border-radius: 8px;
        font-size: 0.875rem;
        font-weight: 600;
        cursor: pointer;
        transition: all 0.2s ease;
        text-decoration: none;
        display: inline-flex;
        align-items: center;
        justify-content: center;
        min-width: 120px;
        white-space: nowrap;
    }



    .btn {
        padding: 12px 24px;
        border: none;
        border-radius: 8px;
        font-size: 0.875rem;
        font-weight: 600;
        cursor: pointer;
        transition: all 0.2s ease;
        text-decoration: none;
        display: inline-flex;
        align-items: center;
        justify-content: center;
        min-width: 120px;
        white-space: nowrap;
    }

    .btn-primary {
        background: #2563eb;
        color: white;
    }

    .btn-primary:hover {
        background: #1d4ed8;
        transform: translateY(-1px);
        box-shadow: 0 4px 12px rgba(37, 99, 235, 0.4);
    }

    .validation-grid th,
    .validation-grid td {
        border: 1px solid #ccc;
        padding: 8px;
        text-align: left;
    }

    .validation-grid th {
        background-color: #f0f0f0;
        font-weight: bold;
    }
</style>
